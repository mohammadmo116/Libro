using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace Libro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(config =>
                config.RegisterServicesFromAssemblies(
                  assembly));
   
            var smtpClient = configuration["Email:SmtpClient"];
            var userName = configuration["Email:UserName"];
            var password = configuration["Email:Password"];

            services.AddFluentEmail("Libro@gmail.com", "Libro")
                   .AddRazorRenderer()
                 .AddSmtpSender(new SmtpClient(smtpClient)
                 {
                     Port = 587,
                     UseDefaultCredentials = false,
                     Credentials = new NetworkCredential(userName, password),
                     EnableSsl = true
                 });

            return services;
        }
    }
}
