using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.BookReviews.Queries
{
    public sealed record class GetBookReviewsQuery(Guid BookId, int PageNumber, int Count)
                    : IRequest<(List<BookReview>, int)>;

}
