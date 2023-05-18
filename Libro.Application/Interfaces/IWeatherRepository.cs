using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IWeatherRepository
    {
        Task<List<WeatherForecast>> Get();
    }
}