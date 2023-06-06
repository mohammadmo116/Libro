using FluentValidation;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.ReadingList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
