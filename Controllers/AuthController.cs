using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

namespace duanminiveprogresql.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Login
        public IActionResult Login(string returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            _logger.LogInformation($"=== LOGIN ATTEMPT: {email} ===");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Empty email or password");
                ModelState.AddModelError("", "Email và mật khẩu không được trống");
                return View();
            }

            try
            {
                // Tìm user theo email
                var user = await _context.Nguoidungs
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning($"User not found: {email}");
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                    return View();
                }

                _logger.LogInformation($"User found: ID={user.Id}, Email={user.Email}, Role={user.Vaitro}");

                // So sánh password trực tiếp (plain text)
                _logger.LogInformation($"Password check - Input: {password}");
                _logger.LogInformation($"Password check - DB: {user.Matkhau}");
                
                if (user.Matkhau != password)
                {
                    _logger.LogWarning($"Password mismatch for: {email}");
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                    return View();
                }

                _logger.LogInformation("Password matched!");

                if (!user.Trangthai)
                {
                    _logger.LogWarning($"Account locked: {email}");
                    ModelState.AddModelError("", "Tài khoản đã bị khóa");
                    return View();
                }

                _logger.LogInformation("Saving to session...");
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Hoten ?? "User");
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Vaitro ?? "Customer");

                var sessionUserId = HttpContext.Session.GetInt32("UserId");
                _logger.LogInformation($"Session saved: UserId={sessionUserId}");
                
                if (sessionUserId == null)
                {
                    _logger.LogError("Failed to save session!");
                    ModelState.AddModelError("", "Lỗi hệ thống: không thể lưu vào session, Thử lại");
                    return View();
                }

                _logger.LogInformation($"LOGIN SUCCESS: {email}");

                TempData["SuccessMessage"] = "Đăng nhập thành công!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogInformation($"Redirecting to: {returnUrl}");
                    return Redirect(returnUrl);
                }

                _logger.LogInformation("Redirecting to Home");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                ModelState.AddModelError("", "Có lỗi xảy ra");
                return View();
            }
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string hoten, string email, string password, string confirmPassword, string sodienthoai)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View();
            }

            try
            {
                // Kiểm tra email đã tồn tại
                var existingUser = await _context.Nguoidungs.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email đã được sử dụng");
                    return View();
                }

                // Tạo người dùng mới với password plain text
                var newUser = new Nguoidung
                {
                    Hoten = hoten,
                    Email = email,
                    Matkhau = password, // ⚠️ Lưu plain text - CHỈ DÙNG CHO DEVELOPMENT!
                    Sodienthoai = sodienthoai,
                    Vaitro = "Customer",
                    Trangthai = true,
                    Ngaytao = DateTime.Now
                };

                _context.Nguoidungs.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New user created: ID={newUser.Id}, Email={email}");

                // Tạo khách hàng - SỬa: DateTime.Now thay vì DateTime.UtcNow
                var newCustomer = new Khachhang
                {
                    Nguoidungid = newUser.Id,
                    Daxacthuc = false,
                    Ngaydangky = DateTime.Now  // ✅ SỬa: DateTime.Now cho timestamp without time zone
                };

                _context.Khachhangs.Add(newCustomer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                return View();
            }
        }

        // Logout
        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _logger.LogInformation($"User logout: ID={userId}");
            
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đã đăng xuất thành công";
            return RedirectToAction("Index", "Home");
        }

        // Test action để kiểm tra session
        public IActionResult TestSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            
            return Content($"UserId: {userId}, UserName: {userName}");
        }
    }
}
