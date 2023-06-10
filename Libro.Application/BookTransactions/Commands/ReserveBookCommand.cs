using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed record ReserveBookCommand(BookTransaction BookTransaction) : IRequest<bool>;

}
