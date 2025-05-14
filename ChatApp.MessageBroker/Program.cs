using ChatApp.Core.Domain;
using ChatApp.Core.Domain.Options;
using ChatApp.MessageBroker;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<KafkaOption>(options => configuration.GetSection(nameof(KafkaOption)).Bind(options));

var connectionString = configuration.GetValue<string>("ConnectionString");

builder.Services.AddDbContextFactory<ChatDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();
app.Run();
