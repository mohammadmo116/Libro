using FluentValidation;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
