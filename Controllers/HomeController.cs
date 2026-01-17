using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

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

        // Trang ch? - Hi?n th? xe n?i b?t
        public async Task<IActionResult> Index()
        {
            try
            {
                // L?y xe n?i b?t (Available)
                var featuredCars = await _context.Xes
                    .Where(x => x.Trangthai == "Available")
                    .OrderByDescending(x => x.Ngaytao)
                    .Take(6)
                    .ToListAsync();

                // Th?ng kê
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

        // V? chúng tôi
        public IActionResult About()
        {
            ViewBag.CompanyName = "AutoRent - Cho Thuê Xe Ô Tô";
            ViewBag.Founded = "2024";
            ViewBag.TotalCustomers = _context.Khachhangs.Count();
            ViewBag.TotalCars = _context.Xes.Count();
            
            return View();
        }

        // Liên h?
        public IActionResult Contact()
        {
            return View();
        }

        // X? lý form liên h?
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string phone, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                TempData["ErrorMessage"] = "Vui lòng di?n d?y d? thông tin!";
                return View();
            }

            try
            {
                // T?o ticket h? tr?
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
                            Tieude = $"Liên h? t? {name}",
                            Noidung = $"Email: {email}\nSÐT: {phone}\n\n{message}",
                            Loaiyeucau = "Liên h?",
                            Trangthai = "Open",
                            Mucdouutien = "Normal",
                            Ngaytao = DateTime.UtcNow
                        };

                        _context.Hotrokhachhangs.Add(ticket);
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "C?m on b?n dã liên h?! Chúng tôi s? ph?n h?i s?m nh?t.";
                _logger.LogInformation($"Contact form submitted: {name} - {email}");
                
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form");
                TempData["ErrorMessage"] = "Có l?i x?y ra. Vui lòng th? l?i!";
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
