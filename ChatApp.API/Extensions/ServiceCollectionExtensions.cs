using ChatApp.API.Middleware;
using ChatApp.Core.Application.Services;
using ChatApp.Core.Application.SKPlugins;
using ChatApp.Core.Domain;
using ChatApp.Core.Domain.Dtos;
using ChatApp.Core.Domain.Dtos.Validators;
using ChatApp.Core.Domain.Interfaces.Producer;
using ChatApp.Core.Domain.Interfaces.Repositories;
using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Options;
using ChatApp.Infrastructure.Producer;
using ChatApp.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using System.Text;

namespace ChatApp.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            AddCustomAuthentication(services, configuration);

            var connectionString = configuration.GetValue<string>("ConnectionString");
            services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IKafkaProducer, KafkaProducer>();

            services.AddSingleton(new UserConnectionService());

            services.AddScoped<ErrorHandlingMiddleware>();

            services.AddSignalR();

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettingsOption>(options => configuration.GetSection(nameof(JwtSettingsOption)).Bind(options));
            services.Configure<KafkaOption>(options => configuration.GetSection(nameof(KafkaOption)).Bind(options));

            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();

            return services;
        }

        public static IServiceCollection AddKernelWithOllamaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();

                var configuration = sp.GetRequiredService<IConfiguration>();
                var ollamaModelId = configuration["Ollama:ModelId"] ?? throw new NullReferenceException("Empty ollama model");
                var ollamaEndpoint = configuration["Ollama:Endpoint"] ?? throw new NullReferenceException("Empty ollama endpoint");

                try
                {
                    kernelBuilder.AddOpenAIChatCompletion(modelId: ollamaModelId, apiKey: "ollama", endpoint: new Uri(ollamaEndpoint));
                }
                catch (Exception ex)
                {
                    var logger = sp.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Failed to configure Ollama. Make sure Ollama is running at {Endpoint} with model {ModelId}", ollamaEndpoint, ollamaModelId);

                    throw;
                }

                var kernel = kernelBuilder.Build();

                var messagePlugin = new MessagePlugin(kernel);
                kernel.Plugins.AddFromObject(messagePlugin);

                return kernel;
            });

            return services;
        }

        private static void AddCustomAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(nameof(JwtSettingsOption)).Get<JwtSettingsOption>();

            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
                throw new NullReferenceException("Secret Key is empty");

            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = GetTokenValidationParams(key);
                options.Events = GetEvents();
            });
        }

        private static JwtBearerEvents GetEvents()
        {
            return new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["token"];
                    var path = context.HttpContext.Request.Path;

                    if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messageHub"))
                        context.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        }

        private static TokenValidationParameters GetTokenValidationParams(byte[] key)
            => new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(120)
            };
    }
}
