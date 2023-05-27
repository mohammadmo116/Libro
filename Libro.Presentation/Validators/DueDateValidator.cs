using FluentValidation;
using Libro.Presentation.Dtos.BookTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
