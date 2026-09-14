using Riok.Mapperly.Abstractions;
using shipping_service_backend.DTOs.Request;
using shipping_service_backend.DTOs.Response;
using shipping_service_backend.Kafka;
using shipping_service_backend.Models;

namespace shipping_service_backend.Mapper;



[Mapper]
public partial class ShipmentMapper
{
    // Collection element mapper — Mapperly auto-uses this for TrackingEvents
    public partial TrackingEventResponse ToTrackingEventResponse(TrackingEvent trackingEvent);

    [MapProperty(nameof(Shipment.TrackingEvents), nameof(ShipmentResponse.TrackingEvents))]
    public partial ShipmentResponse ToResponse(Shipment shipment);

    // Status update — only touch what the request owns
    [MapperIgnoreTarget(nameof(Shipment.ShipmentId))]
    [MapperIgnoreTarget(nameof(Shipment.OrderNumber))]
    [MapperIgnoreTarget(nameof(Shipment.CustomerEmail))]
    [MapperIgnoreTarget(nameof(Shipment.TrackingNumber))]
    [MapperIgnoreTarget(nameof(Shipment.ShippingAddress))]
    [MapperIgnoreTarget(nameof(Shipment.Carrier))]
    [MapperIgnoreTarget(nameof(Shipment.CreatedAt))]
    [MapperIgnoreTarget(nameof(Shipment.DeliveredAt))]
    [MapperIgnoreTarget(nameof(Shipment.TrackingEvents))]
    public partial void UpdateShipmentStatus(
        UpdateShipmentStatusRequest request, Shipment shipment);

    [MapperIgnoreTarget(nameof(Shipment.ShipmentId))]
    [MapperIgnoreTarget(nameof(Shipment.CreatedAt))]
    [MapperIgnoreTarget(nameof(Shipment.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Shipment.TrackingEvents))]
    [MapperIgnoreTarget(nameof(Shipment.TrackingNumber))]
    [MapperIgnoreTarget(nameof(Shipment.Status))]
    public partial Shipment ToEntity(CreateShipmentRequest request);

   



}