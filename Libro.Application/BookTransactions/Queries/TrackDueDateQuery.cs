using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.BookTransactions.Queiries
{
    public sealed record TrackDueDateQuery(int PageNumber, int Count) : IRequest<(List<BookTransaction>, int)>;

}
