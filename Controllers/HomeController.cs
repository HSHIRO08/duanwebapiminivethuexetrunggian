using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace duanminiveprogresql.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Trang chủ - Hiển thị xe nổi bật
        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy xe nổi bật (Available)
                var featuredCars = await _context.Xes
                    .Where(x => x.Trangthai == "Available")
                    .OrderByDescending(x => x.Ngaytao)
                    .Take(6)
                    .ToListAsync();

                // Thống kê
                ViewBag.TotalCars = await _context.Xes.CountAsync();
                ViewBag.AvailableCars = await _context.Xes.CountAsync(x => x.Trangthai == "Available");
                ViewBag.TotalBookings = await _context.Datxes.CountAsync();

                return View(featuredCars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View(new List<Xe>());
            }
        }

        // Về chúng tôi
        public IActionResult About()
        {
            ViewBag.CompanyName = "Cho Thuê Xe Ô Tô";
            ViewBag.Founded = "2026";
            ViewBag.TotalCustomers = _context.Khachhangs.Count();
            ViewBag.TotalCars = _context.Xes.Count();

            return View();
        }

        // Liên hệ
        public IActionResult Contact()
        {
            return View();
        }

        // Xử lý form liên hệ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string phone, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin!";
                return View();
            }

            try
            {
                // Tạo ticket hỗ trợ
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId != null)
                {
                    var khachhang = await _context.Khachhangs
                        .FirstOrDefaultAsync(k => k.Nguoidungid == userId);

                    if (khachhang != null)
                    {
                        var ticket = new Hotrokhachhang
                        {
                            Khachhangid = khachhang.Id,
                            Tieude = $"Liên hệ từ {name}",
                            Noidung = $"Email: {email}\nSĐT: {phone}\n\n{message}",
                            Loaiyeucau = "Liên hệ",
                            Trangthai = "Open",
                            Mucdouutien = "Normal",
                            Ngaytao = DateTime.UtcNow
                        };

                        _context.Hotrokhachhangs.Add(ticket);
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi cho bạn sớm nhất.";
                _logger.LogInformation($"Contact form submitted: {name} - {email}");

                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form");
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                return View();
            }
        }

        // Tìm ki?m nhanh
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index", "Xe");
            }

            var cars = await _context.Xes
                .Where(x => x.Tenxe.Contains(keyword)
                    || x.Hangxe.Contains(keyword)
                    || x.Loaixe.Contains(keyword))
                .ToListAsync();

            ViewBag.Keyword = keyword;
            return View("~/Views/Xe/Index.cshtml", cars);
        }

        // Error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        // Privacy policy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
