using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.ReadingListExceptions;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed class RemoveBookFromUserReadingListCommandHandler : IRequestHandler<RemoveBookFromUserReadingListCommand, bool>
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<RemoveBookFromUserReadingListCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveBookFromUserReadingListCommandHandler(
             IReadingListRepository readingListRepository,
             IBookRepository bookRepository,
             ILogger<RemoveBookFromUserReadingListCommandHandler> logger,
             IUnitOfWork unitOfWork
            )
        {
            _readingListRepository = readingListRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(RemoveBookFromUserReadingListCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookAsync(request.BookReadingList.BookId);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException(Book)");
                _logger.LogInformation($"BookId : {request.BookReadingList.BookId}");
                throw new CustomNotFoundException("Book");
            }
            var readinList = await _readingListRepository.GetReadingListByUserAsync(request.UserId, request.BookReadingList.ReadingListId);
            if (readinList is null)
            {
                _logger.LogInformation($"CustomNotFoundException(ReadingList)");
                _logger.LogInformation($"BookId : {request.BookReadingList.ReadingListId}");
                throw new CustomNotFoundException("ReadingList");
            }
            if (!await _readingListRepository.ContainsTheBook(request.BookReadingList))
            {
                _logger.LogInformation($"BookIsNotInTheReadingListException");
                _logger.LogInformation($"ReadingListId : {request.BookReadingList.ReadingListId}, BookId : {request.BookReadingList.BookId}");
                throw new ReadingListDoesNotContainTheBookException();
            }
            _readingListRepository.RemoveBookFromReadingList(request.BookReadingList);
            var NumberOfRows = await _unitOfWork.SaveChangesAsync();
            return NumberOfRows > 0;
        }
    }
}
