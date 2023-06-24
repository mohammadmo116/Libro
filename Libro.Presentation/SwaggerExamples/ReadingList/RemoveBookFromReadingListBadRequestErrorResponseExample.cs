using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.ReadingList
{
    public class RemoveBookFromReadingListBadRequestErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                new List<ErrorModel>()
                {
                        new() {
                         FieldName = "Book",
                         Message="ReadingList Does Not Contain The Book"
                        }

                }

            };
        }
    }
}
