using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed record class CreateReadingListCommand(Guid UserId, ReadingList ReadingList) : IRequest<ReadingList>;

}
