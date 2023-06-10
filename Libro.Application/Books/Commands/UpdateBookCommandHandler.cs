using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Books.Commands
{
    public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly ILogger<UpdateBookCommandHandler> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookCommandHandler(
            ILogger<UpdateBookCommandHandler> logger,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _bookRepository = bookRepository;
        }
        public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookAsync(request.Book.Id);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException BookId:{request.Book.Id}");
                throw new CustomNotFoundException("Book");

            }
            book.Title = request.Book.Title is null ?
              book.Title : request.Book.Title;

            book.Genre = request.Book.Genre is null ?
                book.Genre : request.Book.Genre;

            book.PublishedDate = request.Book.PublishedDate is null ?
                book.PublishedDate : request.Book.PublishedDate;

            book.IsAvailable = request.Book.IsAvailable is null ?
                book.IsAvailable : request.Book.IsAvailable;

            _bookRepository.UpdateBook(book);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;

        }
    }
}
