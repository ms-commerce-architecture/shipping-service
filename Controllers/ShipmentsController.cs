using Microsoft.AspNetCore.Mvc;
using shipping_service_backend.DTOs.Request;
using shipping_service_backend.DTOs.Response;
using shipping_service_backend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace shipping_service_backend.Controllers
{
    [Route("api/Shipments")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly ILogger<ShipmentsController> _logger;

        public ShipmentsController(IShipmentService shipmentService, ILogger<ShipmentsController> logger)
        {
            _shipmentService = shipmentService;
            _logger = logger;
        }

        // GET: api/Shipments
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ShipmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 0, [FromQuery] int size = 20)
        {
            // Journalisation structurée (pas d'interpolation de chaîne '$')
            _logger.LogInformation("Fetching shipments page {Page} with size {Size}.", page, size);

            try
            {
                var result = await _shipmentService.GetAllAsync(page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch shipments for page {Page} and size {Size}.", page, size);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // GET api/Shipments/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentByid(long id)
        {
            _logger.LogInformation("Fetching shipment with ID {ShipmentId}.", id);

            try
            {
                var result = await _shipmentService.GetByIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning("Shipment with ID {ShipmentId} was not found.", id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving shipment {ShipmentId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // POST api/Shipments
        [HttpPost]
        [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request)
        {
            _logger.LogInformation("Initiating creation of a new shipment.");

            try
            {
                var shipment = await _shipmentService.CreateAsync(request);

                _logger.LogInformation("Shipment successfully created with ID {ShipmentId}.", shipment.ShipmentId);

                return CreatedAtAction(
                    nameof(GetShipmentByid),
                    new { id = shipment.ShipmentId },
                    shipment);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data provided for shipment creation.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create shipment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // PUT api/Shipments/5
        [HttpPut("{shipmentId}")]
        [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShipment(long shipmentId, [FromBody] UpdateShipmentStatusRequest request)
        {
            _logger.LogInformation("Updating shipment {ShipmentId}.", shipmentId);

            try
            {
                var shipment = await _shipmentService.UpdateAsync(shipmentId, request);

                _logger.LogInformation("Shipment {ShipmentId} successfully updated.", shipmentId);
                return Ok(shipment);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Attempted to update non-existent shipment {ShipmentId}.", shipmentId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating shipment {ShipmentId}.", shipmentId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // DELETE api/Shipments/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogInformation("Request received to delete shipment {ShipmentId}.", id);

            try
            {
                await _shipmentService.DeleteAsync(id);

                _logger.LogInformation("Shipment {ShipmentId} successfully deleted.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Attempted to delete non-existent shipment {ShipmentId}.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting shipment {ShipmentId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
