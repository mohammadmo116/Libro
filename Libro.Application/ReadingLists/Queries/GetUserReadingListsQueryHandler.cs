using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using MediatR;

namespace Libro.Application.ReadingLists.Queries
{
    public sealed class GetUserReadingListsQueryHandler : IRequestHandler<GetUserReadingListsQuery, (List<ReadingList>, int)>
    {
        private readonly IReadingListRepository _readingListRepository;

        public GetUserReadingListsQueryHandler(
            IReadingListRepository readingListRepository
            )
        {
            _readingListRepository = readingListRepository;
        }
        public async Task<(List<ReadingList>, int)> Handle(GetUserReadingListsQuery request, CancellationToken cancellationToken)
        {
            return await _readingListRepository.GetReadingListsByUserAsync(request.UserId, request.PageNumber, request.Count);

        }
    }
}
