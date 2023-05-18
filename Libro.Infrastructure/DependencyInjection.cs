
using Libro.Application.Interfaces;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Configuration;

namespace Libro.Infrastracture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
        
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("sqlServer") ?? throw new InvalidOperationException("Connection string 'sqlServer' not found.")));
           
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            return services;
        }
    }
}
