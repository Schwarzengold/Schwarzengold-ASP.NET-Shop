using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web_Menu.Models;

namespace Web_Menu.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public int Quantity { get; set; }

        public string CartId { get; set; }
    }
}
