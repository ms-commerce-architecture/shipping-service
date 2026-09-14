namespace shipping_service_backend.DTOs.Response
{
    public record TrackingEventResponse(
     long EventId,
     string Status,
     string Location,
     string? Notes,
     DateTime OccurredAt
     );

}
