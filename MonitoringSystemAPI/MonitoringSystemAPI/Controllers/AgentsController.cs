using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(IAgentService agentService, ILogger<AgentsController> logger)
        {
            _agentService = agentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AgentDto>>>> GetAllAgents()
        {
            try
            {
                var agents = await _agentService.GetAllAgentsAsync();
                return Ok(ApiResponse<List<AgentDto>>.SuccessResponse(agents, "Agents retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agents");
                return StatusCode(500, ApiResponse<List<AgentDto>>.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AgentDto>>> GetAgent(int id)
        {
            try
            {
                var agent = await _agentService.GetAgentByIdAsync(id);

                if (agent == null)
                {
                    return NotFound(ApiResponse<AgentDto>.ErrorResponse("Agent not found"));
                }

                return Ok(ApiResponse<AgentDto>.SuccessResponse(agent, "Agent retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agent");
                return StatusCode(500, ApiResponse<AgentDto>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAgentStatus(int id, [FromBody] string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Status is required"));
                }

                await _agentService.UpdateAgentStatusAsync(id, status);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Agent status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agent status");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPut("{id}/performance")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAgentPerformance(int id, [FromBody] UpdatePerformanceDto performanceData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid performance data"));
                }

                await _agentService.UpdateAgentPerformanceAsync(id, performanceData.Completed, performanceData.EstimatedHours, performanceData.ActualHours);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Agent performance updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agent performance");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }
    }
}