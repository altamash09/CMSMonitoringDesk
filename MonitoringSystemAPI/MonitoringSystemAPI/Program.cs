using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using MonitoringAPI.Services;
using MonitoringAPI.Hubs;
using MonitoringAPI.Data;
using MonitoringSystemAPI.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context - using connection string only
builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR
builder.Services.AddSignalR();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:3001",    
                "https://localhost:3001"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Service registrations
builder.Services.AddScoped<IMonitoringService, MonitoringService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IReviewerService, ReviewerService>();
builder.Services.AddScoped<ISLAService, SLAService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();

// RabbitMQ Configuration - Using the correct interface
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetService<IConfiguration>();
    return new ConnectionFactory()
    {
        HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
        Port = int.Parse(configuration["RabbitMQ:Port"] ?? "15672"),
        UserName = configuration["RabbitMQ:UserName"] ?? "guest",
        Password = configuration["RabbitMQ:Password"] ?? "guest",
        VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/"
    };
});

// Background Services
builder.Services.AddHostedService<MonitoringAPI.BackgroundServices.RabbitMQConsumerService>();
builder.Services.AddHostedService<MonitoringAPI.BackgroundServices.SqlServiceBrokerService>();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MonitoringHub>("/monitoringhub");

app.Run();