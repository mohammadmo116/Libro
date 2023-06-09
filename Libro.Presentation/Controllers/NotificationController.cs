using Libro.Application.Notifications.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Notification")]
    public class NotificationController : ControllerBase
    {

        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HasRole("librarian")]
        [HttpPost("ReservedBooks", Name = "ReservedBooks")]
        public async Task<ActionResult> NotifyPatronForReservedBooks()
        {
            try
            {
                
                var request = new NotifyPatronsForReservedBooksCommand();
                var Result = await _mediator.Send(request);
                return Result ? Ok("Patrons Has Been Notified") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e){

                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }
        }
        [HasRole("librarian")]
        [HttpPost("DueDates", Name = "DueDates")]
        public async Task<ActionResult> NotifyPatronForDueDates()
        {
            try
            {

                var request = new NotifyPatronsForDueDatesCommand();
                var Result = await _mediator.Send(request);
                return Result ? Ok("Patrons Has Been Notified") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {

                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }
        }
    }
}
