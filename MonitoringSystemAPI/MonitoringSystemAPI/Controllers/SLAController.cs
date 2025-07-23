using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SLAController : ControllerBase
    {
        private readonly ISLAService _slaService;
        private readonly ILogger<SLAController> _logger;

        public SLAController(ISLAService slaService, ILogger<SLAController> logger)
        {
            _slaService = slaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<SLADataDto>>>> GetSLAData([FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var slaData = await _slaService.GetSLADataAsync(targetDate);

                return Ok(ApiResponse<List<SLADataDto>>.SuccessResponse(slaData, "SLA data retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SLA data");
                return StatusCode(500, ApiResponse<List<SLADataDto>>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateSLARecord([FromBody] UpdateSLADto slaData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid SLA data"));
                }

                await _slaService.UpdateSLARecordAsync(slaData.Hour, slaData.Completed, slaData.Percentage, slaData.Date);
                return Ok(ApiResponse<object>.SuccessResponse(null, "SLA record updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SLA record");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("current-percentage")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetCurrentSLAPercentage()
        {
            try
            {
                var percentage = await _slaService.GetCurrentSLAPercentageAsync();
                return Ok(ApiResponse<decimal>.SuccessResponse(percentage, "Current SLA percentage retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current SLA percentage");
                return StatusCode(500, ApiResponse<decimal>.ErrorResponse("Internal server error"));
            }
        }
    }
}