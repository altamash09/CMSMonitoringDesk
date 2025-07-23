using MonitoringAPI.Models;

namespace MonitoringAPI.Services
{
    public interface ISignalRNotificationService
    {
        Task SendUserStatusUpdate(UserStatusUpdateDto statusUpdate);
        Task SendMonitoringUpdate(MonitoringUpdateDto monitoringUpdate);
        Task SendSLAUpdate(SLAUpdateDto slaUpdate);
        Task SendDashboardUpdate(DashboardSummaryDto dashboardData);
        Task SendErrorMessage(string message, string connectionId = null);
    }
}