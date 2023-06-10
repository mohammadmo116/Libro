using FluentValidation;
using Libro.Presentation.Dtos.Book;

namespace Libro.Presentation.Validators
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>

    {
        public CreateBookDtoValidator()
        {
            RuleFor(a => a.PublishedDate).LessThan(DateTime.UtcNow);
            RuleFor(a => a.IsAvailable).NotNull().NotEmpty().WithMessage("The IsAvailable field is required.");
        }
    }
}
