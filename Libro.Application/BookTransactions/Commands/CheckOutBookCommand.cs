using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed record CheckOutBookCommand(Guid UserId, Guid BookId, DateTime dueDate) : IRequest
    {
    }
}
