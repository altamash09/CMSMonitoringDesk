namespace MonitoringAPI.Models
{
    // RabbitMQ Message Models
    public class RabbitMQMessage
    {
        public string MessageType { get; set; }
        public object Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}