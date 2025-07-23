using MonitoringAPI.Models;

namespace MonitoringAPI.Services
{
    public interface ISLAService
    {
        Task<List<SLADataDto>> GetSLADataAsync(DateTime date);
        Task UpdateSLARecordAsync(int hour, int completed, decimal percentage, DateTime date);
        Task<decimal> GetCurrentSLAPercentageAsync();
    }
}