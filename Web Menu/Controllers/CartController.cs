using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMenu.BusinessLogic.Interfaces;
using Web_Menu.Models;
using WebMenu.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web_Menu.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IGameService _gameService;

        public CartController(ICartService cartService, IGameService gameService)
        {
            _cartService = cartService;
            _gameService = gameService;
        }

        private string GetCartId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Guid.NewGuid().ToString();
            }
            return User.Identity.Name;
        }

        public async Task<IActionResult> Index()
        {
            var cartItems = _cartService.GetCartItems(GetCartId());
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game != null)
            {
                _cartService.AddToCart(GetCartId(), game);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            _cartService.RemoveFromCart(GetCartId(), id);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cartItems = _cartService.GetCartItems(GetCartId());
            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }
            var totalAmount = cartItems.Sum(i => i.Game.Price * i.Quantity);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = totalAmount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutConfirmed(CheckoutViewModel model)
        {
            Console.WriteLine($"Field: dsasdadasdsadsadadsdasad");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                model.CartItems = _cartService.GetCartItems(GetCartId());
                model.TotalAmount = model.CartItems.Sum(i => i.Game.Price * i.Quantity);
                
            }

            var cartItems = _cartService.GetCartItems(GetCartId());
            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            SendConfirmationEmail(User.Identity.Name, cartItems);

            _cartService.ClearCart(GetCartId());

            return RedirectToAction("PurchaseConfirmation");
        }

        public IActionResult PurchaseConfirmation()
        {
            return View();
        }

        private void SendConfirmationEmail(string userEmail, List<CartItem> cartItems)
        {
            Console.WriteLine($"Email sent to {userEmail} with purchase details.");
        }
    }
}
