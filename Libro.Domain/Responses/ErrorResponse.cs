using System.Net;

namespace Libro.Domain.Responses
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {

        }
        public ErrorResponse(HttpStatusCode status)
        {
            Status = status;
            Errors = new List<ErrorModel>() { };
        }

        public HttpStatusCode Status { get; set; }
        public List<ErrorModel>? Errors { get; set; }

    }
}
