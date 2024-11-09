using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web_Menu.Models;

namespace WebMenu.BusinessLogic.Interfaces
{
    public interface ICartService
    {
        void AddToCart(string cartId, Game game);
        void RemoveFromCart(string cartId, int gameId);
        List<CartItem> GetCartItems(string cartId);
        void ClearCart(string cartId);
    }
}
