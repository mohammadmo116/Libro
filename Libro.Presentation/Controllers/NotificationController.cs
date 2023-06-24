using Libro.Application.Notifications.Commands;
using Libro.Application.Notifications.Queries;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Notifications;
using Libro.Presentation.SwaggerExamples.Notification;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Notification")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    public class NotificationController : ControllerBase
    {

        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Returns user's notification with pagination
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Route Defualts:
        ///  
        ///     { 
        ///     Defualt:
        ///         PageNumber=0,
        ///         Count=5
        ///     
        ///     Max:
        ///         Count=10
        ///     }
        /// Sample request:
        ///
        ///     GET /Notification?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of Notifications with pagination", typeof(GetNotificationsPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetNotificationsPaginationOkResultExample))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Patron")]

        [HasRole("patron")]
        [HttpGet(Name = "GetUserNotifications")]
        public async Task<ActionResult<(List<GetNotificationsDto>, int)>> GetUserNotifications(int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(userId, out Guid parsedUserId);

            var query = new GetUserNotificaionsQuery(parsedUserId, PageNumber, Count);
            var Result = await _mediator.Send(query);

            return Ok(new { Notifications = Result.Item1.Adapt<List<GetNotificationsDto>>(), Pages = Result.Item2 });

        }


        /// <summary>
        /// Notify Patrons About their reserved books(Push - database - Email notification)
        /// </summary>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST /Notification/ReservedBooks
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Succes when Patrons are Notified")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Librarian")]
        [HasRole("librarian")]
        [HttpPost("ReservedBooks", Name = "NotifyPatronsForReservedBooks")]
        public async Task<ActionResult> NotifyPatronsForReservedBooks()
        {

            var request = new NotifyPatronsForReservedBooksCommand();
            var Result = await _mediator.Send(request);
            return Result ? Ok("Patrons Has Been Notified") : StatusCode(StatusCodes.Status500InternalServerError);


        }
        /// <summary>
        /// Notify Patrons About their DueDate books(Push - database - Email notification)
        /// </summary>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST /Notification/DueDates
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Succes when Patrons are Notified")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Librarian")]
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
