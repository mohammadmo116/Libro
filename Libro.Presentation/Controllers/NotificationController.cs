using Libro.Application.Notifications.Commands;
using Libro.Application.Notifications.Queries;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Notifications;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

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

        [HasRole("patron")]
        [HttpGet(Name = "GetUserNotifications")]
        public async Task<ActionResult<(List<GetNotificationsDto>, int)>> GetUserNotifications(int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                return BadRequest("Bad user Id");
            }
            try
            {
                var query = new GetUserNotificaionsQuery(parsedUserId, PageNumber, Count);
                var Result = await _mediator.Send(query);

                return Ok(new { Books = Result.Item1.Adapt<List<GetNotificationsDto>>(), Pages = Result.Item2 });
            }
            catch (CustomNotFoundException e)
            {

                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }
        }

        [HasRole("librarian")]
        [HttpPost("ReservedBooks", Name = "NotifyPatronsForReservedBooks")]
        public async Task<ActionResult> NotifyPatronsForReservedBooks()
        {

            var request = new NotifyPatronsForReservedBooksCommand();
            var Result = await _mediator.Send(request);
            return Result ? Ok("Patrons Has Been Notified") : StatusCode(StatusCodes.Status500InternalServerError);


        }
        [HasRole("librarian")]
        [HttpPost("DueDates", Name = "NotifyPatronsForDueDates")]
        public async Task<ActionResult> NotifyPatronsForDueDates()
        {

            var request = new NotifyPatronsForDueDatesCommand();
            var Result = await _mediator.Send(request);
            return Result ? Ok("Patrons Has Been Notified") : StatusCode(StatusCodes.Status500InternalServerError);


        }
    }
}
