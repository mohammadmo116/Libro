using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
