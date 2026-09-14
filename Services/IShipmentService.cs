
using shipping_service_backend.DTOs.Request;
using shipping_service_backend.DTOs.Response;


namespace shipping_service_backend.Services
{
    public interface IShipmentService
    {
        Task<PagedResult<ShipmentResponse>> GetAllAsync(int pageNumber, int pageSize);

        Task<ShipmentResponse> GetByIdAsync(long id);
        Task<ShipmentResponse> GetByOrderNumberAsync(string orderNumber);
        Task<List<ShipmentResponse>> GetTrackingEventsAsync(long shipmentId);
        Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request);

        Task<ShipmentResponse> UpdateAsync(long id, UpdateShipmentStatusRequest request);
        Task DeleteAsync(long shipmentId);

        Task CreateFromOrderEventAsync(string orderNumber,string customerEmail);
    }
}
