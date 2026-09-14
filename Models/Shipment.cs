using shipping_service_backend.Kafka;

namespace shipping_service_backend.Models
{
    public class Shipment
    {

        public long ShipmentId { get; set; }
        public string OrderNumber { get; set; } = String.Empty;
        public string CustomerEmail { get; set; } = String.Empty;
        public string TrackingNumber { get; set; } = String.Empty;
        public string Carrier { get; set; } = String.Empty;
        public ShipmentStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
    }
}
