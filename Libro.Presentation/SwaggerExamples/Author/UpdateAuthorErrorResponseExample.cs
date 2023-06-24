using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.Author
{
    public class UpdateAuthorErrorResponseExample : IExamplesProvider<ErrorResponse>
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
                        Message="Date Of Birth must be less than now"
                        },
                        new() {
                         FieldName = "Id",
                         Message="Bad Id"
                        }

                }

            };
        }
    }
}
