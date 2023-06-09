using Libro.Domain.Entities;
using MediatR;
using Libro.Application.WeatherForecasts.Queries;
using Microsoft.AspNetCore.Mvc;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Libro.Infrastructure.Hubs;

namespace Libro.WebApi.Controllers
{

    [ApiController]
    [Route("WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INotificationRepository _notificationRepository;



        public WeatherForecastController(IMediator mediator, INotificationRepository notificationRepository
            , IHubContext<NotificationHub> hubContext)
        {
            _mediator = mediator;
            _notificationRepository = notificationRepository;

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
  


    }


}

