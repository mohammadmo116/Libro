using FluentValidation;
using Libro.Presentation.Dtos.Book;

namespace Libro.Presentation.Validators
{
    public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
    {
        public UpdateBookDtoValidator()
        {
            RuleFor(a => a.PublishedDate).LessThan(DateTime.UtcNow);
        }
    }
}
