using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed record UpdateReadingListCommand(Guid UserId, ReadingList ReadingList) : IRequest<bool>;

}
