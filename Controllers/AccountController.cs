using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Trang cá nhân
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var nguoidung = await _context.Nguoidungs
                .Include(n => n.Khachhangs)
                .FirstOrDefaultAsync(n => n.Id == userId.Value);

            if (nguoidung == null)
            {
                return NotFound();
            }

            return View(nguoidung);
        }

        // Cập nhật thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string hoten, string sodienthoai, string diachi, 
            string cmnd, string banglai, DateOnly? ngaysinh, string gioitinh, string diachichitiet)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var nguoidung = await _context.Nguoidungs
                .Include(n => n.Khachhangs)
                .FirstOrDefaultAsync(n => n.Id == userId.Value);

            if (nguoidung == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            nguoidung.Hoten = hoten;
            nguoidung.Sodienthoai = sodienthoai;
            nguoidung.Diachi = diachi;

            // Cập nhật thông tin khách hàng
            var khachhang = nguoidung.Khachhangs.FirstOrDefault();
            if (khachhang != null)
            {
                khachhang.Cmnd = cmnd;
                khachhang.Banglai = banglai;
                khachhang.Ngaysinh = ngaysinh;
                khachhang.Gioitinh = gioitinh;
                khachhang.Diachichitiet = diachichitiet;
            }

            await _context.SaveChangesAsync();

            // Cập nhật session
            HttpContext.Session.SetString("UserName", nguoidung.Hoten);

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        // Lịch sử đặt xe
        public async Task<IActionResult> BookingHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null)
            {
                return NotFound();
            }

            var bookings = await _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Thanhtoans)
                .Where(d => d.Khachhangid == khachhang.Id)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();

            return View(bookings);
        }

        // Lịch sử thuê xe (đã hoàn thành)
        public async Task<IActionResult> RentalHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null)
            {
                return NotFound();
            }

            var rentalHistory = await _context.Lichsuthues
                .Include(l => l.Xe)
                .Include(l => l.Datxe)
                .Where(l => l.Khachhangid == khachhang.Id)
                .OrderByDescending(l => l.Ngaytraxe)
                .ToListAsync();

            return View(rentalHistory);
        }

        // Đánh giá xe
        [HttpPost]
        public async Task<IActionResult> RateRental(int lichsuId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            var lichsu = await _context.Lichsuthues.FindAsync(lichsuId);

            if (lichsu == null || lichsu.Khachhangid != khachhang.Id)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch sử thuê xe" });
            }

            lichsu.Danhgia = rating;
            lichsu.Nhanxet = comment;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cảm ơn bạn đã đánh giá!" });
        }

        // Hỗ trợ khách hàng
        public async Task<IActionResult> Support()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null)
            {
                return NotFound();
            }

            var supportTickets = await _context.Hotrokhachhangs
                .Where(h => h.Khachhangid == khachhang.Id)
                .OrderByDescending(h => h.Ngaytao)
                .ToListAsync();

            return View(supportTickets);
        }

        // Tạo yêu cầu hỗ trợ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupport(string tieude, string noidung, string loaiyeucau)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null)
            {
                return NotFound();
            }

            var support = new Hotrokhachhang
            {
                Khachhangid = khachhang.Id,
                Tieude = tieude,
                Noidung = noidung,
                Loaiyeucau = loaiyeucau,
                Trangthai = "Open",
                Mucdouutien = "Normal",
                Ngaytao = DateTime.UtcNow
            };

            _context.Hotrokhachhangs.Add(support);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yêu cầu hỗ trợ đã được gửi!";
            return RedirectToAction("Support");
        }
    }
}
