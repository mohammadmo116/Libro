﻿
using Libro.Application.Interfaces;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Libro.Infrastracture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
        
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("sqlServer") ?? throw new InvalidOperationException("Connection string 'sqlServer' not found.")));
       
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookTransactionRepository,BookTransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
