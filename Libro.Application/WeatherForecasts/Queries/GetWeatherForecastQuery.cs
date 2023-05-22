using Libro.Domain.Entities;
using MediatR;


namespace Libro.Application.WeatherForecasts.Queries
{
    public class GetWeatherForecastQuery : IRequest<List<WeatherForecast>>
    {
 
    }
}
