using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMenu.BusinessLogic.Services;
using Web_Menu.Models;
using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using WebMenu.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebMenu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameService _gameService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger, IGameService gameService, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _gameService = gameService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _gameService.GetAllGamesAsync();
            return View(games);
        }

        public async Task<IActionResult> GameDetails(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StyleGroups = GetStyleGroupsSelectList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = await MapViewModelToGameAsync(model);

                try
                {
                    await _gameService.CreateGameAsync(game);
                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
            }

            ViewBag.StyleGroups = GetStyleGroupsSelectList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var model = MapGameToViewModel(game);

            ViewBag.StyleGroups = GetStyleGroupsSelectList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GameViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var game = await MapViewModelToGameAsync(model);

                try
                {
                    await _gameService.UpdateGameAsync(game);
                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
            }

            ViewBag.StyleGroups = GetStyleGroupsSelectList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _gameService.DeleteGameAsync(id);
            return RedirectToAction("Index");
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

        private async Task<Game> MapViewModelToGameAsync(GameViewModel model)
        {
            var game = await _gameService.GetGameByIdAsync(model.Id) ?? new Game
            {
                Title = model.Title,
                Quote = model.Quote,
                TrailerUrl = model.TrailerUrl,
                Overview = model.Overview,
                Gameplay = model.Gameplay,
                StyleGroup = model.StyleGroup,
                Price = model.Price,
                GalleryImages = new List<string>(),
                Characters = new List<Character>()
            };

            game.Title = model.Title;
            game.Quote = model.Quote;
            game.TrailerUrl = model.TrailerUrl;
            game.Overview = model.Overview;
            game.Gameplay = model.Gameplay;
            game.StyleGroup = model.StyleGroup;
            game.Price = model.Price;

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
                game.GalleryImages.Clear();
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
                var existingCharacters = game.Characters.ToList();

                foreach (var charModel in model.Characters)
                {
                    var existingCharacter = existingCharacters.FirstOrDefault(c => c.Id == charModel.Id);
                    if (existingCharacter != null)
                    {
                        existingCharacter.Name = charModel.Name;
                        existingCharacter.Description = charModel.Description;

                        if (charModel.Photo != null)
                        {
                            string photoPath = await SaveImage(charModel.Photo);
                            existingCharacter.PhotoUrl = photoPath;
                        }
                    }
                    else
                    {
                        var newCharacter = new Character
                        {
                            Name = charModel.Name,
                            Description = charModel.Description
                        };

                        if (charModel.Photo != null)
                        {
                            string photoPath = await SaveImage(charModel.Photo);
                            newCharacter.PhotoUrl = photoPath;
                        }

                        game.Characters.Add(newCharacter);
                    }
                }

                var characterIdsFromModel = model.Characters.Select(c => c.Id).ToList();
                var charactersToRemove = existingCharacters.Where(c => !characterIdsFromModel.Contains(c.Id)).ToList();

                foreach (var characterToRemove in charactersToRemove)
                {
                    game.Characters.Remove(characterToRemove);
                }
            }
            else
            {
                game.Characters.Clear();
            }

            return game;
        }

        private GameViewModel MapGameToViewModel(Game game)
        {
            var model = new GameViewModel
            {
                Id = game.Id,
                Title = game.Title,
                Quote = game.Quote,
                TrailerUrl = game.TrailerUrl,
                Overview = game.Overview,
                Gameplay = game.Gameplay,
                StyleGroup = game.StyleGroup,
                Price = game.Price,
                NumberOfCharacters = game.Characters.Count,
                Characters = game.Characters.Select(c => new CharacterViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };

            return model;
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

            if (!System.IO.File.Exists(path))
            {
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }

            return "/images/" + fileName;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
