namespace shipping_service_backend.Kafka
{
    public class OrderCreatedEvent
    {
        public string OrderNumber { get; set; }= string.Empty;
        public string Email { get; set; }= string.Empty;
    }
}
