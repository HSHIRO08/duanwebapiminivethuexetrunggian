using DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace duanminiveprogresql.Controllers
{
    public class DebugController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DebugController> _logger;

        public DebugController(AppDbContext context, ILogger<DebugController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Action d? ki?m tra session
        public IActionResult CheckSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userRole = HttpContext.Session.GetString("UserRole");

            var result = $@"
                <html>
                <head>
                    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
                </head>
                <body style='padding: 20px;'>
                    <div class='container'>
                        <div class='card'>
                            <div class='card-header bg-primary text-white'>
                                <h2>?? Session Information</h2>
                            </div>
                            <div class='card-body'>
                                <table class='table'>
                                    <tr>
                                        <th>UserId:</th>
                                        <td><code>{userId?.ToString() ?? "NULL"}</code></td>
                                        <td>{(userId != null ? "?" : "?")}</td>
                                    </tr>
                                    <tr>
                                        <th>UserName:</th>
                                        <td><code>{userName ?? "NULL"}</code></td>
                                        <td>{(!string.IsNullOrEmpty(userName) ? "?" : "?")}</td>
                                    </tr>
                                    <tr>
                                        <th>UserEmail:</th>
                                        <td><code>{userEmail ?? "NULL"}</code></td>
                                        <td>{(!string.IsNullOrEmpty(userEmail) ? "?" : "?")}</td>
                                    </tr>
                                    <tr>
                                        <th>UserRole:</th>
                                        <td><code>{userRole ?? "NULL"}</code></td>
                                        <td>{(!string.IsNullOrEmpty(userRole) ? "?" : "?")}</td>
                                    </tr>
                                    <tr>
                                        <th>Session Available:</th>
                                        <td><code>{HttpContext.Session.IsAvailable}</code></td>
                                        <td>{(HttpContext.Session.IsAvailable ? "?" : "?")}</td>
                                    </tr>
                                </table>
                                <hr>
                                <a href='/Auth/Login' class='btn btn-primary'>? Back to Login</a>
                                <a href='/Auth/Logout' class='btn btn-danger'>Logout</a>
                                <a href='/' class='btn btn-secondary'>Home</a>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return Content(result, "text/html", Encoding.UTF8);
        }

        // Action d? xem password (plain text - không hash)
        public IActionResult ViewPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Content("Vui lòng nhập password. Ví dụ: /Debug/ViewPassword?password=123456");
            }

            var result = $@"
                <html>
                <head>
                    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
                </head>
                <body style='padding: 20px;'>
                    <div class='container'>
                        <div class='card'>
                            <div class='card-header bg-success text-white'>
                                <h2>?? Plain Text Password (No Hash)</h2>
                            </div>
                            <div class='card-body'>
                                <table class='table'>
                                    <tr>
                                        <th>Password:</th>
                                        <td><code>{password}</code></td>
                                    </tr>
                                </table>
                                <div class='alert alert-warning'>
                                    <h5>?? CHÚ Ý B?O M?T</h5>
                                    <p>Password hi?n dang du?c luu d?ng <strong>plain text</strong> (không mã hóa)</p>
                                    <p>Ði?u này <strong>C?C K? KHÔNG AN TOÀN</strong> và ch? nên dùng cho môi tru?ng development/testing!</p>
                                </div>
                                <div class='alert alert-info'>
                                    <h5>?? SQL d? c?p nh?t trong database:</h5>
                                    <pre>UPDATE nguoidung 
