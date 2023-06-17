﻿using Libro.Domain.Responses;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.SwaggerExamples.ReadingList
{
    public class UpdateReadingListErrorResponseExample : IExamplesProvider<ErrorResponse>
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
                        FieldName = "Id",
                        Message="bad Id"
                    },
                }

            };
        }
    }
}
