using FluentValidation;
using Libro.Presentation.Dtos.ReadingList;

namespace Libro.Presentation.Validators
{
    public class CreateReadingListDtoValidator : AbstractValidator<CreateReadingListDto>
    {
        public CreateReadingListDtoValidator()
        {
            RuleFor(a => a.Name).NotEmpty().NotNull().WithMessage("The Private field is required.");
        }

    }
}
