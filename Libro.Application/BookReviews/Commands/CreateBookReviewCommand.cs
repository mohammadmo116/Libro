using Libro.Domain.Entities;
using MediatR;

namespace Libro.Application.BookReviews.Commands
{
    public sealed record class CreateBookReviewCommand(BookReview BookReview) : IRequest<BookReview>;

}
