using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NguoiDungApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// L?y danh sách t?t c? ngu?i dùng
        /// </summary>
        /// <returns>Danh sách ngu?i dùng</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNguoidungs()
        {
            return await _context.Nguoidungs
                .Select(n => new
                {
                    n.Id,
                    n.Email,
                    n.Hoten,
                    n.Sodienthoai,
                    n.Diachi,
                    n.Vaitro,
                    n.Trangthai,
                    n.Ngaytao
                    // Không tr? v? m?t kh?u
                })
                .ToListAsync();
        }

        /// <summary>
        /// L?y thông tin ngu?i dùng theo ID
        /// </summary>
        /// <param name="id">ID ngu?i dùng</param>
        /// <returns>Thông tin ngu?i dùng</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNguoidung(int id)
        {
            var nguoidung = await _context.Nguoidungs
                .Where(n => n.Id == id)
                .Select(n => new
                {
                    n.Id,
                    n.Email,
                    n.Hoten,
                    n.Sodienthoai,
                    n.Diachi,
                    n.Vaitro,
                    n.Trangthai,
                    n.Ngaytao
                })
                .FirstOrDefaultAsync();

            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm th?y ngu?i dùng" });
            }

            return nguoidung;
        }

        /// <summary>
        /// Tìm ki?m ngu?i dùng theo email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Thông tin ngu?i dùng</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<object>> GetNguoidungByEmail(string email)
        {
            var nguoidung = await _context.Nguoidungs
                .Where(n => n.Email == email)
                .Select(n => new
                {
                    n.Id,
                    n.Email,
                    n.Hoten,
                    n.Sodienthoai,
                    n.Diachi,
                    n.Vaitro,
                    n.Trangthai,
                    n.Ngaytao
                })
                .FirstOrDefaultAsync();

            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm th?y ngu?i dùng" });
            }

            return nguoidung;
        }

        /// <summary>
        /// T?o ngu?i dùng m?i
        /// </summary>
        /// <param name="model">Thông tin ngu?i dùng</param>
        /// <returns>Ngu?i dùng v?a t?o</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateNguoidung([FromBody] CreateNguoidungModel model)
        {
            // Ki?m tra email dã t?n t?i
            if (await _context.Nguoidungs.AnyAsync(n => n.Email == model.Email))
            {
                return BadRequest(new { message = "Email dã du?c s? d?ng" });
            }

            // S?A: DateTime.Now thay vì DateTime.UtcNow
            var nguoidung = new Nguoidung
            {
                Email = model.Email,
                Matkhau = HashPassword(model.Password),
                Hoten = model.Hoten,
                Sodienthoai = model.Sodienthoai,
                Diachi = model.Diachi,
                Vaitro = model.Vaitro ?? "Customer",
                Trangthai = true,
                Ngaytao = DateTime.Now  // ? S?A: DateTime.Now
            };

            _context.Nguoidungs.Add(nguoidung);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNguoidung), new { id = nguoidung.Id }, new
            {
                nguoidung.Id,
                nguoidung.Email,
                nguoidung.Hoten,
                nguoidung.Sodienthoai,
                nguoidung.Diachi,
                nguoidung.Vaitro,
                nguoidung.Trangthai,
                nguoidung.Ngaytao
            });
        }

        /// <summary>
        /// C?p nh?t thông tin ngu?i dùng
        /// </summary>
        /// <param name="id">ID ngu?i dùng</param>
        /// <param name="model">Thông tin c?p nh?t</param>
        /// <returns>K?t qu? c?p nh?t</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNguoidung(int id, [FromBody] UpdateNguoidungModel model)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm th?y ngu?i dùng" });
            }

            // Ki?m tra email trùng v?i user khác
            if (model.Email != nguoidung.Email)
            {
                if (await _context.Nguoidungs.AnyAsync(n => n.Email == model.Email && n.Id != id))
                {
                    return BadRequest(new { message = "Email dã du?c s? d?ng" });
                }
                nguoidung.Email = model.Email;
            }

            nguoidung.Hoten = model.Hoten;
            nguoidung.Sodienthoai = model.Sodienthoai;
            nguoidung.Diachi = model.Diachi;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                nguoidung.Matkhau = HashPassword(model.NewPassword);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "C?p nh?t thành công" });
        }

        /// <summary>
        /// C?p nh?t tr?ng thái ngu?i dùng
        /// </summary>
        /// <param name="id">ID ngu?i dùng</param>
        /// <param name="trangthai">Tr?ng thái (true: active, false: inactive)</param>
        /// <returns>K?t qu? c?p nh?t</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool trangthai)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm th?y ngu?i dùng" });
            }

            nguoidung.Trangthai = trangthai;
            await _context.SaveChangesAsync();

            return Ok(new { message = "C?p nh?t tr?ng thái thành công", trangthai });
        }

        /// <summary>
        /// Xóa ngu?i dùng
        /// </summary>
        /// <param name="id">ID ngu?i dùng</param>
        /// <returns>K?t qu? xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoidung(int id)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm th?y ngu?i dùng" });
            }

            _context.Nguoidungs.Remove(nguoidung);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa ngu?i dùng thành công" });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }

    // DTOs
    public class CreateNguoidungModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Hoten { get; set; } = null!;
        public string? Sodienthoai { get; set; }
        public string? Diachi { get; set; }
        public string? Vaitro { get; set; }
    }

    public class UpdateNguoidungModel
    {
        public string Email { get; set; } = null!;
        public string Hoten { get; set; } = null!;
        public string? Sodienthoai { get; set; }
        public string? Diachi { get; set; }
        public string? NewPassword { get; set; }
    }
}
