using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KhachHangApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        /// <returns>Danh sách khách hàng</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetKhachhangs()
        {
            return await _context.Khachhangs
                .Include(k => k.Nguoidung)
                .Select(k => new
                {
                    k.Id,
                    k.Nguoidungid,
                    Email = k.Nguoidung.Email,
                    Hoten = k.Nguoidung.Hoten,
                    Sodienthoai = k.Nguoidung.Sodienthoai,
                    k.Cmnd,
                    k.Banglai,
                    k.Ngaysinh,
                    k.Gioitinh,
                    k.Diachichitiet,
                    k.Daxacthuc,
                    k.Ngaydangky
                })
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo ID
        /// </summary>
        /// <param name="id">ID khách hàng</param>
        /// <returns>Thông tin khách hàng</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Khachhang>> GetKhachhang(int id)
        {
            var khachhang = await _context.Khachhangs
                .Include(k => k.Nguoidung)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (khachhang == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng" });
            }

            return khachhang;
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo User ID
        /// </summary>
        /// <param name="nguoidungId">ID người dùng</param>
        /// <returns>Thông tin khách hàng</returns>
        [HttpGet("user/{nguoidungId}")]
        public async Task<ActionResult<Khachhang>> GetKhachhangByUserId(int nguoidungId)
        {
            var khachhang = await _context.Khachhangs
                .Include(k => k.Nguoidung)
                .FirstOrDefaultAsync(k => k.Nguoidungid == nguoidungId);

            if (khachhang == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng" });
            }

            return khachhang;
        }

        /// <summary>
        /// Tạo khách hàng mới
        /// </summary>
        /// <param name="khachhang">Thông tin khách hàng</param>
        /// <returns>Khách hàng vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<Khachhang>> CreateKhachhang(Khachhang khachhang)
        {
            // Kiểm tra user có tồn tại
            var nguoidung = await _context.Nguoidungs.FindAsync(khachhang.Nguoidungid);
            if (nguoidung == null)
            {
                return BadRequest(new { message = "Người dùng không tồn tại" });
            }

            // Kiểm tra đã có khách hàng cho user này chưa
            if (await _context.Khachhangs.AnyAsync(k => k.Nguoidungid == khachhang.Nguoidungid))
            {
                return BadRequest(new { message = "Người dùng này đã có thông tin khách hàng" });
            }

            khachhang.Ngaydangky = DateTime.UtcNow;
            khachhang.Daxacthuc = false;

            _context.Khachhangs.Add(khachhang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKhachhang), new { id = khachhang.Id }, khachhang);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="id">ID khách hàng</param>
        /// <param name="khachhang">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKhachhang(int id, Khachhang khachhang)
        {
            if (id != khachhang.Id)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            _context.Entry(khachhang).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhachhangExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy khách hàng" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Cập nhật thành công" });
        }

        /// <summary>
        /// Xác thực khách hàng
        /// </summary>
        /// <param name="id">ID khách hàng</param>
        /// <returns>Kết quả xác thực</returns>
        [HttpPatch("{id}/verify")]
        public async Task<IActionResult> VerifyKhachhang(int id)
        {
            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng" });
            }

            khachhang.Daxacthuc = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xác thực khách hàng thành công" });
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        /// <param name="id">ID khách hàng</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKhachhang(int id)
        {
            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng" });
            }

            _context.Khachhangs.Remove(khachhang);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa khách hàng thành công" });
        }

        private bool KhachhangExists(int id)
        {
            return _context.Khachhangs.Any(e => e.Id == id);
        }
    }
}
