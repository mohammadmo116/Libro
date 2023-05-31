using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IWeatherRepository
    {
        public Task<List<WeatherForecast>> Get();
    }
}