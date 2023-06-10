using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Books.Commands
{
    public sealed class RemoveBookCommandHandler : IRequestHandler<RemoveBookCommand, bool>
    {

        private readonly ILogger<RemoveBookCommandHandler> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveBookCommandHandler(
            ILogger<RemoveBookCommandHandler> logger,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _bookRepository = bookRepository;
        }
        public async Task<bool> Handle(RemoveBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookAsync(request.BookId);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException BookId:{request.BookId}");
                throw new CustomNotFoundException("Book");
            }

            _bookRepository.RemoveBook(book);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;
        }
    }
}
