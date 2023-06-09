
using Microsoft.Extensions.DependencyInjection;

using Amazon.Runtime;

namespace Libro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(config=>
                config.RegisterServicesFromAssemblies(
                  assembly));
     
            return services;
        }
    }
}
