using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web_Menu.Models;
using WebMenu.BusinessLogic.Interfaces;
using WebMenu.DataAccess.Interfaces;

namespace WebMenu.BusinessLogic.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> _cartItemRepository;

        public CartService(IRepository<CartItem> cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public void AddToCart(string cartId, Game game)
        {
            var cartItem = _cartItemRepository.FindAsync(
                c => c.CartId == cartId && c.GameId == game.Id).Result.FirstOrDefault();

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    GameId = game.Id,
                    Quantity = 1,
                    CartId = cartId
                };
                _cartItemRepository.InsertAsync(cartItem).Wait();
            }
            else
            {
                cartItem.Quantity++;
                _cartItemRepository.UpdateAsync(cartItem).Wait();
            }
        }

        public void RemoveFromCart(string cartId, int gameId)
        {
            var cartItem = _cartItemRepository.FindAsync(
                c => c.CartId == cartId && c.GameId == gameId).Result.FirstOrDefault();

            if (cartItem != null)
            {
                _cartItemRepository.DeleteAsync(cartItem.Id).Wait();
            }
        }

        public List<CartItem> GetCartItems(string cartId)
        {
            return _cartItemRepository.FindAsync(c => c.CartId == cartId, includeProperties: "Game").Result.ToList();
        }

        public void ClearCart(string cartId)
        {
            var cartItems = _cartItemRepository.FindAsync(c => c.CartId == cartId).Result;
            foreach (var item in cartItems)
            {
                _cartItemRepository.DeleteAsync(item.Id).Wait();
            }
        }
    }
}
