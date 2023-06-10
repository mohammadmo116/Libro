using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.BookReviews.Commands
{
    public sealed class CreateBookReviewCommandHandler : IRequestHandler<CreateBookReviewCommand, BookReview>
    {
        private readonly ILogger<CreateBookReviewCommandHandler> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookTransactionRepository _bookTransactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookReviewCommandHandler(
            ILogger<CreateBookReviewCommandHandler> logger,
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IBookTransactionRepository bookTransactionRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _bookTransactionRepository = bookTransactionRepository;
        }
        public async Task<BookReview> Handle(CreateBookReviewCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserAsync(request.BookReview.UserId);
            if (user is null)
            {
                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
            }
            var book = await _bookRepository.GetBookAsync(request.BookReview.BookId);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException BookId:{request.BookReview.BookId}");
                throw new CustomNotFoundException("Book");

            }
            if (!await _bookTransactionRepository.BookIsReturnedAsync(request.BookReview.UserId, request.BookReview.BookId))
            {
                throw new NotAllowedToReviewBookException();

            }
            if (await _bookRepository.BookIsReviewedByUser(request.BookReview.UserId, request.BookReview.BookId))
            {
                throw new BookIsAlreadyReviewedException();

            }
            await _bookRepository.CreateReviewAsync(request.BookReview);
            await _unitOfWork.SaveChangesAsync();
            return request.BookReview;
        }
    }
}
