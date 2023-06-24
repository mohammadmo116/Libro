using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.Author
{
    public class CreateAuthorErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                    new List<ErrorModel>() {
                        new() {
                        FieldName = "DateOfBirth",
                        Message="'DateOfBirth' must be less than now"
                        },
                         new() {
                        FieldName = "Name",
                        Message="The Name field is required."
                        },

                    }

            };
        }
    }
}
