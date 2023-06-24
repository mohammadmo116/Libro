using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.BookTransaction
{
    public class CheckOutBookErrorResponseExample : IExamplesProvider<ErrorResponse>
    {

        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                new List<ErrorModel>() {
                        new() {
                        FieldName = "Book",
                        Message="book is already Borrowed"
                        }
                }

            };
        }
    }
}
