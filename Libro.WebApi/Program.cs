using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Libro.Application;
using Libro.Infrastracture;
using Libro.Infrastructure.Authorization;
using Libro.Infrastructure.Hubs;
using Libro.Infrastructure.Jobs;
using Libro.Presentation;
using Libro.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
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
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
        ClockSkew = TimeSpan.FromSeconds(5)
    });


services.AddAuthorization();
services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
services.AddSingleton<IAuthorizationPolicyProvider, RoleAuthorizationPolicyProvider>();


services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment)
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
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();



}
//HangFire
app.UseHangfireDashboard();

////HangFire Jobs 
//every Minute
//RecurringJob.AddOrUpdate<JobToNotifyPatronDueForDateBooks>("my-job-id", job => job.ExecuteAsync(), "* * * * *");
//every day at 7:00:00 am
RecurringJob.AddOrUpdate<JobToNotifyPatronDueForDateBooks>("my-job-id", job => job.ExecuteAsync(), "00 07 * * *");


//Serilog
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/NotificationHub").RequireAuthorization();
app.Run();

public partial class Program { }
