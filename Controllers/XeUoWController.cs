using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using DataAccess.Repositories;
using DataAccess.Context;
using Domain.Entities;

namespace duanminiveprogresql.Controllers
{
    /// <summary>
    /// DEMO Controller sử dụng Repository Pattern & Unit of Work
    /// Đây là example để refactor các controllers khác
    /// </summary>
    public class XeUoWController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<XeUoWController> _logger;

        public XeUoWController(IUnitOfWork unitOfWork, ILogger<XeUoWController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /XeUoW
        public async Task<IActionResult> Index(string? search, string? loaixe, string? hangxe)
        {
            try
            {
                IEnumerable<Xe> xes;

                // S? d?ng specific repository methods
                if (!string.IsNullOrEmpty(search))
                {
                    xes = await _unitOfWork.Xes.SearchCarsAsync(search);
                }
                else if (!string.IsNullOrEmpty(loaixe))
                {
                    xes = await _unitOfWork.Xes.GetCarsByTypeAsync(loaixe);
                }
                else if (!string.IsNullOrEmpty(hangxe))
                {
                    xes = await _unitOfWork.Xes.GetCarsByBrandAsync(hangxe);
                }
                else
                {
                    xes = await _unitOfWork.Xes.GetAvailableCarsAsync();
                }

                return View(xes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cars");
                TempData["ErrorMessage"] = "Có l?i x?y ra khi t?i danh sách xe";
                return View(new List<Xe>());
            }
        }

        // GET: /XeUoW/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(id);
                
                if (xe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm th?y xe";
                    return RedirectToAction(nameof(Index));
                }

                return View(xe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading car {id}");
                TempData["ErrorMessage"] = "Có l?i x?y ra";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /XeUoW/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /XeUoW/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Xe xe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(xe);
                }

                // S? d?ng transaction cho consistency
                await _unitOfWork.BeginTransactionAsync();

                // Thêm xe m?i
                await _unitOfWork.Xes.AddAsync(xe);
                
                // Save changes và commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Created new car: {xe.Tenxe}");
                TempData["SuccessMessage"] = "Thêm xe thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating car");
                ModelState.AddModelError("", "Có l?i x?y ra khi thêm xe");
                return View(xe);
            }
        }

        // GET: /XeUoW/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(id);
                
                if (xe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm th?y xe";
                    return RedirectToAction(nameof(Index));
                }

                return View(xe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading car {id} for edit");
                TempData["ErrorMessage"] = "Có l?i x?y ra";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /XeUoW/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Xe xe)
        {
            if (id != xe.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(xe);
                }

                await _unitOfWork.BeginTransactionAsync();

                _unitOfWork.Xes.Update(xe);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Updated car: {xe.Tenxe}");
                TempData["SuccessMessage"] = "C?p nh?t xe thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error updating car {id}");
                ModelState.AddModelError("", "Có l?i x?y ra khi c?p nh?t xe");
                return View(xe);
            }
        }

        // POST: /XeUoW/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(id);
                
                if (xe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm th?y xe";
                    return RedirectToAction(nameof(Index));
                }

                await _unitOfWork.BeginTransactionAsync();

                _unitOfWork.Xes.Remove(xe);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Deleted car: {xe.Tenxe}");
                TempData["SuccessMessage"] = "Xóa xe thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error deleting car {id}");
                TempData["ErrorMessage"] = "Có l?i x?y ra khi xóa xe";
                return RedirectToAction(nameof(Index));
            }
        }

        // Ki?m tra xe có available không
        public async Task<IActionResult> CheckAvailability(int xeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var isAvailable = await _unitOfWork.Xes.IsCarAvailableAsync(xeId, startDate, endDate);
                
                return Json(new
                {
                    success = true,
                    available = isAvailable,
                    message = isAvailable ? "Xe kh? d?ng" : "Xe ?ã ???c ??t trong th?i gian này"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking car availability");
                return Json(new
                {
                    success = false,
                    message = "Có l?i x?y ra"
                });
            }
        }

        // Example: Complex operation v?i multiple repositories
        public async Task<IActionResult> BookingStats(int xeId)
        {
            try
            {
                var xe = await _unitOfWork.Xes.GetByIdAsync(xeId);
                if (xe == null)
                {
                    return NotFound();
                }

                // L?y bookings c?a xe này
                var bookings = await _unitOfWork.DatXes.GetBookingsByCarAsync(xeId);
                
                // Tính th?ng kê
                var stats = new
                {
                    TenXe = xe.Tenxe,
                    TongSoLanThue = bookings.Count(),
                    SoLanHoanThanh = bookings.Count(b => b.Trangthai == "Completed"),
                    SoLanHuy = bookings.Count(b => b.Trangthai == "Cancelled"),
                    TongDoanhThu = bookings.Where(b => b.Trangthai == "Completed").Sum(b => b.Tongtien)
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stats for car {xeId}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
