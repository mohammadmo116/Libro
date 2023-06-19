using FluentValidation;
using Libro.Presentation.Dtos.Author;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
