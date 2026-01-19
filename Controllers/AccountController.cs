using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace duanminivepropgsql.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Helper: Check if user is logged in
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // Helper: Get current user ID
        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // Profile Page
        public async Task<IActionResult> Profile()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            var user = await _context.Nguoidungs
                .Include(u => u.Khachhangs)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng";
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        // Update Profile - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string hoten, string sodienthoai, string diachi,
            string cmnd, string banglai, DateOnly? ngaysinh, string gioitinh)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Nguoidungs
                    .Include(u => u.Khachhangs)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng";
                    return RedirectToAction("Profile");
                }

                // Update user info
                user.Hoten = hoten;
                user.Sodienthoai = sodienthoai;
                user.Diachi = diachi;

                // Update or create customer info
                var khachhang = user.Khachhangs.FirstOrDefault();
                if (khachhang != null)
                {
                    khachhang.Cmnd = cmnd;
                    khachhang.Banglai = banglai;
                    khachhang.Ngaysinh = ngaysinh;
                    khachhang.Gioitinh = gioitinh;
                }
                else if (!string.IsNullOrEmpty(cmnd) || !string.IsNullOrEmpty(banglai))
                {
                    // Create new customer record if additional info is provided
                    var newKhachhang = new Khachhang
                    {
                        Nguoidungid = user.Id,
                        Cmnd = cmnd,
                        Banglai = banglai,
                        Ngaysinh = ngaysinh,
                        Gioitinh = gioitinh,
                        Daxacthuc = false,
                        Ngaydangky = DateTime.Now
                    };
                    _context.Khachhangs.Add(newKhachhang);
                }

                await _context.SaveChangesAsync();

                // Update session
                HttpContext.Session.SetString("UserName", user.Hoten);

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin";
                return RedirectToAction("Profile");
            }
        }

        // Booking History
        public async Task<IActionResult> BookingHistory()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(k => k.Nguoidungid == userId);

            if (khachhang == null)
            {
                TempData["InfoMessage"] = "Bạn chưa có thông tin khách hàng. Vui lòng cập nhật trong trang cá nhân.";
                return View(new List<Datxe>());
            }

            var bookings = await _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                .Where(d => d.Khachhangid == khachhang.Id)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();

            return View(bookings);
        }

        // Rental History
        public async Task<IActionResult> RentalHistory()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(k => k.Nguoidungid == userId);

            if (khachhang == null)
            {
                TempData["InfoMessage"] = "Bạn chưa có thông tin khách hàng. Vui lòng cập nhật trong trang cá nhân.";
                return View(new List<Lichsuthue>());
            }

            var rentalHistory = await _context.Lichsuthues
                .Include(l => l.Xe)
                .Include(l => l.Khachhang)
                .Where(l => l.Khachhangid == khachhang.Id)
                .OrderByDescending(l => l.Ngaynhanxe)
                .ToListAsync();

            return View(rentalHistory);
        }

        // Support Page
        public async Task<IActionResult> Support()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetCurrentUserId();
            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(k => k.Nguoidungid == userId);

            if (khachhang == null)
            {
                TempData["InfoMessage"] = "Bạn chưa có thông tin khách hàng. Vui lòng cập nhật trong trang cá nhân.";
                return View(new List<Hotrokhachhang>());
            }

            var supportRequests = await _context.Hotrokhachhangs
                .Include(h => h.Nhanvienxuly)
                .Where(h => h.Khachhangid == khachhang.Id)
                .OrderByDescending(h => h.Ngaytao)
                .ToListAsync();

            return View(supportRequests);
        }

        // Create Support Request - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupport(string loaiyeucau, string tieude, string noidung)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var userId = GetCurrentUserId();
                var khachhang = await _context.Khachhangs
                    .FirstOrDefaultAsync(k => k.Nguoidungid == userId);

                if (khachhang == null)
                {
                    TempData["ErrorMessage"] = "Bạn chưa có thông tin khách hàng. Vui lòng cập nhật trong trang cá nhân.";
                    return RedirectToAction("Profile");
                }

                var supportRequest = new Hotrokhachhang
                {
                    Khachhangid = khachhang.Id,
                    Loaiyeucau = loaiyeucau,
                    Tieude = tieude,
                    Noidung = noidung,
                    Ngaytao = DateTime.Now,
                    Trangthai = "Chờ xử lý"
                };

                _context.Hotrokhachhangs.Add(supportRequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Gửi yêu cầu hỗ trợ thành công!";
                return RedirectToAction("Support");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating support request");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi yêu cầu hỗ trợ";
                return RedirectToAction("Support");
            }
        }
    }
}
