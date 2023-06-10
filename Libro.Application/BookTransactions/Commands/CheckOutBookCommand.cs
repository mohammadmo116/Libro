using MediatR;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed record CheckOutBookCommand(Guid TransactionId, DateTime DueDate) : IRequest<bool>;

}
