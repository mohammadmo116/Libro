using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.BookReviews.Queries
{
    public sealed class GetBookReviewsQueryHandler : IRequestHandler<GetBookReviewsQuery, (List<BookReview>, int)>
    {
        private readonly ILogger<GetBookReviewsQueryHandler> _logger;
        private readonly IBookRepository _bookRepository;


        public GetBookReviewsQueryHandler(
            ILogger<GetBookReviewsQueryHandler> logger,
            IBookRepository bookRepository

            )
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public async Task<(List<BookReview>, int)> Handle(GetBookReviewsQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookAsync(request.BookId);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException BookId:{request.BookId}");
                throw new CustomNotFoundException("Book");

            }
            return await _bookRepository.GetReviewsAsync(request.BookId, request.PageNumber, request.Count);
        }
    }
}
