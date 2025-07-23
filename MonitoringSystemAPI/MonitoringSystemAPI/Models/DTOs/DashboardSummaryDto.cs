namespace MonitoringAPI.Models
{
    public class DashboardSummaryDto
    {
        public MonitoringStatsDto MonitoringStats { get; set; }
        public List<SLADataDto> SLAData { get; set; }
        public List<AgentDto> Agents { get; set; }
        public List<ReviewerDto> Reviewers { get; set; }
        public UserStatsDto AgentStats { get; set; }
        public UserStatsDto ReviewerStats { get; set; }
    }
}