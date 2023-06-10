using FluentValidation;
using Libro.Presentation.Dtos.ReadingList;

namespace Libro.Presentation.Validators
{
    public class CreateReadingListDtoValidator : AbstractValidator<CreateReadingListDto>
    {
        public CreateReadingListDtoValidator()
        {
            RuleFor(a => a.Private).NotNull().NotEmpty().WithMessage("The Private field is required.");
        }

    }
}
