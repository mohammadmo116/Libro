using FluentValidation;
using Libro.Presentation.Dtos.Author;

namespace Libro.Presentation.Validators
{
    public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
    {

        public CreateAuthorDtoValidator()
        {
            RuleFor(a => a.DateOfBirth).LessThan(DateTime.UtcNow);
        }
    }
}
