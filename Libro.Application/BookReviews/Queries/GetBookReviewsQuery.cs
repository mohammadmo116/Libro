using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookReviews.Queries
{
    public sealed record class GetBookReviewsQuery(Guid BookId,int PageNumber, int Count)
                    :IRequest<(List<BookReview>,int)>;
   
}
