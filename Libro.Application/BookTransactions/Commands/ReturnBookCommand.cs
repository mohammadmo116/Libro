using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed record ReturnBookCommand(Guid TransactionId) : IRequest<bool>;
}
