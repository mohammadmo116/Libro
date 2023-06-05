using Libro.Application.ReadingLists.Commands;
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
    public sealed class GetReadingListWithBooksQueryHandler : IRequestHandler<GetReadingListWithBooksQuery, (ReadingList, int)>
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly ILogger<GetReadingListWithBooksQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GetReadingListWithBooksQueryHandler(
            IReadingListRepository readingListRepository,
            ILogger<GetReadingListWithBooksQueryHandler> logger,
            IUnitOfWork unitOfWork
            )
        {
            _readingListRepository = readingListRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(ReadingList,int)> Handle(GetReadingListWithBooksQuery request, CancellationToken cancellationToken)
        {
            return await _readingListRepository.GetReadingListWithBooksAsync(request.UserId,request.ReadingListId, request.PageNumber, request.Count);
         
        }



    }
}
