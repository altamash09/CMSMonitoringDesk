namespace MonitoringAPI.Models
{
    // Real-time message DTOs for SignalR
    public class UserStatusUpdateDto
    {
        public int UserId { get; set; }
        public string UserType { get; set; } // Agent or Reviewer
        public string Status { get; set; } // online, idle, offline
        public DateTime Timestamp { get; set; }
    }
}