SET matkhau = '{password}' 
WHERE email = 'your-email@example.com';</pre>
                                </div>
                                <a href='/Debug/ListUsers' class='btn btn-primary'>View All Users</a>
                                <a href='/' class='btn btn-secondary'>Home</a>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            ";

            return Content(result, "text/html", Encoding.UTF8);
        }

        // Action d? xem danh sách users
        public async Task<IActionResult> ListUsers()
        {
            var users = await _context.Nguoidungs
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Hoten,
                    u.Vaitro,
                    u.Trangthai,
                    PasswordLength = u.Matkhau != null ? u.Matkhau.Length : 0,
                    PasswordHash = u.Matkhau != null ? u.Matkhau.Substring(0, Math.Min(20, u.Matkhau.Length)) + "..." : "NULL"
                })
                .ToListAsync();

            var html = new StringBuilder();
            html.AppendLine("<html><head>");
            html.AppendLine("<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>");
            html.AppendLine("</head><body style='padding: 20px;'>");
            html.AppendLine("<div class='container'>");
            html.AppendLine("<div class='card'>");
            html.AppendLine("<div class='card-header bg-info text-white'>");
            html.AppendLine("<h2>?? Users in Database</h2>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='card-body'>");
            html.AppendLine("<table class='table table-striped table-bordered'>");
            html.AppendLine("<thead class='table-dark'>");
            html.AppendLine("<tr><th>ID</th><th>Email</th><th>H? tên</th><th>Vai trò</th><th>Tr?ng thái</th><th>Password Hash</th><th>Actions</th></tr>");
            html.AppendLine("</thead><tbody>");

            foreach (var user in users)
            {
                var statusBadge = user.Trangthai ? "<span class='badge bg-success'>Active</span>" : "<span class='badge bg-danger'>Inactive</span>";
                html.AppendLine($@"
                    <tr>
                        <td>{user.Id}</td>
                        <td><strong>{user.Email}</strong></td>
                        <td>{user.Hoten}</td>
                        <td><span class='badge bg-primary'>{user.Vaitro}</span></td>
                        <td>{statusBadge}</td>
                        <td><small><code>{user.PasswordHash}</code></small></td>
                        <td>
                            <a href='/Debug/TestLogin?email={user.Email}&password=123456' class='btn btn-sm btn-warning' target='_blank'>Test Login</a>
                        </td>
                    </tr>
                ");
            }

            html.AppendLine("</tbody></table>");
            html.AppendLine("<hr>");
            html.AppendLine("<a href='/Debug/ViewPassword?password=123456' class='btn btn-success'>View Password '123456'</a> ");
            html.AppendLine("<a href='/Debug/CheckSession' class='btn btn-info'>Check Session</a> ");
            html.AppendLine("<a href='/' class='btn btn-secondary'>Home</a>");
            html.AppendLine("</div></div></div>");
            html.AppendLine("</body></html>");

            return Content(html.ToString(), "text/html", Encoding.UTF8);
        }

        // Action d? test dang nh?p
        public async Task<IActionResult> TestLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Content(@"
                    <html>
                    <head><link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'></head>
                    <body style='padding: 20px;'>
                        <div class='container'>
                            <div class='alert alert-warning'>
                                <h2>?? Test Login</h2>
                                <p>Số dòng: <code>/Debug/TestLogin?email=your@email.com&password=yourpassword</code></p>
                                <a href='/Debug/ListUsers' class='btn btn-primary'>View All Users</a>
                            </div>
                        </div>
                    </body>
                    </html>
                ", "text/html", Encoding.UTF8);
            }

            var html = new StringBuilder();
            html.AppendLine("<html><head>");
            html.AppendLine("<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>");
            html.AppendLine("</head><body style='padding: 20px;'><div class='container'>");
            html.AppendLine("<div class='card'><div class='card-header bg-warning'>");
            html.AppendLine("<h2>?? Test Login Result</h2>");
            html.AppendLine("</div><div class='card-body'>");

            try
            {
                // Tìm user
                var user = await _context.Nguoidungs.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    html.AppendLine($"<div class='alert alert-danger'>? Không tìm th?y user v?i email: <strong>{email}</strong></div>");
                }
                else
                {
                    html.AppendLine($"<div class='alert alert-success'>? Tìm thấy user!</div>");
                    html.AppendLine("<table class='table'>");
                    html.AppendLine($"<tr><th>ID:</th><td>{user.Id}</td></tr>");
                    html.AppendLine($"<tr><th>Email:</th><td>{user.Email}</td></tr>");
                    html.AppendLine($"<tr><th>Họ tên:</th><td>{user.Hoten}</td></tr>");
                    html.AppendLine($"<tr><th>Vai trò:</th><td><span class='badge bg-primary'>{user.Vaitro}</span></td></tr>");
                    html.AppendLine($"<tr><th>Trạng thái:</th><td>{(user.Trangthai ? "<span class='badge bg-success'>Active</span>" : "<span class='badge bg-danger'>Inactive</span>")}</td></tr>");
                    html.AppendLine("</table>");

                    // So sánh plain text password
                    html.AppendLine("<div class='card mt-3'><div class='card-header bg-info text-white'><h5>?? Password Comparison (Plain Text)</h5></div><div class='card-body'>");
                    html.AppendLine("<table class='table'>");
                    html.AppendLine($"<tr><th>Password nhập vào:</th><td><code>{password}</code></td></tr>");
                    html.AppendLine($"<tr><th>Password trong DB:</th><td><code>{user.Matkhau}</code></td></tr>");

                    if (user.Matkhau == password)
                    {
                        html.AppendLine("<tr><td colspan='2'><div class='alert alert-success mb-0'><strong>? Password KH?P - Ðang nh?p s? thành công!</strong></div></td></tr>");
                    }
                    else
                    {
                        html.AppendLine("<tr><td colspan='2'><div class='alert alert-danger mb-0'><strong>? Password KHÔNG KH?P - Ðang nh?p s? th?t b?i!</strong>");
                        html.AppendLine("<hr>");
                        html.AppendLine("<p>Đã cập nhật password trong DB, chạy SQL:</p>");
                        html.AppendLine($"<pre>UPDATE nguoidung SET matkhau = '{password}' WHERE email = '{email}';</pre>");
                        html.AppendLine("</div></td></tr>");
                    }
                    html.AppendLine("</table></div></div>");
                }
            }
            catch (Exception ex)
            {
                html.AppendLine($"<div class='alert alert-danger'>? Lỗi: {ex.Message}</div>");
                _logger.LogError(ex, "Error in TestLogin");
            }

            html.AppendLine("<hr>");
            html.AppendLine("<a href='/Debug/ListUsers' class='btn btn-primary'>View All Users</a> ");
            html.AppendLine("<a href='/Auth/Login' class='btn btn-success'>Go to Login Page</a> ");
            html.AppendLine("<a href='/' class='btn btn-secondary'>Home</a>");
            html.AppendLine("</div></div></div></body></html>");

            return Content(html.ToString(), "text/html", Encoding.UTF8);
        }
    }
}
