using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.Role
{
    public class CreateRoleErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                    new List<ErrorModel>() {
                        new() {
                        FieldName = "Name",
                        Message   = "The Role With Name patron Already Exists"
                        }
                    }

            };
        }
    }
}
