using Libro.Domain.Entities;
using MediatR;
using System.Collections.Generic;


namespace Libro.Application.WeatherForecasts.Queries
{
    public class GetWeatherForecastQuery : IRequest<List<WeatherForecast>>
    {
 
    }
}
