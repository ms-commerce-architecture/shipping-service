namespace shipping_service_backend.DTOs.Request
{
    public record UpdateShipmentStatusRequest(
        string Status,
        string? Location,
        string? Notes,
        DateTime? EstimatedDelivery
    );
}
