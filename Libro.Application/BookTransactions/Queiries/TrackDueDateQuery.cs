using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.BookTransactions.Queiries
{
    public sealed record TrackDueDateQuery:IRequest<List<BookTransaction>>;
    
}
