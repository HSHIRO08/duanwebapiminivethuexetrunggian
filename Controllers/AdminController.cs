using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

namespace duanminivepropgsql.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Middleware ki?m tra role Admin
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này!";
                return RedirectToAction("Index", "Home");
            }

            // Th?ng kê t?ng quan
            ViewBag.TotalCars = await _context.Xes.CountAsync();
            ViewBag.AvailableCars = await _context.Xes.CountAsync(x => x.Trangthai == "Available");
            ViewBag.TotalBookings = await _context.Datxes.CountAsync();
            ViewBag.PendingBookings = await _context.Datxes.CountAsync(d => d.Trangthai == "Pending");
            ViewBag.TotalCustomers = await _context.Khachhangs.CountAsync();
            ViewBag.TotalRevenue = await _context.Thanhtoans
                .Where(t => t.Trangthai == "Completed")
                .SumAsync(t => (decimal?)t.Sotien) ?? 0;

            // Ðon d?t xe g?n dây
            var recentBookings = await _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .OrderByDescending(d => d.Ngaydat)
                .Take(10)
                .ToListAsync();

            return View(recentBookings);
        }

        // ========== QU?N LÝ XE ==========

        // Danh sách xe
        public async Task<IActionResult> Xe()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var cars = await _context.Xes.OrderByDescending(x => x.Ngaytao).ToListAsync();
            return View(cars);
        }

        // Thêm xe - GET
        public IActionResult CreateXe()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        // Thêm xe - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateXe(Xe xe)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                xe.Ngaytao = DateTime.Now;  
                xe.Trangthai = "Available";

                _context.Xes.Add(xe);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm xe thành công!";
                return RedirectToAction("Xe");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating car");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(xe);
            }
        }

        // S?a xe - GET
        public async Task<IActionResult> EditXe(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var xe = await _context.Xes.FindAsync(id);
            if (xe == null) return NotFound();

            return View(xe);
        }

        // S?a xe - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditXe(int id, Xe xe)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id != xe.Id) return BadRequest();

            try
            {
                xe.Ngaycapnhat = DateTime.Now;  // ? S?A: DateTime.Now
                _context.Update(xe);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật xe thành công!";
                return RedirectToAction("Xe");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(xe);
            }
        }

        // Xóa xe
        public async Task<IActionResult> DeleteXe(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var xe = await _context.Xes.FindAsync(id);
                if (xe != null)
                {
                    _context.Xes.Remove(xe);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa xe thành công!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car");
                TempData["ErrorMessage"] = "Không thể xóa xe này!";
            }

            return RedirectToAction("Xe");
        }

        // ========== QU?N LÝ ÐON THUÊ ==========

        public async Task<IActionResult> DonThue(string status = "All")
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var query = _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .AsQueryable();

            if (status != "All")
            {
                query = query.Where(d => d.Trangthai == status);
            }

            var bookings = await query.OrderByDescending(d => d.Ngaydat).ToListAsync();
            ViewBag.CurrentStatus = status;

            return View(bookings);
        }

        // Xác nh?n don
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var booking = await _context.Datxes.FindAsync(id);
                if (booking != null)
                {
                    booking.Trangthai = "Confirmed";
                    booking.Ngayxacnhan = DateTime.Now;  // ? S?A: DateTime.Now
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ðã xác nhận đơn thuê!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming booking");
                TempData["ErrorMessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("DonThue");
        }

        // H?y don
        public async Task<IActionResult> CancelBooking(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var booking = await _context.Datxes.FindAsync(id);
                if (booking != null)
                {
                    booking.Trangthai = "Cancelled";
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ðã hủy đơn thuê!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking");
                TempData["ErrorMessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("DonThue");
        }

        // ========== QU?N LÝ KHÁCH HÀNG ==========

        public async Task<IActionResult> KhachHang()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var customers = await _context.Khachhangs
                .Include(k => k.Nguoidung)
                .OrderByDescending(k => k.Ngaydangky)
                .ToListAsync();

            return View(customers);
        }

        // Xác th?c khách hàng
        public async Task<IActionResult> VerifyCustomer(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var customer = await _context.Khachhangs.FindAsync(id);
                if (customer != null)
                {
                    customer.Daxacthuc = true;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ðã xác thực khách hàng!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying customer");
                TempData["ErrorMessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("KhachHang");
        }

        // ========== BÁO CÁO & TH?NG KÊ ==========

        public async Task<IActionResult> BaoCao()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var today = DateTime.Now.Date;  // ? S?A: DateTime.Now
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            // Th?ng kê theo tháng
            ViewBag.BookingsThisMonth = await _context.Datxes
                .CountAsync(d => d.Ngaydat >= thisMonth);
            ViewBag.BookingsLastMonth = await _context.Datxes
                .CountAsync(d => d.Ngaydat >= lastMonth && d.Ngaydat < thisMonth);

            ViewBag.RevenueThisMonth = await _context.Thanhtoans
                .Where(t => t.Ngaythanhtoan >= thisMonth && t.Trangthai == "Completed")
                .SumAsync(t => (decimal?)t.Sotien) ?? 0;

            ViewBag.RevenueLastMonth = await _context.Thanhtoans
                .Where(t => t.Ngaythanhtoan >= lastMonth && t.Ngaythanhtoan < thisMonth && t.Trangthai == "Completed")
                .SumAsync(t => (decimal?)t.Sotien) ?? 0;

            // Top xe du?c thuê nhi?u
            var topCars = await _context.Lichsuthues
                .GroupBy(l => l.Xeid)
                .Select(g => new
                {
                    XeId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var topCarsDetails = new List<dynamic>();
            foreach (var item in topCars)
            {
                var xe = await _context.Xes.FindAsync(item.XeId);
                topCarsDetails.Add(new { Xe = xe, Count = item.Count });
            }

            ViewBag.TopCars = topCarsDetails;

            return View();
        }

        // ========== H? TR? KHÁCH HÀNG ==========

        public async Task<IActionResult> HoTro(string status = "All")
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var query = _context.Hotrokhachhangs
                .Include(h => h.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .AsQueryable();

            if (status != "All")
            {
                query = query.Where(h => h.Trangthai == status);
            }

            var tickets = await query.OrderByDescending(h => h.Ngaytao).ToListAsync();
            ViewBag.CurrentStatus = status;

            return View(tickets);
        }

        // Tr? l?i h? tr?
        [HttpPost]
        public async Task<IActionResult> ReplySupport(int id, string reply)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var ticket = await _context.Hotrokhachhangs.FindAsync(id);
                if (ticket != null)
                {
                    var userId = HttpContext.Session.GetInt32("UserId");
                    ticket.Traloi = reply;
                    ticket.Nhanvienxulyid = userId;
                    ticket.Trangthai = "Resolved";
                    ticket.Ngaygiaiquyet = DateTime.Now;  // ? S?A: DateTime.Now
                    ticket.Ngaycapnhat = DateTime.Now;    // ? S?A: DateTime.Now

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ðã trả lời hỗ trợ!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error replying support");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction("HoTro");
        }
    }
}
