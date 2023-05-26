using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    class CheckOutBookValidator : AbstractValidator<CheckOutBookCommand>
    {
        public CheckOutBookValidator() {
            RuleFor(x => x.dueDate).NotNull().NotEmpty().GreaterThan(DateTime.UtcNow);
        }
    }
}
