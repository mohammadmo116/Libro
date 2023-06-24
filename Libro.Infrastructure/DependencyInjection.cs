using Hangfire;
using Libro.Application.Interfaces;
using Libro.Application.Repositories;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Libro.Infrastracture
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {


                if (environment.IsDevelopment())
                    options.UseSqlServer(configuration
                        .GetConnectionString("sqlServerDevelopment")
                        ?? throw new InvalidOperationException("Connection string 'sqlServer' not found."));
                if (environment.IsProduction())
                    options.UseSqlServer(configuration
                        .GetConnectionString("sqlServerProduction")
                        ?? throw new InvalidOperationException("Connection string 'sqlServerDocker' not found."));
                else if (environment.EnvironmentName == "IntegrationTesting")
                    options.UseInMemoryDatabase("IntegrationTestingDb");


            });

            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookTransactionRepository, BookTransactionRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IReadingListRepository, ReadingListRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSignalR();
            //configure Hangfire
            if(environment.IsDevelopment())
                services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("sqlServerDevelopment") ?? throw new InvalidOperationException("Connection string 'sqlServerDevelopment' not found.")));
            if (environment.IsProduction())
                services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("sqlServerProduction") ?? throw new InvalidOperationException("Connection string 'sqlServerProduction' not found.")));

            services.AddHangfireServer();
            return services;
        }
    }
}
