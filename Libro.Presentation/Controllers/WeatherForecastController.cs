using Libro.Domain.Entities;
using MediatR;
using Libro.Application.SampleEntity.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Libro.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}