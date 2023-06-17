using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.SwaggerExamples.Notification
{
    public class GetNotificationsErrorResponseExample : IExamplesProvider<ErrorResponse>
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
                         FieldName = "UserId",
                         Message="Bad user Id"
                        }

                }

            };
        }
    }
}

