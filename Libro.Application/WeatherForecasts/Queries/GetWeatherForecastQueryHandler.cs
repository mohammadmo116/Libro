using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Libro.Application.WeatherForecasts.Queries
{
    public class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, List<WeatherForecast>>
    {

        private readonly ILogger<WeatherForecast> _logger;
        private readonly IWeatherRepository _weatherRepository;

        public GetWeatherForecastQueryHandler(ILogger<WeatherForecast> logger,
                                               IWeatherRepository weatherRepository)
        {
            _logger = logger;
            _weatherRepository = weatherRepository;
        }

        public Task<List<WeatherForecast>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("getting weather forecast...");
            return _weatherRepository.Get();




        }
    }
}

