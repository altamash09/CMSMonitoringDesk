using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewersController : ControllerBase
    {
        private readonly IReviewerService _reviewerService;
        private readonly ILogger<ReviewersController> _logger;

        public ReviewersController(IReviewerService reviewerService, ILogger<ReviewersController> logger)
        {
            _reviewerService = reviewerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ReviewerDto>>>> GetAllReviewers()
        {
            try
            {
                var reviewers = await _reviewerService.GetAllReviewersAsync();
                return Ok(ApiResponse<List<ReviewerDto>>.SuccessResponse(reviewers, "Reviewers retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviewers");
                return StatusCode(500, ApiResponse<List<ReviewerDto>>.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReviewerDto>>> GetReviewer(int id)
        {
            try
            {
                var reviewer = await _reviewerService.GetReviewerByIdAsync(id);

                if (reviewer == null)
                {
                    return NotFound(ApiResponse<ReviewerDto>.ErrorResponse("Reviewer not found"));
                }

                return Ok(ApiResponse<ReviewerDto>.SuccessResponse(reviewer, "Reviewer retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviewer");
                return StatusCode(500, ApiResponse<ReviewerDto>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateReviewerStatus(int id, [FromBody] string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Status is required"));
                }

                await _reviewerService.UpdateReviewerStatusAsync(id, status);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Reviewer status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reviewer status");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPut("{id}/performance")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateReviewerPerformance(int id, [FromBody] UpdatePerformanceDto performanceData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid performance data"));
                }

                await _reviewerService.UpdateReviewerPerformanceAsync(id, performanceData.Completed, performanceData.EstimatedHours, performanceData.ActualHours);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Reviewer performance updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reviewer performance");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }
    }
}