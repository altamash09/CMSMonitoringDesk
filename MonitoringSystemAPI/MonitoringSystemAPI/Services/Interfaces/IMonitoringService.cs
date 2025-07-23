using MonitoringAPI.Models;

namespace MonitoringAPI.Services
{
    public interface IMonitoringService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime date, bool isBacklog);
        Task<MonitoringStatsDto> GetMonitoringStatsAsync(DateTime date, bool isBacklog);
        Task<List<SLADataDto>> GetSLADataAsync(DateTime date);
        Task UpdateMonitoringRecordAsync(string status, int count, DateTime date, bool isBacklog);
    }
}