using Libro.Application.Interfaces;
using Libro.Domain.Entities;

namespace Libro.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private static readonly string[] Summaries = new[]
   { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        public async Task<List<WeatherForecast>> Get()
        {
            var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
        .ToList();
            return weatherForecasts;

        }
    }
}

