using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MonitoringAPI.Services;
using MonitoringAPI.Hubs;
using MonitoringAPI.Data;
using MonitoringSystemAPI.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["SecretKey"] ?? "YourSecretKeyHere_MakeItLongAndSecure_AtLeast32Characters";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "MonitoringAPI",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "MonitoringClients",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Configure JWT for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/monitoringhub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

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
                "https://localhost:3001",
                "http://localhost:3002",
                "http://localhost:3002",
                "http://10.144.69.61",
                "https://10.144.69.61"
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

// Log requests before CORS policy
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();
    var path = context.Request.Path;
    var method = context.Request.Method;

    Console.WriteLine($"[CORS Check] Origin: {origin} | Path: {path} | Method: {method}");

    await next();
});

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MonitoringHub>("/monitoringhub");

Console.WriteLine("=== Monitoring API Starting ===");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

app.Run();