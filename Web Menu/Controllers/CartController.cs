using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMenu.BusinessLogic.Interfaces;
using Web_Menu.Models;
using WebMenu.ViewModels;


namespace Web_Menu.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IGameService _gameService;
        private readonly IEmailSender _emailSender;

        public CartController(ICartService cartService, IGameService gameService, IEmailSender emailSender)
        {
            _cartService = cartService;
            _gameService = gameService;
            _emailSender = emailSender;
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
            if (!ModelState.IsValid)
            {
                model.CartItems = _cartService.GetCartItems(GetCartId());
                model.TotalAmount = model.CartItems.Sum(i => i.Game.Price * i.Quantity);
                
            }

            var cartItems = _cartService.GetCartItems(GetCartId());
            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }


            _cartService.ClearCart(GetCartId());

            var userEmail = User.Identity.Name;
            SendPurchaseConfirmationEmail(userEmail, cartItems);

            return RedirectToAction("PurchaseConfirmation");
        }

        public IActionResult PurchaseConfirmation()
        {
            return View();
        }

        private void SendPurchaseConfirmationEmail(string userEmail, List<CartItem> cartItems)
        {
            var gamesHtml = string.Join("", cartItems.Select(item => $@"
        <div style='display: flex; align-items: center; margin-bottom: 15px;'>
            <div>
                <p style='margin: 0; font-size: 16px; font-weight: bold;'>{item.Game.Title}</p>
                <p style='margin: 0; font-size: 14px; color: #555;'>Price: ${item.Game.Price}</p>
            </div>
        </div>"));

            var totalAmount = cartItems.Sum(item => item.Game.Price * item.Quantity);

            var subject = "Purchase Confirmation - Game Store";
            var message = $@"
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; padding: 20px;'>
            <h2 style='color: #00285c;'>Thank you for your purchase!</h2>
            <p>You have successfully purchased the following games:</p>
            <div style='border: 1px solid #ddd; padding: 10px; border-radius: 5px;'>
                {gamesHtml}
            </div>
            <h3 style='color: #00509e; margin-top: 20px;'>Total Amount: <span style='color: #222;'>${totalAmount}</span></h3>
            <p>The games will now appear in your library. You can access them anytime by logging into your account.</p>
            <hr style='margin: 20px 0; border: 0; border-top: 1px solid #eee;' />
            <p style='font-size: 12px; color: #777;'>If you have any questions or issues, feel free to contact our support team.</p>
            <p style='font-size: 12px; color: #777;'>Thank you for shopping with us!</p>
        </div>";

            try
            {
                _emailSender.SendEmailAsync(userEmail, subject, message).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }



    }
}
