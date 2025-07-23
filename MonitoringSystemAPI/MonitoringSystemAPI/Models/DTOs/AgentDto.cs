namespace MonitoringAPI.Models
{
    public class AgentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int Completed { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
        public string Rank { get; set; }
    }
}