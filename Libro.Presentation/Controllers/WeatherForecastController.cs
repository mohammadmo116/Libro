using Libro.Domain.Entities;
using MediatR;
using Libro.Application.WeatherForecasts.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Libro.WebApi.Controllers
{
    [ApiController]
    [Route("WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
   

        private readonly IMediator _mediator;


        public WeatherForecastController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<ActionResult<List<WeatherForecast>>> Get()
        {
     
             var query = new GetWeatherForecastQuery();
            var Result = await _mediator.Send(query);
            return Ok(Result);
        }
       /* [HttpGet("d")]
        public async Task<ActionResult<List<string>>> Geta()
        {
           return _context.Roles.Include(e=>e.Users.Where(e=>e.Id==new Guid("1C4C200A-F632-11ED-B67E-0242AC120002"))).Select(e=>e.Name).ToList();
       
        }*/


    }
}