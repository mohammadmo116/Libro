using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.BookTransactions.Queries
{
    public sealed record GetBookTransactionQuery(Guid UserId, Guid TransactionId) : IRequest<BookTransaction>;
}
