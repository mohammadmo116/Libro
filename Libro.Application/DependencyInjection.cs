
using FluentEmail.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace Libro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(config =>
                config.RegisterServicesFromAssemblies(
                  assembly));

            services.AddFluentEmail("Libro@gmail.com", "Libro")
                   .AddRazorRenderer()
                 .AddSmtpSender(new SmtpClient("sandbox.smtp.mailtrap.io")
                 {
                     Port = 587,
                     UseDefaultCredentials = false,
                     Credentials = new NetworkCredential("d0fd5bb36db97b", "075cccf39ffd54"),
                     EnableSsl = true
                 });
           
            return services;
        }
    }
}
