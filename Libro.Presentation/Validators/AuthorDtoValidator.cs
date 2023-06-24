using FluentValidation;
using Libro.Presentation.Dtos.Author;

namespace Libro.Presentation.Validators
{
    public class AuthorDtoValidator : AbstractValidator<AuthorDto>
    {
        public AuthorDtoValidator()
        {
            RuleFor(a => a.DateOfBirth).LessThan(DateTime.UtcNow);
        }
    }
}
