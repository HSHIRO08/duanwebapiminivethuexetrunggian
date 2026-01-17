using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

namespace duanminiveprogresql.Controllers
{
    public class DatXeController : Controller
    {
        private readonly AppDbContext _context;

        public DatXeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trang d?t xe
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
                TempData["ErrorMessage"] = "Xe không t?n t?i ho?c không kh? d?ng";
                return RedirectToAction("Index", "Xe");
            }

            ViewBag.Xe = xe;
            return View();
        }

        // POST: Ð?t xe
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
                TempData["ErrorMessage"] = "Không tìm th?y thông tin khách hàng";
                return RedirectToAction("Index", "Home");
            }

            var xe = await _context.Xes.FindAsync(xeId);
            if (xe == null || xe.Trangthai != "Available")
            {
                TempData["ErrorMessage"] = "Xe không kh? d?ng";
                return RedirectToAction("Index", "Xe");
            }

            // Tính s? ngày thuê
            var songaythue = (int)(ngayketthuc - ngaybatdau).TotalDays;
            if (songaythue <= 0)
            {
                ModelState.AddModelError("", "Ngày k?t thúc ph?i sau ngày b?t d?u");
                ViewBag.Xe = xe;
                return View();
            }

            // Tính t?ng ti?n
            var tongtien = xe.Giathuetheongay * songaythue;

            // T?o don d?t xe - S?A: DateTime.Now thay vì DateTime.UtcNow
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
                Ngaydat = DateTime.Now  // ? S?A: DateTime.Now
            };

            _context.Datxes.Add(datxe);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ð?t xe thành công! Vui lòng ch? xác nh?n.";
            return RedirectToAction("Details", new { id = datxe.Id });
        }

        // Chi ti?t don d?t xe
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

            // Ki?m tra quy?n xem
            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            if (khachhang == null || datxe.Khachhangid != khachhang.Id)
            {
                return Forbid();
            }

            return View(datxe);
        }

        // H?y don d?t xe
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng dang nh?p" });
            }

            var khachhang = await _context.Khachhangs.FirstOrDefaultAsync(k => k.Nguoidungid == userId.Value);
            var datxe = await _context.Datxes.FindAsync(id);

            if (datxe == null || datxe.Khachhangid != khachhang.Id)
            {
                return Json(new { success = false, message = "Không tìm th?y don d?t xe" });
            }

            if (datxe.Trangthai != "Pending" && datxe.Trangthai != "Confirmed")
            {
                return Json(new { success = false, message = "Không th? h?y don ? tr?ng thái này" });
            }

            datxe.Trangthai = "Cancelled";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "H?y don thành công" });
        }
    }
}
