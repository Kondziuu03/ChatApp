using ChatApp.API.Extensions;
using ChatApp.API.Hubs;
using ChatApp.API.Middleware;
using Microsoft.SemanticKernel.Memory;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddConfiguration(configuration);
builder.Services.AddServices();
builder.Services.AddOptions(configuration);
builder.Services.AddValidators();
builder.Services.AddKernelWithOllamaConfiguration(configuration);

var orgin = configuration["Origin"] ?? throw new NullReferenceException("Empty orgin");

builder.Services.AddCors(options =>
    options.AddPolicy("Origin",
        builder =>
        {
            builder.WithOrigins(orgin)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        }));

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Origin");

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapHub<MessageHub>("messageHub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
