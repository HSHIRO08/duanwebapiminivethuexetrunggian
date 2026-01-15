using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Controllers
{
    public class DatXeController : Controller
    {
        private readonly AppDbContext _context;

        public DatXeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trang đặt xe
        public async Task<IActionResult> Create(int xeId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Create", "DatXe", new { xeId }) });
            }

            var xe = await _context.Xes.FindAsync(xeId);
            if (xe == null || xe.Trangthai != "Available")
            {
                TempData["ErrorMessage"] = "Xe không tồn tại hoặc không khả dụng";
                return RedirectToAction("Index", "Xe");
            }

            ViewBag.Xe = xe;
            return View();
        }

        // POST: Đặt xe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int xeId, DateTime ngaybatdau, DateTime ngayketthuc, string diadiemnhan, string diadiemtra, string ghichu)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng";
                return RedirectToAction("Index", "Home");
            }

            var xe = await _context.Xes.FindAsync(xeId);
            if (xe == null || xe.Trangthai != "Available")
            {
                TempData["ErrorMessage"] = "Xe không khả dụng";
                return RedirectToAction("Index", "Xe");
            }

            // Tính số ngày thuê
            var songaythue = (int)(ngayketthuc - ngaybatdau).TotalDays;
            if (songaythue <= 0)
            {
                ModelState.AddModelError("", "Ngày kết thúc phải sau ngày bắt đầu");
                ViewBag.Xe = xe;
                return View();
            }

            // Tính tổng tiền
            var tongtien = xe.Giathuetheongay * songaythue;

            // Tạo đơn đặt xe - SỬA: DateTime.Now thay vì DateTime.UtcNow
            var datxe = new Datxe
            {
                Khachhangid = khachhang.Id,
                Xeid = xeId,
                Ngaybatdau = ngaybatdau,
                Ngayketthuc = ngayketthuc,
                Songaythue = songaythue,
                Giatheongay = xe.Giathuetheongay,
                Tongtien = tongtien,
                Diadiemnhan = diadiemnhan,
                Diadiemtra = diadiemtra,
                Ghichu = ghichu,
                Trangthai = "Pending",
                Ngaydat = DateTime.Now  // ✅ SỬA: DateTime.Now
            };

            _context.Datxes.Add(datxe);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt xe thành công! Vui lòng chờ xác nhận.";
            return RedirectToAction("Details", new { id = datxe.Id });
        }

        // Chi tiết đơn đặt xe
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var datxe = await _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(d => d.Thanhtoans)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (datxe == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền xem
            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null || datxe.Khachhangid != khachhang.Id)
            {
                return Forbid();
            }

            return View(datxe);
        }

        // Hủy đơn đặt xe
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            var datxe = await _context.Datxes.FindAsync(id);

            if (datxe == null || datxe.Khachhangid != khachhang.Id)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn đặt xe" });
            }

            if (datxe.Trangthai != "Pending" && datxe.Trangthai != "Confirmed")
            {
                return Json(new { success = false, message = "Không thể hủy đơn ở trạng thái này" });
            }

            datxe.Trangthai = "Cancelled";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Hủy đơn thành công" });
        }
    }
}
