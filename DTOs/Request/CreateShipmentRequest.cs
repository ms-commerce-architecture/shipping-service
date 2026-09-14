namespace shipping_service_backend.DTOs.Request
{
    public record CreateShipmentRequest
    (
        string OrderNumber,
        string CustomerEmail,
        string? Carrier,
        string? ShippingAddress,
        DateTime? EstimatedDelivery
    );
}
