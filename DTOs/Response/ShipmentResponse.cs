namespace shipping_service_backend.DTOs.Response
{
    public record ShipmentResponse(
        long ShipmentId,
        string OrderNumber,
        string CustomerEmail,
        string TrackingNumber,
        string? Carrier,
        string Status,
        string? ShippingAddress,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        DateTime? EstimatedDelivery,
        DateTime? DeliveredAt,
        List<TrackingEventResponse> TrackingEvents
    );


    public record PagedResult<T>(
        IEnumerable<T> Content,
        int TotalElements,
        int TotalPages,
        int PageNumber,
        int PageSize
        );
}
