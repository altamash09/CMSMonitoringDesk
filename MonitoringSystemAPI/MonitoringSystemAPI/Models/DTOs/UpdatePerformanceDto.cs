namespace MonitoringAPI.Models
{
    // Update DTOs
    public class UpdatePerformanceDto
    {
        public int Completed { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
    }
}