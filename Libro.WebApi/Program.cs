using Libro.Application;
using Libro.Infrastracture;
using Libro.Infrastructure;
using Libro.Presentation;
using Libro.WebApi.Filters;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilter>())
    .AddApplicationPart(typeof(Libro.Presentation.AssemblyReference).Assembly)
    .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
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

app.UseAuthorization();

app.MapControllers();

app.Run();
