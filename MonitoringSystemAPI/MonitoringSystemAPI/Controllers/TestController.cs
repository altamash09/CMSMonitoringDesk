using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Models;

namespace MonitoringSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "API is running!",
                timestamp = DateTime.UtcNow,
                status = "success"
            });
        }

        [HttpGet("auth-test")]
        [Authorize]
        public IActionResult AuthTest()
        {
            var username = User.Identity?.Name ?? "Unknown";
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(new
            {
                message = "Authentication working!",
                username = username,
                claims = claims,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpPost("sample-data")]
        public IActionResult GetSampleData()
        {
            var sampleDashboard = new
            {
                monitoringStats = new
                {
                    stats = new Dictionary<string, object>
                    {
                        ["Un-Monitored"] = new { count = 45, color = "from-red-100 to-red-200", mandatory = true },
                        ["Monitoring In Process"] = new { count = 23, color = "from-amber-100 to-yellow-200", mandatory = true },
                        ["Live"] = new { count = 156, color = "from-emerald-100 to-green-200", mandatory = true },
                        ["Review In Process"] = new { count = 34, color = "from-blue-100 to-blue-200", mandatory = true }
                    }
                },
                slaData = Enumerable.Range(0, 24).Select(h => new
                {
                    hour = h,
                    completed = Random.Shared.Next(10, 55),
                    percentage = Random.Shared.Next(75, 98)
                }).ToList(),
                agents = new[]
                {
                    new { id = 1, name = "John Smith", status = "online", completed = 45, estimatedHours = 8.5, actualHours = 7.2, rank = "Diamond" },
                    new { id = 2, name = "Sarah Johnson", status = "online", completed = 52, estimatedHours = 9.0, actualHours = 8.1, rank = "Platinum" }
                },
                reviewers = new[]
                {
                    new { id = 1, name = "Robert Wilson", status = "online", completed = 28, estimatedHours = 6.5, actualHours = 5.8, rank = "Diamond" },
                    new { id = 2, name = "Jennifer Lee", status = "online", completed = 32, estimatedHours = 7.0, actualHours = 6.2, rank = "Platinum" }
                },
                agentStats = new { total = 2, online = 2, idle = 0, offline = 0 },
                reviewerStats = new { total = 2, online = 2, idle = 0, offline = 0 }
            };

            return Ok(ApiResponse<object>.SuccessResponse(sampleDashboard, "Sample data retrieved"));
        }
    }
}