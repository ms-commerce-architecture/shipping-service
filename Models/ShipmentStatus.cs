namespace shipping_service_backend.Models
{
    public enum ShipmentStatus
    {
        Pending,
        Processing,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed,
        Returned,
    }
}
