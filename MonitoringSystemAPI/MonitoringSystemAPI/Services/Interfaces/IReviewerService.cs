using MonitoringAPI.Models;

namespace MonitoringAPI.Services
{
    public interface IReviewerService
    {
        Task<List<ReviewerDto>> GetAllReviewersAsync();
        Task<ReviewerDto> GetReviewerByIdAsync(int id);
        Task UpdateReviewerStatusAsync(int id, string status);
        Task UpdateReviewerPerformanceAsync(int id, int completed, decimal estimatedHours, decimal actualHours);
    }
}