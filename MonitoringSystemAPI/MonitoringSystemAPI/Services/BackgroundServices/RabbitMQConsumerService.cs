using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.BackgroundServices
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQConsumerService(
            ILogger<RabbitMQConsumerService> logger,
            IServiceProvider serviceProvider,
            IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _connectionFactory = connectionFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Declare queues
                await _channel.QueueDeclareAsync(queue: "user_status_updates", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync(queue: "monitoring_updates", durable: true, exclusive: false, autoDelete: false);

                // Set up consumers
                await SetupUserStatusConsumer();
                await SetupMonitoringConsumer();

                _logger.LogInformation("RabbitMQ Consumer Service started successfully");

                // Keep the service running
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RabbitMQ Consumer Service");
            }
        }

        private async Task SetupUserStatusConsumer()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var userStatusMessage = JsonSerializer.Deserialize<UserStatusMessage>(message);

                    _logger.LogInformation($"Received user status update: {userStatusMessage.UserName} - {userStatusMessage.Status}");

                    // Process the message
                    using var scope = _serviceProvider.CreateScope();
                    var agentService = scope.ServiceProvider.GetRequiredService<IAgentService>();
                    var reviewerService = scope.ServiceProvider.GetRequiredService<IReviewerService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<ISignalRNotificationService>();

                    // Update user status in database
                    if (userStatusMessage.UserType.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                    {
                        await agentService.UpdateAgentStatusAsync(userStatusMessage.UserId, userStatusMessage.Status);
                    }
                    else if (userStatusMessage.UserType.Equals("Reviewer", StringComparison.OrdinalIgnoreCase))
                    {
                        await reviewerService.UpdateReviewerStatusAsync(userStatusMessage.UserId, userStatusMessage.Status);
                    }

                    // Send real-time update to clients
                    var statusUpdate = new UserStatusUpdateDto
                    {
                        UserId = userStatusMessage.UserId,
                        UserType = userStatusMessage.UserType,
                        Status = userStatusMessage.Status,
                        Timestamp = DateTime.UtcNow
                    };

                    await notificationService.SendUserStatusUpdate(statusUpdate);

                    // Acknowledge the message
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing user status message");
                    // Reject the message and don't requeue
                    await _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(queue: "user_status_updates", autoAck: false, consumer: consumer);
            _logger.LogInformation("User status consumer set up successfully");
        }

        private async Task SetupMonitoringConsumer()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var rabbitMQMessage = JsonSerializer.Deserialize<RabbitMQMessage>(message);

                    _logger.LogInformation($"Received monitoring message: {rabbitMQMessage.MessageType}");

                    using var scope = _serviceProvider.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<ISignalRNotificationService>();

                    switch (rabbitMQMessage.MessageType?.ToLower())
                    {
                        case "monitoring_update":
                            var monitoringData = JsonSerializer.Deserialize<MonitoringUpdateDto>(rabbitMQMessage.Data.ToString());
                            await notificationService.SendMonitoringUpdate(monitoringData);
                            break;

                        case "sla_update":
                            var slaData = JsonSerializer.Deserialize<SLAUpdateDto>(rabbitMQMessage.Data.ToString());
                            await notificationService.SendSLAUpdate(slaData);
                            break;

                        default:
                            _logger.LogWarning($"Unknown message type: {rabbitMQMessage.MessageType}");
                            break;
                    }

                    // Acknowledge the message
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing monitoring message");
                    // Reject the message and don't requeue
                    await _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(queue: "monitoring_updates", autoAck: false, consumer: consumer);
            _logger.LogInformation("Monitoring updates consumer set up successfully");
        }

        public override void Dispose()
        {
            try
            {
                _channel?.CloseAsync();
                _connection?.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RabbitMQ resources");
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
                base.Dispose();
            }
        }
    }
}