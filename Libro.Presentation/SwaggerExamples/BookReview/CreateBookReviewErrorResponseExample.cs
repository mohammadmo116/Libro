using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.BookReview
{
    public class CreateBookReviewErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                new List<ErrorModel>() {
                        new() {
                        FieldName = "BookReviewNotAllowed",
                        Message="Not allowed to review book, you need to borrow and return the book"
                        },
                        new() {
                         FieldName = "BookIsReviewed",
                         Message="you Already have Reviewed this book"
                        },
                         new() {
                         FieldName = "Rate",
                         Message="The field Rate must be between 1 and 5."
                        }

                }

            };
        }
    }
}
