using Discerniy.API.Tools;
using Discerniy.Domain.Entity.DomainEntity;
using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Repositories;
using Discerniy.Domain.Interface.Services;
using Discerniy.Infrastructure.Repository;
using Discerniy.Infrastructure.Services;
using Discerniy.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VoDA.AspNetCore.Services.Email;
using RabbitMQ.Client;
using Discerniy.Infrastructure;
using Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers;
using Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers.WebSockerHandlers;
using Discerniy.Domain.Interface.Commands;
using Discerniy.Infrastructure.Commands.DeviceWebSocketHandlers;
using Discerniy.API.Middleware;
using Discerniy.Domain.Responses;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// use 0.0.0.0 to listen on all interfaces
builder.WebHost.UseUrls("https://0.0.0.0:7225");

builder.Configuration.AddJsonFile("CORS.json", optional: false, reloadOnChange: true);
var corsConfig = builder.Configuration.Get<CorsConfig>() ?? throw new ArgumentNullException(nameof(CorsConfig));
builder.Services.AddSingleton(corsConfig);

builder.Services.Configure<AppConfig>(builder.Configuration);

var config = builder.Configuration.Get<AppConfig>() ?? throw new ArgumentNullException(nameof(AppConfig));
builder.Services.AddSingleton(config.MongoDb);
builder.Services.AddSingleton(config.Robot);
builder.Services.AddSingleton(config.RabbitMq);
builder.Services.AddSingleton(config.Jwt);
builder.Services.AddSingleton(config.AuthService);
builder.Services.AddSingleton(config.Url);
builder.Services.AddSingleton(config.Confirmation);

builder.Services.AddSignalR();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config.Jwt.Issuer,
            ValidAudience = config.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Jwt.Secret))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = config.Redis.Configuration;
    o.InstanceName = config.Redis.InstanceName;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = config.RabbitMq.Host,
        Port = config.RabbitMq.Port,
        UserName = config.RabbitMq.Username,
        Password = config.RabbitMq.Password,
        ClientProvidedName = config.InstanceName
    };
    return factory;
});
builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<IRandomGenerator, RandomGenerator>();
builder.Services.AddSingleton<IDeviceWebSocketManager, DeviceWebSocketManager>();
builder.Services.AddScoped<IConfirmationManager, ConfirmationManager>();

builder.Services.AddSingleton<IMarkRepository, MarkRepository>();
builder.Services.AddSingleton<IRobotRepository, RobotRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IGroupRepository, GroupRepository>();
builder.Services.AddSingleton<IRepository<RobotModel>, RobotRepository>();
builder.Services.AddSingleton<IRepository<UserModel>, UserRepository>();

builder.Services.AddScoped<IMarkService, MarkService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRobotService, RobotService>();
builder.Services.AddScoped<IClientService<UserModel, UserResponse>, UserService>();
builder.Services.AddScoped<IClientService<RobotModel, RobotResponse>, RobotService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.LoadCOSR();

builder.Services.AddEmailService(options =>
{
    options.Host = config.Email.Host;
    options.Port = config.Email.Port;
    options.DisplayName = config.Email.DisplayName;
    options.Email = config.Email.Email;
    options.Password = config.Email.Password;
    options.EnableSsl = config.Email.EnableSsl;
    options.UseDefaultCredentials = config.Email.UseDefaultCredentials;
    options.EmailTemplatesFolder = config.Email.EmailTemplatesFolder;
});

builder.Services.AddScoped<IDeviceWebSocketCommandHandler, DeviceWebSocketCommandHandler>();
builder.Services.AddScoped<IDeviceWebSocketHandler, DeviceWebSocketUpdateLocation>();

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddSingleton<IMessagePublisherFactory, MessagePublisherFactory>();

builder.Services.AddSingleton<IWebSocketMessagePublisher, WebSocketMessagePublisher>();
builder.Services.AddSingleton<IRabbitMqWebSocketHandler, UpdateUserUpdateLocationIntervalWebSockerHandler>();
builder.Services.AddSingleton<IRabbitMqWebSocketHandler, LocationUpdatedWebSockerHandler>();
builder.Services.AddSingleton<IRabbitMqWebSocketHandler, SendNearClientsWebSockerHandler>();
builder.Services.AddSingleton<IRabbitMqWebSocketHandler, WarningWebSockerHandler>();

builder.Services.AddSingleton<IConsumerHandler, RabbitMqWebSocketHandler>();

builder.Services.AddHostedService<RabbitMqConsumerService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCORS();

app.UseHttpsRedirection();

app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<DeviceWebSocketMiddleware>();

app.MapHub<UserConnectionHub>("/connect");

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    Preloader.Preload(scope.ServiceProvider);
}

app.Run();
