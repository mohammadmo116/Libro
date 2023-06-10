using MediatR;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed record ReturnBookCommand(Guid TransactionId) : IRequest<bool>;
}
