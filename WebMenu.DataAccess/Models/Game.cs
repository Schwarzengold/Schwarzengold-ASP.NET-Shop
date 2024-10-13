﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_Menu.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Quote { get; set; }
        public string TrailerUrl { get; set; }
        public string Overview { get; set; }
        public string Gameplay { get; set; }
        public List<Character> Characters { get; set; } = new List<Character>();
        public string BackgroundImageUrl { get; set; }
        public string BannerImageUrl { get; set; }
        public string CardImageUrl { get; set; }
        public List<string> GalleryImages { get; set; } = new List<string>();
        public string StyleGroup { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $1000.00.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }

        public string PhotoUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
