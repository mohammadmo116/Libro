using FluentValidation;
using FluentValidation.AspNetCore;
using Libro.Application;
using Libro.Infrastracture;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Hubs;
using Libro.Presentation;
using Libro.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddApplicationPart(typeof(Libro.Presentation.AssemblyReference).Assembly)
    .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
   .ConfigureApiBehaviorOptions(options =>
    options.SuppressModelStateInvalidFilter = true);

services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<Libro.Presentation.AssemblyReference>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    });


services.AddAuthorization();
services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();


services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Libro.WebApi", Version = "V1" });

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

services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentaion();
builder.Host.UseSerilog((context, config) =>
        config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();
try
{

    Log.Information("Application Starting Up");
}
catch (Exception ex)
{
    Log.Fatal(ex, "the Application faild to start");


}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();



}
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/NotificationHub").RequireAuthorization();
app.Run();
