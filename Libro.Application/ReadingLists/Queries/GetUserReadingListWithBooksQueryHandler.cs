using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using MediatR;

namespace Libro.Application.ReadingLists.Queries
{
    public sealed class GetUserReadingListWithBooksQueryHandler : IRequestHandler<GetUserReadingListWithBooksQuery, (ReadingList, int)>
    {
        private readonly IReadingListRepository _readingListRepository;


        public GetUserReadingListWithBooksQueryHandler(
            IReadingListRepository readingListRepository
            )
        {
            _readingListRepository = readingListRepository;

        }

        public async Task<(ReadingList, int)> Handle(GetUserReadingListWithBooksQuery request, CancellationToken cancellationToken)
        {
            return await _readingListRepository.GetReadingListWithBooksAsync(request.UserId, request.ReadingListId, request.PageNumber, request.Count);

        }



    }
}
