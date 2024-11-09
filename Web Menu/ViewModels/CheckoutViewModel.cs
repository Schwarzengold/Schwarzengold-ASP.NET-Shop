using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web_Menu.Models;

namespace WebMenu.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }

        [Required(ErrorMessage = "You must agree to the purchase policy to proceed.")]
        [Display(Name = "Do you agree with the purchase policy of this service?")]
        public bool AgreeToPolicy { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
