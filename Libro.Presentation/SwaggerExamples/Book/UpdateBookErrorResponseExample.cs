﻿using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.SwaggerExamples.Book
{

    public class UpdateBookErrorResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse(HttpStatusCode.BadRequest)
            {

                Status = HttpStatusCode.BadRequest,
                Errors =
                new List<ErrorModel>() {
                        new() {
                        FieldName = "PublishedDate",
                        Message="Published Date' must be less than now"
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
