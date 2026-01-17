using Microsoft.AspNetCore.Mvc;
using duanminiveprogresql.Repositories;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Controllers.API
{
    /// <summary>
    /// DEMO API Controller s? d?ng Repository Pattern & Unit of Work
    /// ?ây là example ?? refactor các API controllers khác
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class XeUoWApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<XeUoWApiController> _logger;

        public XeUoWApiController(IUnitOfWork unitOfWork, ILogger<XeUoWApiController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// L?y t?t c? xe kh? d?ng
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Xe>>> GetAvailableCars()
        {
            try
            {
                var xes = await _unitOfWork.Xes.GetAvailableCarsAsync();
                return Ok(xes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available cars");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tìm ki?m xe
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Xe>>> SearchCars([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return BadRequest("Search term is required");
                }

                var xes = await _unitOfWork.Xes.SearchCarsAsync(searchTerm);
                return Ok(xes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cars");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// L?y xe theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Xe>> GetCar(int id)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(id);
                
                if (xe == null)
                {
                    return NotFound($"Xe ID {id} không t?n t?i");
                }

                return Ok(xe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting car {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Thêm xe m?i
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Xe>> CreateCar([FromBody] Xe xe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _unitOfWork.BeginTransactionAsync();
                
                var createdXe = await _unitOfWork.Xes.AddAsync(xe);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Created new car via API: {xe.Tenxe}");
                
                return CreatedAtAction(nameof(GetCar), new { id = createdXe.Id }, createdXe);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating car via API");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// C?p nh?t xe
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Xe xe)
        {
            try
            {
                if (id != xe.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingXe = await _unitOfWork.Xes.GetByIdAsync(id);
                if (existingXe == null)
                {
                    return NotFound($"Xe ID {id} không t?n t?i");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _unitOfWork.BeginTransactionAsync();
                
                _unitOfWork.Xes.Update(xe);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Updated car via API: {xe.Tenxe}");
                
                return NoContent();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error updating car {id} via API");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Xóa xe
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(id);
                if (xe == null)
                {
                    return NotFound($"Xe ID {id} không t?n t?i");
                }

                await _unitOfWork.BeginTransactionAsync();
                
                _unitOfWork.Xes.Remove(xe);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Deleted car via API: {xe.Tenxe}");
                
                return NoContent();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error deleting car {id} via API");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Ki?m tra availability
        /// </summary>
        [HttpGet("{id}/availability")]
        public async Task<ActionResult> CheckAvailability(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var isAvailable = await _unitOfWork.Xes.IsCarAvailableAsync(id, startDate, endDate);
                
                return Ok(new
                {
                    carId = id,
                    startDate,
                    endDate,
                    available = isAvailable,
                    message = isAvailable ? "Xe kh? d?ng" : "Xe ?ã ???c ??t trong th?i gian này"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking car availability via API");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Th?ng kê xe
        /// </summary>
        [HttpGet("{id}/stats")]
        public async Task<ActionResult> GetCarStats(int id)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(id);
                if (xe == null)
                {
                    return NotFound($"Xe ID {id} không t?n t?i");
                }

                // S? d?ng multiple repositories
                var bookings = await _unitOfWork.DatXes.GetBookingsByCarAsync(id);
                
                var stats = new
                {
                    CarInfo = new
                    {
                        xe.Id,
                        xe.Tenxe,
                        xe.Hangxe,
                        xe.Loaixe,
                        xe.Giathuetheongay,
                        xe.Trangthai
                    },
                    BookingStats = new
                    {
                        TotalBookings = bookings.Count(),
                        CompletedBookings = bookings.Count(b => b.Trangthai == "Completed"),
                        CancelledBookings = bookings.Count(b => b.Trangthai == "Cancelled"),
                        PendingBookings = bookings.Count(b => b.Trangthai == "Pending"),
                        TotalRevenue = bookings.Where(b => b.Trangthai == "Completed").Sum(b => b.Tongtien)
                    }
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stats for car {id} via API");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
