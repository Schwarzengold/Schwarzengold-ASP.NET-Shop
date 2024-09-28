using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Web_Menu.Models
{
    public class GameViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Quote { get; set; }

        [Url]
        public string TrailerUrl { get; set; }

        public string Overview { get; set; }

        public string Gameplay { get; set; }

        [Range(1, 3, ErrorMessage = "Number of characters must be between 1 and 3.")]
        public int NumberOfCharacters { get; set; } = 1;

        public List<CharacterViewModel> Characters { get; set; } = new List<CharacterViewModel>();

        public IFormFile BackgroundImage { get; set; }

        public IFormFile BannerImage { get; set; }

        public IFormFile CardImage { get; set; }

        public List<IFormFile> GalleryImages { get; set; } = new List<IFormFile>();

        [Required]
        public string StyleGroup { get; set; }
    }

    public class CharacterViewModel
    {
        public IFormFile Photo { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
