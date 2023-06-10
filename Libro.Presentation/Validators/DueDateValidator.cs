using FluentValidation;
using Libro.Presentation.Dtos.BookTransaction;

namespace Libro.Presentation.Validators
{
    public class DueDateValidator : AbstractValidator<DueDateDto>
    {
        public DueDateValidator()
        {
            RuleFor(a => a.DueDate).NotEmpty().NotNull().GreaterThan(DateTime.UtcNow);

        }
    }
}
