using Libro.Domain.Entities;
using MediatR;
using Libro.Application.WeatherForecasts.Queries;
using Microsoft.AspNetCore.Mvc;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Libro.Infrastructure.Hubs;
using Libro.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Libro.Infrastructure;

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
 
        [HttpGet("R",Name = "Geet")]
        public async Task<ActionResult<List<WeatherForecast>>> Geet()
        {
            //await _hubContext.Clients.User("cffe5b7d-bf22-42f4-b46e-75f84f7a3926").SendAsync("aa", "Trst");
            var userIds = _context.Users
                .Include(a => a.Roles)
                .Where(u => u.Roles.Any(a => a.Name == "patron"))
                .Select(a => a.Id.ToString()).ToList();
        


            return Ok(userIds);
      
        }

       

    }


}

