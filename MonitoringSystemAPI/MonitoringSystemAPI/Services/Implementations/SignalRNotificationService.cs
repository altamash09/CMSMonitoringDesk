using Microsoft.AspNetCore.SignalR;
using MonitoringAPI.Models;
using MonitoringAPI.Hubs;

namespace MonitoringAPI.Services
{
    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly IHubContext<MonitoringHub> _hubContext;
        private readonly ILogger<SignalRNotificationService> _logger;

        public SignalRNotificationService(
            IHubContext<MonitoringHub> hubContext,
            ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendUserStatusUpdate(UserStatusUpdateDto statusUpdate)
        {
            try
            {
                await _hubContext.Clients.Group("MonitoringGroup")
                    .SendAsync("UserStatusUpdate", statusUpdate);

                _logger.LogInformation($"Sent user status update for {statusUpdate.UserType} ID: {statusUpdate.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending user status update");
            }
        }

        public async Task SendMonitoringUpdate(MonitoringUpdateDto monitoringUpdate)
        {
            try
            {
                await _hubContext.Clients.Group("MonitoringGroup")
                    .SendAsync("MonitoringStatsUpdate", monitoringUpdate);

                _logger.LogInformation($"Sent monitoring update for status: {monitoringUpdate.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending monitoring update");
            }
        }

        public async Task SendSLAUpdate(SLAUpdateDto slaUpdate)
        {
            try
            {
                await _hubContext.Clients.Group("MonitoringGroup")
                    .SendAsync("SLAUpdate", slaUpdate);

                _logger.LogInformation($"Sent SLA update for hour: {slaUpdate.Hour}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SLA update");
            }
        }

        public async Task SendDashboardUpdate(DashboardSummaryDto dashboardData)
        {
            try
            {
                await _hubContext.Clients.Group("MonitoringGroup")
                    .SendAsync("DashboardDataUpdate", dashboardData);

                _logger.LogInformation("Sent complete dashboard data update");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending dashboard update");
            }
        }

        public async Task SendErrorMessage(string message, string connectionId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId)
                        .SendAsync("Error", message);
                }
                else
                {
                    await _hubContext.Clients.Group("MonitoringGroup")
                        .SendAsync("Error", message);
                }

                _logger.LogInformation($"Sent error message: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending error message");
            }
        }
    }
}