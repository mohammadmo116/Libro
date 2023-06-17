using Libro.Domain.Responses;
using Libro.Presentation.SwaggerExamples.Authentication;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.SwaggerExamples.Librarian
{
    public class UpdateLibraraianErrorResponseExample: IExamplesProvider<ErrorResponse>
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
                        FieldName = "PhoneNumber",
                        Message="the error message - example -The PhoneNumber field is not a valid phone number."
                        },
                            new() {
                        FieldName = "Email",
                        Message=" the error message - example - The Email is used or required"
                        },
                             new() {
                        FieldName = "UserName",
                        Message=" the error message - example - The UserName is used or required"
                        },
                              new() {
                        FieldName = "Password",
                        Message=" the error message - example - The Password field is required"
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
