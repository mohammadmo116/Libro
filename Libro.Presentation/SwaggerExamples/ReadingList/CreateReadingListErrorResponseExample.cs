using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.ReadingList
{
    public class CreateReadingListErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                new List<ErrorModel>() {
                    new()
                    {
                        FieldName = "Name",
                        Message="The Name field is required."
                    },
                    new()
                    {
                        FieldName = "Private",
                        Message="The Private field is required."
                    }
                }

            };
        }
    }
}
