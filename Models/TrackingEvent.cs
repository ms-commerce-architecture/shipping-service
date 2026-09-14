namespace shipping_service_backend.Models
{
    public class TrackingEvent
    {
        public long EventId { get; set; }
        public long shipmentId { get; set; }
        public ShipmentStatus Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime OccurredAt { get; set; }
        public Shipment Shipment { get; set; } = null!;
    }
}
