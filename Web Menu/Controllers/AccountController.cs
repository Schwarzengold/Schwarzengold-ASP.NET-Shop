using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMenu.ViewModels;
using Web_Menu.Models;
using WebMenu.BusinessLogic.Interfaces;

namespace Web_Menu.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Buyer");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        Request.Scheme);

                    SendRegistrationConfirmationEmail(model.Email, confirmationLink);

                    return RedirectToAction("ConfirmRegistrationPrompt");
                }

                AddErrors(result);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "Your email has been successfully confirmed! You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Error confirming your email. Please try again.";
            return RedirectToAction("Register");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ViewData["ErrorMessage"] = "You need to confirm your email to log in. Please check your inbox for the confirmation email.";
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewData["ErrorMessage"] = "Invalid login attempt. Please check your email and password.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ConfirmRegistrationPrompt()
        {
            return View();
        }

        private void SendRegistrationConfirmationEmail(string userEmail, string confirmationLink)
        {
            var subject = "Confirm Your Email - Game Store";
            var message = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; padding: 20px;'>
                    <h2 style='color: #00285c;'>Welcome to the Game Store!</h2>
                    <p>Thank you for registering with us! To complete your registration, please confirm your email address by clicking the link below:</p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <a href='{confirmationLink}' 
                           style='display: inline-block; background-color: #00509e; color: #fff; padding: 10px 20px; text-decoration: none; font-size: 16px; border-radius: 5px;'>
                           Confirm Email
                        </a>
                    </div>
                    <p>If the button above doesn't work, copy and paste the following URL into your browser:</p>
                    <p style='background-color: #f9f9f9; padding: 10px; border-radius: 5px; color: #00509e;'>{confirmationLink}</p>
                    <hr style='margin: 20px 0; border: 0; border-top: 1px solid #eee;' />
                    <p style='font-size: 12px; color: #777;'>If you did not register on Game Store, please ignore this email.</p>
                    <p style='font-size: 12px; color: #777;'>For any questions or support, contact us at <a href='mailto:support@gamestore.com' style='color: #00509e;'>support@gamestore.com</a>.</p>
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}