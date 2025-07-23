using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using MonitoringAPI.Models;
using MonitoringAPI.Data;
using MonitoringAPI.Services;

namespace MonitoringSystemAPI.Services.Implementations
{
    public class SLAService : ISLAService
    {
        private readonly MonitoringDbContext _context;
        private readonly ILogger<SLAService> _logger;

        public SLAService(MonitoringDbContext context, ILogger<SLAService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SLADataDto>> GetSLADataAsync(DateTime date)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetSLAData";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Date", date.Date));

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                var result = new List<SLADataDto>();

                while (await reader.ReadAsync())
                {
                    result.Add(new SLADataDto
                    {
                        Hour = reader.GetInt32("Hour"),
                        Completed = reader.GetInt32("Completed"),
                        Percentage = reader.GetDecimal("Percentage")
                    });
                }

                return result.OrderBy(s => s.Hour).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SLA data");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task UpdateSLARecordAsync(int hour, int completed, decimal percentage, DateTime date)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateSLARecord";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Hour", hour));
                command.Parameters.Add(new SqlParameter("@Completed", completed));
                command.Parameters.Add(new SqlParameter("@Percentage", percentage));
                command.Parameters.Add(new SqlParameter("@Date", date.Date));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SLA record");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<decimal> GetCurrentSLAPercentageAsync()
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetCurrentSLAPercentage";
                command.CommandType = CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();

                return result != null ? Convert.ToDecimal(result) : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current SLA percentage");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }
}