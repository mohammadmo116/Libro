using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.ReadingList
{
    public class AddBookToReadingListErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.NotFound,
                Errors =
                new List<ErrorModel>() {
                    new()
                    {
                        FieldName = "Book",
                        Message="404 Book NotFound"
                    },
                    new()
                    {
                        FieldName = "ReadingList",
                        Message="404 ReadingList NotFound"
                    }
                }

            };
        }
    }
}
