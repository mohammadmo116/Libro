﻿using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed class RemoveReadingListCommandHandler : IRequestHandler<RemoveReadingListCommand, bool>
    {
        private readonly IReadingListRepository _readingListRepository;

        private readonly ILogger<RemoveReadingListCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveReadingListCommandHandler(
             IReadingListRepository readingListRepository,

             ILogger<RemoveReadingListCommandHandler> logger,
             IUnitOfWork unitOfWork
            )
        {
            _readingListRepository = readingListRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        public async Task<bool> Handle(RemoveReadingListCommand request, CancellationToken cancellationToken)
        {
            var readingList = await _readingListRepository.GetReadingListByUserAsync(request.UserId, request.ReadingListId);
            if (readingList is null)
            {
                _logger.LogInformation($"CustomNotFoundException readingListId:{request.ReadingListId}");
                throw new CustomNotFoundException("readingList");

            }

            _readingListRepository.RemoveReadingList(readingList);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;
        }
    }
}
