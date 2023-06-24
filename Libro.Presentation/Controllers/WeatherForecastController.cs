using Libro.Application.WeatherForecasts.Queries;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Hubs;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Libro.WebApi.Controllers
{

    [ApiController]
    [Route("WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public WeatherForecastController(IMediator mediator, INotificationRepository notificationRepository
            , IHubContext<NotificationHub> hubContext,
            ApplicationDbContext context)
        {
            _context = context;
            _mediator = mediator;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }
        //pass array of roles

        [HasRole("admin,patron")]
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<ActionResult<List<WeatherForecast>>> Get()
        {
            var query = new GetWeatherForecastQuery();
            var Result = await _mediator.Send(query);
            return Ok(Result);
        }

        [HttpGet("aa", Name = "Geet")]
        public async Task<ActionResult> Geet()
        {

            return Ok("D");
        }



    }


}

