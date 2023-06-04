using FluentValidation;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Validators
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>

    { 
    public CreateBookDtoValidator() {
            RuleFor(a => a.PublishedDate).LessThan(DateTime.UtcNow);
            RuleFor(a => a.IsAvailable).NotNull().NotEmpty().WithMessage("The IsAvailable field is required.") ;
        }
    }
}
