using Libro.Domain.Entities;
using MediatR;
using System.Collections.Generic;


namespace Libro.Application.SampleEntity.Queries
{
    public class GetWeatherForecastQuery : IRequest<List<WeatherForecast>>
    {
 
    }
}
