
using Microsoft.EntityFrameworkCore;
using shipping_service_backend.Data;
using shipping_service_backend.DTOs.Request;
using shipping_service_backend.DTOs.Response;
using shipping_service_backend.Mapper;
using shipping_service_backend.Models;

namespace shipping_service_backend.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _db;
        private readonly ShipmentMapper _shipmentMapper;
        private readonly TrackingEventMapper _trackingEventMapper;
        private readonly ILogger<ShipmentService> _logger;

        public ShipmentService(
            AppDbContext db,
            ShipmentMapper shipmentMapper,
            TrackingEventMapper trackingEventMapper,
            ILogger<ShipmentService> logger)
        {
            _db = db;
            _shipmentMapper = shipmentMapper;
            _trackingEventMapper = trackingEventMapper;
            _logger = logger;
        }

        public async Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request)
        {
            _logger.LogInformation("Attempting to create shipment for OrderNumber: {OrderNumber}.", request.OrderNumber);

            var alreadyExists = await _db.Shipments.AnyAsync(s => s.OrderNumber == request.OrderNumber);
            if (alreadyExists)
            {
                _logger.LogWarning("Creation failed. Shipment with OrderNumber {OrderNumber} already exists.", request.OrderNumber);
                throw new InvalidOperationException($"Shipment for order {request.OrderNumber} already exists.");
            }

            var shipment = _shipmentMapper.ToEntity(request);
            shipment.TrackingNumber = GenerateTrackingNumber();
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully created shipment ID {ShipmentId} for OrderNumber {OrderNumber}.", shipment.ShipmentId, request.OrderNumber);
            return _shipmentMapper.ToResponse(shipment);
        }

        public async Task DeleteAsync(long shipmentId)
        {
            _logger.LogInformation("Attempting to delete shipment ID {ShipmentId}.", shipmentId);

            var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);
            if (shipment == null)
            {
                _logger.LogWarning("Delete failed. Shipment ID {ShipmentId} not found.", shipmentId);
                throw new KeyNotFoundException($"Shipment with ID {shipmentId} was not found.");
            }

            if (shipment.Status != ShipmentStatus.Pending)
            {
                _logger.LogWarning("Delete failed. Shipment ID {ShipmentId} is in '{Status}' status. Only 'Pending' shipments can be deleted.",
                    shipmentId, shipment.Status);
                throw new InvalidOperationException("Cannot delete a shipment that is already processed.");
            }

            _db.Shipments.Remove(shipment);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted shipment ID {ShipmentId}.", shipmentId);
        }

        public async Task<PagedResult<ShipmentResponse>> GetAllAsync(int page, int size)
        {
            size = Math.Clamp(size, 1, 100);
            page = Math.Max(page, 0);

            _logger.LogDebug("Querying database for shipments. Page: {Page}, Size: {Size}.", page, size);

            var query = _db.Shipments.Include(s => s.TrackingEvents).AsNoTracking();

            var totalElements = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalElements / (double)size);

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} shipments out of {TotalElements} total elements.", items.Count, totalElements);

            return new PagedResult<ShipmentResponse>(
                items.Select(_shipmentMapper.ToResponse),
                totalElements,
                totalPages,
                page,
                size
            );
        }

        public async Task<ShipmentResponse> GetByIdAsync(long shipmentId)
        {
            _logger.LogDebug("Fetching shipment ID {ShipmentId} from database.", shipmentId);

            var shipment = await _db.Shipments
                .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

            if (shipment == null)
            {
                _logger.LogWarning("Shipment ID {ShipmentId} was not found in the database.", shipmentId);
                throw new KeyNotFoundException($"Shipment {shipmentId} not found.");
            }

            return _shipmentMapper.ToResponse(shipment);
        }

        public async Task<ShipmentResponse> UpdateAsync(long id, UpdateShipmentStatusRequest request)
        {
            _logger.LogInformation("Attempting to update status for shipment ID {ShipmentId}.", id);

            var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.ShipmentId == id);
            if (shipment == null)
            {
                _logger.LogWarning("Update failed. Shipment ID {ShipmentId} not found.", id);
                throw new KeyNotFoundException($"Shipment {id} not found.");
            }

            var oldStatus = shipment.Status;
            _shipmentMapper.UpdateShipmentStatus(request, shipment);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully updated shipment ID {ShipmentId} status from '{OldStatus}' to '{NewStatus}'.",
                id, oldStatus, shipment.Status);

            return _shipmentMapper.ToResponse(shipment);
        }

        public async Task CreateFromOrderEventAsync(string orderNumber, string customerEmail)
        {
            var alreadyExists = await _db.Shipments.AnyAsync(s => s.OrderNumber == orderNumber);
            if (alreadyExists)
            {
                _logger.LogWarning(
             "Skipping duplicate Kafka event — shipment for order {OrderNumber} already exists.",
             orderNumber);
                return;
            }
            var shipment = new Shipment
            {
                OrderNumber = orderNumber,
                CustomerEmail = customerEmail,
                TrackingNumber = GenerateTrackingNumber(),
                Status = ShipmentStatus.Pending,
                TrackingEvents = new List<TrackingEvent>()
            };

                _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Shipment {TrackingNumber} auto-created from Kafka event for order {OrderNumber}",
                shipment.TrackingNumber, orderNumber);
        }

        public Task<ShipmentResponse> GetByOrderNumberAsync(string orderNumber)
        {
            _logger.LogWarning("Method {MethodName} is not implemented yet.", nameof(GetByOrderNumberAsync));
            throw new NotImplementedException();
        }

        public Task<List<ShipmentResponse>> GetTrackingEventsAsync(long shipmentId)
        {
            _logger.LogWarning("Method {MethodName} is not implemented yet.", nameof(GetTrackingEventsAsync));
            throw new NotImplementedException();
        }

        private static string GenerateTrackingNumber() =>
        "SHIP-" + Guid.NewGuid().ToString("N").ToUpper()[..12];
    }
}
