using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.BackgroundServices
{
    public class SqlServiceBrokerService : BackgroundService
    {
        private readonly ILogger<SqlServiceBrokerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly int _pollingInterval;

        public SqlServiceBrokerService(
            ILogger<SqlServiceBrokerService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _pollingInterval = configuration.GetValue<int>("ServiceBroker:PollingInterval", 5000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SQL Service Broker Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessServiceBrokerMessages();
                    await Task.Delay(_pollingInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SQL Service Broker Service");
                    await Task.Delay(10000, stoppingToken); // Wait longer on error
                }
            }
        }

        private async Task ProcessServiceBrokerMessages()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_ReceiveServiceBrokerMessages", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 30;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                try
                {
                    var messageType = reader["MessageType"]?.ToString();
                    var messageBody = reader["MessageBody"]?.ToString();
                    var timestamp = reader.GetDateTime("Timestamp");

                    await ProcessMessage(messageType, messageBody, timestamp);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Service Broker message");
                }
            }
        }

        private async Task ProcessMessage(string messageType, string messageBody, DateTime timestamp)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<ISignalRNotificationService>();
            var monitoringService = scope.ServiceProvider.GetRequiredService<IMonitoringService>();

            _logger.LogInformation($"Processing Service Broker message: {messageType}");

            switch (messageType?.ToUpper())
            {
                case "MONITORING_STATS_UPDATE":
                    await HandleMonitoringStatsUpdate(messageBody, notificationService, monitoringService);
                    break;

                case "SLA_DATA_UPDATE":
                    await HandleSLADataUpdate(messageBody, notificationService);
                    break;

                case "DASHBOARD_REFRESH":
                    await HandleDashboardRefresh(notificationService, monitoringService);
                    break;

                case "USER_PERFORMANCE_UPDATE":
                    await HandleUserPerformanceUpdate(messageBody, notificationService);
                    break;

                default:
                    _logger.LogWarning($"Unknown Service Broker message type: {messageType}");
                    break;
            }
        }

        private async Task HandleMonitoringStatsUpdate(string messageBody, ISignalRNotificationService notificationService, IMonitoringService monitoringService)
        {
            try
            {
                var updateData = JsonSerializer.Deserialize<Dictionary<string, object>>(messageBody);

                if (updateData.TryGetValue("Status", out var statusObj) &&
                    updateData.TryGetValue("Count", out var countObj))
                {
                    var status = statusObj.ToString();
                    var count = Convert.ToInt32(countObj);

                    var monitoringUpdate = new MonitoringUpdateDto
                    {
                        Status = status,
                        Count = count,
                        Timestamp = DateTime.UtcNow
                    };

                    await notificationService.SendMonitoringUpdate(monitoringUpdate);

                    // Also update the database record
                    await monitoringService.UpdateMonitoringRecordAsync(status, count, DateTime.Today, false);

                    _logger.LogInformation($"Processed monitoring stats update: {status} = {count}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling monitoring stats update");
            }
        }

        private async Task HandleSLADataUpdate(string messageBody, ISignalRNotificationService notificationService)
        {
            try
            {
                var updateData = JsonSerializer.Deserialize<Dictionary<string, object>>(messageBody);

                if (updateData.TryGetValue("Hour", out var hourObj) &&
                    updateData.TryGetValue("Completed", out var completedObj) &&
                    updateData.TryGetValue("Percentage", out var percentageObj))
                {
                    var hour = Convert.ToInt32(hourObj);
                    var completed = Convert.ToInt32(completedObj);
                    var percentage = Convert.ToDecimal(percentageObj);

                    var slaUpdate = new SLAUpdateDto
                    {
                        Hour = hour,
                        Completed = completed,
                        Percentage = percentage,
                        Timestamp = DateTime.UtcNow
                    };

                    await notificationService.SendSLAUpdate(slaUpdate);

                    _logger.LogInformation($"Processed SLA data update: Hour {hour} = {completed} ({percentage}%)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling SLA data update");
            }
        }

        private async Task HandleDashboardRefresh(ISignalRNotificationService notificationService, IMonitoringService monitoringService)
        {
            try
            {
                // Get fresh dashboard data and send to all clients
                var dashboardData = await monitoringService.GetDashboardSummaryAsync(DateTime.Today, false);
                await notificationService.SendDashboardUpdate(dashboardData);

                _logger.LogInformation("Processed dashboard refresh request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling dashboard refresh");
            }
        }

        private async Task HandleUserPerformanceUpdate(string messageBody, ISignalRNotificationService notificationService)
        {
            try
            {
                var updateData = JsonSerializer.Deserialize<Dictionary<string, object>>(messageBody);

                if (updateData.TryGetValue("UserId", out var userIdObj) &&
                    updateData.TryGetValue("UserType", out var userTypeObj) &&
                    updateData.TryGetValue("Completed", out var completedObj))
                {
                    var userId = Convert.ToInt32(userIdObj);
                    var userType = userTypeObj.ToString();
                    var completed = Convert.ToInt32(completedObj);

                    // This would trigger a user-specific update
                    // For now, we'll trigger a general dashboard refresh
                    using var scope = _serviceProvider.CreateScope();
                    var monitoringService = scope.ServiceProvider.GetRequiredService<IMonitoringService>();
                    var dashboardData = await monitoringService.GetDashboardSummaryAsync(DateTime.Today, false);
                    await notificationService.SendDashboardUpdate(dashboardData);

                    _logger.LogInformation($"Processed user performance update: {userType} {userId} = {completed}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling user performance update");
            }
        }
    }
}