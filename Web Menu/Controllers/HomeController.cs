using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web_Menu.Data;
using Web_Menu.Models;

namespace Web_Menu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var games = _context.Games.Include(g => g.Characters).ToList();
            return View(games);
        }


        public IActionResult GameDetails(int id)
        {
            var game = _context.Games.Include(g => g.Characters).FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StyleGroups = GetStyleGroupsSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    Title = model.Title,
                    Quote = model.Quote,
                    TrailerUrl = model.TrailerUrl,
                    Overview = model.Overview,
                    Gameplay = model.Gameplay,
                    StyleGroup = model.StyleGroup,
                    GalleryImages = new List<string>(),
                    Characters = new List<Character>()
                };


                if (model.BackgroundImage != null)
                {
                    string backgroundImagePath = await SaveImage(model.BackgroundImage);
                    game.BackgroundImageUrl = backgroundImagePath;
                }


                if (model.BannerImage != null)
                {
                    string bannerImagePath = await SaveImage(model.BannerImage);
                    game.BannerImageUrl = bannerImagePath;
                }


                if (model.CardImage != null)
                {
                    string cardImagePath = await SaveImage(model.CardImage);
                    game.CardImageUrl = cardImagePath;
                }


                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var image in model.GalleryImages)
                    {
                        if (image != null)
                        {
                            string galleryImagePath = await SaveImage(image);
                            game.GalleryImages.Add(galleryImagePath);
                        }
                    }
                }


                if (model.Characters != null && model.Characters.Any())
                {
                    foreach (var charModel in model.Characters)
                    {
                        var character = new Character
                        {
                            Name = charModel.Name,
                            Description = charModel.Description
                        };

                        if (charModel.Photo != null)
                        {
                            string photoPath = await SaveImage(charModel.Photo);
                            character.PhotoUrl = photoPath;
                        }

                        game.Characters.Add(character);
                    }
                }

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.StyleGroups = GetStyleGroupsSelectList();
            return View(model);
        }

        private SelectList GetStyleGroupsSelectList()
        {
            var styleGroups = new List<SelectListItem>
            {
                new SelectListItem { Value = "fallout-page", Text = "Yellow" },
                new SelectListItem { Value = "horizon-page", Text = "Purple" },
                new SelectListItem { Value = "deadspace-page", Text = "Red" },
                new SelectListItem { Value = "lastofus-page", Text = "Orange" },
                new SelectListItem { Value = "skyrim-page", Text = "Blue" }
            };
            return new SelectList(styleGroups, "Value", "Text");
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };

            var extension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Invalid file type.");
                return null;
            }

            if (!allowedContentTypes.Contains(imageFile.ContentType))
            {
                ModelState.AddModelError("", "Invalid content type.");
                return null;
            }


            if (imageFile.Length > (5 * 1024 * 1024))
            {
                ModelState.AddModelError("", "File size exceeds 5 MB.");
                return null;
            }


            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;


            string wwwRootPath = _hostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath, "images", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }


            return "/images/" + fileName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var game = _context.Games.Include(g => g.Characters).FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }


            if (game.Characters != null && game.Characters.Any())
            {
                _context.Characters.RemoveRange(game.Characters);
            }


            _context.Games.Remove(game);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
