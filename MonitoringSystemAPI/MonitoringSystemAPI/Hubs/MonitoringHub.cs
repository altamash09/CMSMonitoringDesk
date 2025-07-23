using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Services;

namespace MonitoringAPI.Hubs
{
    [Authorize]
    public class MonitoringHub : Hub
    {
        private readonly ILogger<MonitoringHub> _logger;
        private readonly IMonitoringService _monitoringService;

        public MonitoringHub(ILogger<MonitoringHub> logger, IMonitoringService monitoringService)
        {
            _logger = logger;
            _monitoringService = monitoringService;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var user = Context.User?.Identity?.Name;

            _logger.LogInformation($"User {user} connected with connection ID: {connectionId}");

            // Join the monitoring group for real-time updates
            await Groups.AddToGroupAsync(connectionId, "MonitoringGroup");

            // Send current dashboard data to the newly connected client
            var dashboardData = await _monitoringService.GetDashboardSummaryAsync(DateTime.Today, false);
            await Clients.Caller.SendAsync("DashboardDataUpdate", dashboardData);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var user = Context.User?.Identity?.Name;

            _logger.LogInformation($"User {user} disconnected. Connection ID: {connectionId}");

            await Groups.RemoveFromGroupAsync(connectionId, "MonitoringGroup");
            await base.OnDisconnectedAsync(exception);
        }

        // Client can request specific date data
        public async Task RequestDashboardData(DateTime date, bool isBacklog)
        {
            try
            {
                var dashboardData = await _monitoringService.GetDashboardSummaryAsync(date, isBacklog);
                await Clients.Caller.SendAsync("DashboardDataUpdate", dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending dashboard data to client");
                await Clients.Caller.SendAsync("Error", "Failed to retrieve dashboard data");
            }
        }

        // Client can join specific monitoring groups
        public async Task JoinMonitoringGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Connection {Context.ConnectionId} joined group {groupName}");
        }

        public async Task LeaveMonitoringGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Connection {Context.ConnectionId} left group {groupName}");
        }
    }
}