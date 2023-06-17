using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Libro.Application.BookTransactions.Queries
{
    public sealed record GetBookTransactionQuery(Guid UserId,Guid TransactionId):IRequest<BookTransaction>;
}
