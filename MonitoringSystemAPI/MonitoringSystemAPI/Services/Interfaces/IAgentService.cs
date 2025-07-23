using MonitoringAPI.Models;

namespace MonitoringAPI.Services
{
    public interface IAgentService
    {
        Task<List<AgentDto>> GetAllAgentsAsync();
        Task<AgentDto> GetAgentByIdAsync(int id);
        Task UpdateAgentStatusAsync(int id, string status);
        Task UpdateAgentPerformanceAsync(int id, int completed, decimal estimatedHours, decimal actualHours);
    }
}