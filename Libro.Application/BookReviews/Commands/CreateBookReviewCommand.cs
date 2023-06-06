using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookReviews.Commands
{
    public sealed record class CreateBookReviewCommand(BookReview BookReview):IRequest<BookReview>;
   
}
