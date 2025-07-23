using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using MonitoringAPI.Models;
using MonitoringAPI.Data;
using MonitoringAPI.Services;

namespace MonitoringSystemAPI.Services.Implementations
{

    public class ReviewerService : IReviewerService
    {
        private readonly MonitoringDbContext _context;
        private readonly ILogger<ReviewerService> _logger;

        public ReviewerService(MonitoringDbContext context, ILogger<ReviewerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ReviewerDto>> GetAllReviewersAsync()
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetAllReviewers";
                command.CommandType = CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<ReviewerDto>();

                while (await reader.ReadAsync())
                {
                    result.Add(new ReviewerDto
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
                _logger.LogError(ex, "Error getting all reviewers");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<ReviewerDto> GetReviewerByIdAsync(int id)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetReviewerById";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ReviewerId", id));

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ReviewerDto
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
                _logger.LogError(ex, "Error getting reviewer by ID");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task UpdateReviewerStatusAsync(int id, string status)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateReviewerStatus";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ReviewerId", id));
                command.Parameters.Add(new SqlParameter("@Status", status));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reviewer status");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task UpdateReviewerPerformanceAsync(int id, int completed, decimal estimatedHours, decimal actualHours)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateReviewerPerformance";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ReviewerId", id));
                command.Parameters.Add(new SqlParameter("@Completed", completed));
                command.Parameters.Add(new SqlParameter("@EstimatedHours", estimatedHours));
                command.Parameters.Add(new SqlParameter("@ActualHours", actualHours));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reviewer performance");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }
}