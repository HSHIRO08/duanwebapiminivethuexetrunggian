using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách người dùng</returns>
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
                    // Không trả về mật khẩu
                })
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        /// <param name="id">ID người dùng</param>
        /// <returns>Thông tin người dùng</returns>
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
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return nguoidung;
        }

        /// <summary>
        /// Tìm kiếm người dùng theo email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Thông tin người dùng</returns>
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
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return nguoidung;
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="model">Thông tin người dùng</param>
        /// <returns>Người dùng vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateNguoidung([FromBody] CreateNguoidungModel model)
        {
            // Kiểm tra email đã tồn tại
            if (await _context.Nguoidungs.AnyAsync(n => n.Email == model.Email))
            {
                return BadRequest(new { message = "Email đã được sử dụng" });
            }

            // SỬa: DateTime.Now thay vì DateTime.UtcNow
            var nguoidung = new Nguoidung
            {
                Email = model.Email,
                Matkhau = HashPassword(model.Password),
                Hoten = model.Hoten,
                Sodienthoai = model.Sodienthoai,
                Diachi = model.Diachi,
                Vaitro = model.Vaitro ?? "Customer",
                Trangthai = true,
                Ngaytao = DateTime.Now  // ✅ SỬa: DateTime.Now
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
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="id">ID người dùng</param>
        /// <param name="model">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNguoidung(int id, [FromBody] UpdateNguoidungModel model)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            // Kiểm tra email trùng với user khác
            if (model.Email != nguoidung.Email)
            {
                if (await _context.Nguoidungs.AnyAsync(n => n.Email == model.Email && n.Id != id))
                {
                    return BadRequest(new { message = "Email đã được sử dụng" });
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

            return Ok(new { message = "Cập nhật thành công" });
        }

        /// <summary>
        /// Cập nhật trạng thái người dùng
        /// </summary>
        /// <param name="id">ID người dùng</param>
        /// <param name="trangthai">Trạng thái (true: active, false: inactive)</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool trangthai)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            nguoidung.Trangthai = trangthai;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", trangthai });
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID người dùng</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoidung(int id)
        {
            var nguoidung = await _context.Nguoidungs.FindAsync(id);
            if (nguoidung == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            _context.Nguoidungs.Remove(nguoidung);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa người dùng thành công" });
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
