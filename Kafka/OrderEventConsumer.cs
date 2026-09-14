using System.Text.Json;
using Confluent.Kafka;
using shipping_service_backend.Services;

namespace shipping_service_backend.Kafka
{
    public class OrderEventConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderEventConsumer> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OrderEventConsumer(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<OrderEventConsumer> logger)
        {
            _config = configuration;
            _scopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"] ?? "localhost:9092",
                GroupId = _config["Kafka:GroupId"] ?? "shipping-service",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            var topic = _config["Kafka:OrderCreatedTopic"] ?? "order-event";

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

          
            consumer.Subscribe(topic);
            _logger.LogInformation("Subscribed to Kafka topic: {Topic}", topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (result?.Message?.Value is null)
                        continue;

                    _logger.LogInformation(
                        "Received message at offset {Offset}: {Message}",
                        result.Offset, result.Message.Value);

                    await ProcessMessageAsync(result.Message.Value);

                   
                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
               
                    break;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                  
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error processing Kafka message");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
            }

            consumer.Close();
            _logger.LogInformation("Kafka consumer stopped.");
        }

        private async Task ProcessMessageAsync(string message)
        {
            OrderCreatedEvent? orderEvent;

            try
            {
                orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message, JsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize message: {Message}", message);
                return;
            }

            if (orderEvent is null
                || string.IsNullOrWhiteSpace(orderEvent.OrderNumber)
                || string.IsNullOrWhiteSpace(orderEvent.Email))
            {
                _logger.LogWarning("Invalid order event — missing fields: {Message}", message);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var shipmentService = scope.ServiceProvider.GetRequiredService<IShipmentService>();
            await shipmentService.CreateFromOrderEventAsync(orderEvent.OrderNumber, orderEvent.Email);
        }
    }
}