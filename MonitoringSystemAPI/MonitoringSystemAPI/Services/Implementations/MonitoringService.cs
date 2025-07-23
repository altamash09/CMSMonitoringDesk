using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using MonitoringAPI.Models;
using MonitoringAPI.Data;

namespace MonitoringAPI.Services
{
    // Helper class for status configuration
    public class StatusConfiguration
    {
        public string Color { get; set; }
        public bool Mandatory { get; set; }
    }

    public class MonitoringService : IMonitoringService
    {
        private readonly MonitoringDbContext _context;
        private readonly ILogger<MonitoringService> _logger;

        public MonitoringService(MonitoringDbContext context, ILogger<MonitoringService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime date, bool isBacklog)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetDashboardSummary";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Date", date.Date));
                command.Parameters.Add(new SqlParameter("@IsBacklog", isBacklog));

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                var summary = new DashboardSummaryDto
                {
                    MonitoringStats = new MonitoringStatsDto { Stats = new Dictionary<string, MonitoringStatItem>() },
                    SLAData = new List<SLADataDto>(),
                    Agents = new List<AgentDto>(),
                    Reviewers = new List<ReviewerDto>()
                };

                // Read monitoring stats (Result Set 1)
                while (await reader.ReadAsync())
                {
                    var status = reader.GetString("Status");
                    var count = reader.GetInt32("Count");
                    var color = reader.GetString("Color");
                    var mandatory = reader.GetBoolean("Mandatory");

                    summary.MonitoringStats.Stats[status] = new MonitoringStatItem
                    {
                        Count = count,
                        Color = color,
                        Mandatory = mandatory
                    };
                }

                // Read SLA Data (Result Set 2)
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    summary.SLAData.Add(new SLADataDto
                    {
                        Hour = reader.GetInt32("Hour"),
                        Completed = reader.GetInt32("Completed"),
                        Percentage = reader.GetDecimal("Percentage")
                    });
                }

                // Read Agents (Result Set 3)
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    summary.Agents.Add(new AgentDto
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

                // Read Reviewers (Result Set 4)
                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    summary.Reviewers.Add(new ReviewerDto
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

                // Calculate stats
                summary.AgentStats = new UserStatsDto
                {
                    Total = summary.Agents.Count,
                    Online = summary.Agents.Count(a => a.Status == "online"),
                    Idle = summary.Agents.Count(a => a.Status == "idle"),
                    Offline = summary.Agents.Count(a => a.Status == "offline")
                };

                summary.ReviewerStats = new UserStatsDto
                {
                    Total = summary.Reviewers.Count,
                    Online = summary.Reviewers.Count(r => r.Status == "online"),
                    Idle = summary.Reviewers.Count(r => r.Status == "idle"),
                    Offline = summary.Reviewers.Count(r => r.Status == "offline")
                };

                summary.MonitoringStats.RecordDate = date;
                summary.MonitoringStats.IsBacklog = isBacklog;

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard summary");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<MonitoringStatsDto> GetMonitoringStatsAsync(DateTime date, bool isBacklog)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_GetMonitoringStats";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Date", date.Date));
                command.Parameters.Add(new SqlParameter("@IsBacklog", isBacklog));

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                var result = new MonitoringStatsDto
                {
                    Stats = new Dictionary<string, MonitoringStatItem>(),
                    RecordDate = date,
                    IsBacklog = isBacklog
                };

                // Map the default colors and mandatory flags
                var statusConfig = GetStatusConfiguration();

                while (await reader.ReadAsync())
                {
                    var status = reader.GetString("Status");
                    var count = reader.GetInt32("Count");
                    var config = statusConfig.GetValueOrDefault(status, new StatusConfiguration
                    {
                        Color = "from-gray-100 to-gray-200",
                        Mandatory = false
                    });

                    result.Stats[status] = new MonitoringStatItem
                    {
                        Count = count,
                        Color = config.Color,
                        Mandatory = config.Mandatory
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monitoring stats");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
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

                return result;
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

        public async Task UpdateMonitoringRecordAsync(string status, int count, DateTime date, bool isBacklog)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateMonitoringRecord";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Status", status));
                command.Parameters.Add(new SqlParameter("@Count", count));
                command.Parameters.Add(new SqlParameter("@Date", date.Date));
                command.Parameters.Add(new SqlParameter("@IsBacklog", isBacklog));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating monitoring record");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        private Dictionary<string, StatusConfiguration> GetStatusConfiguration()
        {
            return new Dictionary<string, StatusConfiguration>
            {
                ["Un-Monitored"] = new StatusConfiguration { Color = "from-red-100 to-red-200", Mandatory = true },
                ["Monitoring In Process"] = new StatusConfiguration { Color = "from-amber-100 to-yellow-200", Mandatory = true },
                ["Not Ready"] = new StatusConfiguration { Color = "from-gray-100 to-gray-200", Mandatory = true },
                ["WFR"] = new StatusConfiguration { Color = "from-orange-100 to-orange-200", Mandatory = true },
                ["Live"] = new StatusConfiguration { Color = "from-emerald-100 to-green-200", Mandatory = true },
                ["Review In Process"] = new StatusConfiguration { Color = "from-blue-100 to-blue-200", Mandatory = true },
                ["Rejected"] = new StatusConfiguration { Color = "from-red-200 to-red-300", Mandatory = true },
                ["Tickets Opened"] = new StatusConfiguration { Color = "from-cyan-100 to-teal-200", Mandatory = true },
                ["DVR Down"] = new StatusConfiguration { Color = "from-purple-100 to-purple-200", Mandatory = false },
                ["Archive Datalost"] = new StatusConfiguration { Color = "from-pink-100 to-rose-200", Mandatory = false },
                ["On-Holiday"] = new StatusConfiguration { Color = "from-indigo-100 to-indigo-200", Mandatory = false }
            };
        }
    }
}