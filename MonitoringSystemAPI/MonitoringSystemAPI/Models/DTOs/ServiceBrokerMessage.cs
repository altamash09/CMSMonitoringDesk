namespace MonitoringAPI.Models
{
    // SQL Service Broker Message Model
    public class ServiceBrokerMessage
    {
        public string MessageType { get; set; }
        public string MessageBody { get; set; }
        public DateTime Timestamp { get; set; }
    }
}