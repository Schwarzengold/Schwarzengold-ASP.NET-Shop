using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Web_Menu.Models;

namespace WebMenu.BusinessLogic.Validators
{
    public class GameValidator : AbstractValidator<Game>
    {
        public GameValidator()
        {
            RuleFor(game => game.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(2, 100).WithMessage("Title must be between 2 and 100 characters.");

            RuleFor(game => game.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .LessThanOrEqualTo(10000).WithMessage("Price must be less than or equal to $10,000.");
        }
    }
}