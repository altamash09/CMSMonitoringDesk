using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IMonitoringService _monitoringService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IMonitoringService monitoringService, ILogger<DashboardController> logger)
        {
            _monitoringService = monitoringService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetDashboardSummary(
            [FromQuery] DateTime? date = null,
            [FromQuery] bool isBacklog = false)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var summary = await _monitoringService.GetDashboardSummaryAsync(targetDate, isBacklog);

                return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(summary, "Dashboard data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard summary");
                return StatusCode(500, ApiResponse<DashboardSummaryDto>.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("monitoring-stats")]
        public async Task<ActionResult<ApiResponse<MonitoringStatsDto>>> GetMonitoringStats(
            [FromQuery] DateTime? date = null,
            [FromQuery] bool isBacklog = false)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var stats = await _monitoringService.GetMonitoringStatsAsync(targetDate, isBacklog);

                return Ok(ApiResponse<MonitoringStatsDto>.SuccessResponse(stats, "Monitoring stats retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monitoring stats");
                return StatusCode(500, ApiResponse<MonitoringStatsDto>.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("sla-data")]
        public async Task<ActionResult<ApiResponse<List<SLADataDto>>>> GetSLAData([FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var slaData = await _monitoringService.GetSLADataAsync(targetDate);

                return Ok(ApiResponse<List<SLADataDto>>.SuccessResponse(slaData, "SLA data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SLA data");
                return StatusCode(500, ApiResponse<List<SLADataDto>>.ErrorResponse("Internal server error"));
            }
        }
    }
}