using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using MonitoringAPI.Models;
using MonitoringAPI.Data;
using MonitoringAPI.Services;

namespace MonitoringSystemAPI.Services.Implementations
{
    public class AgentService : IAgentService
    {
        private readonly MonitoringDbContext _context;
        private readonly ILogger<AgentService> _logger;

        public AgentService(MonitoringDbContext context, ILogger<AgentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AgentDto>> GetAllAgentsAsync()
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetAllAgents";
                command.CommandType = CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<AgentDto>();

                while (await reader.ReadAsync())
                {
                    result.Add(new AgentDto
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Status = reader.GetString("Status"),
                        Completed = reader.GetInt32("Completed"),
                        EstimatedHours = reader.GetDecimal("EstimatedHours"),
                        ActualHours = reader.GetDecimal("ActualHours"),
                        Rank = reader.GetString("Rank")
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all agents");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<AgentDto> GetAgentByIdAsync(int id)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetAgentById";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@AgentId", id));

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new AgentDto
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Status = reader.GetString("Status"),
                        Completed = reader.GetInt32("Completed"),
                        EstimatedHours = reader.GetDecimal("EstimatedHours"),
                        ActualHours = reader.GetDecimal("ActualHours"),
                        Rank = reader.GetString("Rank")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agent by ID");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task UpdateAgentStatusAsync(int id, string status)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateAgentStatus";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@AgentId", id));
                command.Parameters.Add(new SqlParameter("@Status", status));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agent status");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task UpdateAgentPerformanceAsync(int id, int completed, decimal estimatedHours, decimal actualHours)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateAgentPerformance";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@AgentId", id));
                command.Parameters.Add(new SqlParameter("@Completed", completed));
                command.Parameters.Add(new SqlParameter("@EstimatedHours", estimatedHours));
                command.Parameters.Add(new SqlParameter("@ActualHours", actualHours));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agent performance");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }
}