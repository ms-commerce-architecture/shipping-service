using Riok.Mapperly.Abstractions;
using shipping_service_backend.DTOs.Response;
using shipping_service_backend.Models;

namespace shipping_service_backend.Mapper;

    [Mapper]
    public partial class TrackingEventMapper
    {
        public  partial TrackingEventResponse ToResponse(TrackingEvent trackingEvent);

}

