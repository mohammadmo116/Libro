using FluentValidation.AspNetCore;
using Libro.Domain.Responses;
using Libro.Presentation.SwaggerExamples;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Libro.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentaion(this IServiceCollection services)
        {
           
            services.AddSwaggerGen(options =>
            {
                
             
                options.SwaggerDoc("v1", new OpenApiInfo
                {

                    Title = "Libro.WebApi",
                    Version = "V1",
                    Description = "An ASP.NET Core Web API for Library System",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = " - Email",
                        Email = "momousa.ce@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }


                });

                options.EnableAnnotations();
                options.ExampleFilters();




                // using System.Reflection;
                var xmlFilename = $"{typeof(Libro.Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "please insert Bearer token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
              
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
                                        new OpenApiSecurityScheme{
                                            Reference=new OpenApiReference{
                                                 Type=ReferenceType.SecurityScheme,
                                                 Id="Bearer"
                                            }
                                        },
        new List<string>()
        }
    });


            });
            services.AddFluentValidationRulesToSwagger();
            services.AddSwaggerExamplesFromAssemblyOf<Libro.Presentation.AssemblyReference>();

            return services;
        }
    }
}
