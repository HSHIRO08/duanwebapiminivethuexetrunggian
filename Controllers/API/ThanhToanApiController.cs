using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThanhToanApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// L?y danh sách t?t c? thanh toán
        /// </summary>
        /// <returns>Danh sách thanh toán</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thanhtoan>>> GetThanhtoans()
        {
            return await _context.Thanhtoans
                .Include(t => t.Datxe)
                    .ThenInclude(d => d.Xe)
                .Include(t => t.Datxe)
                    .ThenInclude(d => d.Khachhang)
                        .ThenInclude(k => k.Nguoidung)
                .ToListAsync();
        }

        /// <summary>
        /// L?y thông tin thanh toán theo ID
        /// </summary>
        /// <param name="id">ID thanh toán</param>
        /// <returns>Thông tin thanh toán</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Thanhtoan>> GetThanhtoan(int id)
        {
            var thanhtoan = await _context.Thanhtoans
                .Include(t => t.Datxe)
                    .ThenInclude(d => d.Xe)
                .Include(t => t.Datxe)
                    .ThenInclude(d => d.Khachhang)
                        .ThenInclude(k => k.Nguoidung)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (thanhtoan == null)
            {
                return NotFound(new { message = "Không tìm th?y thanh toán" });
            }

            return thanhtoan;
        }

        /// <summary>
        /// L?y thanh toán theo don d?t xe
        /// </summary>
        /// <param name="datxeId">ID don d?t xe</param>
        /// <returns>Danh sách thanh toán</returns>
        [HttpGet("datxe/{datxeId}")]
        public async Task<ActionResult<IEnumerable<Thanhtoan>>> GetThanhtoanByDatxe(int datxeId)
        {
            return await _context.Thanhtoans
                .Where(t => t.Datxeid == datxeId)
                .ToListAsync();
        }

        /// <summary>
        /// T?o thanh toán m?i
        /// </summary>
        /// <param name="thanhtoan">Thông tin thanh toán</param>
        /// <returns>Thanh toán v?a t?o</returns>
        [HttpPost]
        public async Task<ActionResult<Thanhtoan>> CreateThanhtoan(Thanhtoan thanhtoan)
        {
            // Ki?m tra don d?t xe có t?n t?i
            var datxe = await _context.Datxes.FindAsync(thanhtoan.Datxeid);
            if (datxe == null)
            {
                return BadRequest(new { message = "Ðon d?t xe không t?n t?i" });
            }

            // T?o mã giao d?ch - S?A: DateTime.Now thay vì DateTime.UtcNow
            thanhtoan.Magiaodich = $"TT{DateTime.Now:yyyyMMddHHmmss}{thanhtoan.Datxeid}";
            thanhtoan.Ngaythanhtoan = DateTime.Now;  // ? S?A: DateTime.Now
            thanhtoan.Trangthai = "Pending";

            _context.Thanhtoans.Add(thanhtoan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThanhtoan), new { id = thanhtoan.Id }, thanhtoan);
        }

        /// <summary>
        /// C?p nh?t tr?ng thái thanh toán
        /// </summary>
        /// <param name="id">ID thanh toán</param>
        /// <param name="trangthai">Tr?ng thái m?i (Pending, Completed, Failed)</param>
        /// <returns>K?t qu? c?p nh?t</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string trangthai)
        {
            var thanhtoan = await _context.Thanhtoans.FindAsync(id);
            if (thanhtoan == null)
            {
                return NotFound(new { message = "Không tìm th?y thanh toán" });
            }

            thanhtoan.Trangthai = trangthai;
            
            if (trangthai == "Completed")
            {
                thanhtoan.Ngayxacnhan = DateTime.Now;  // ? S?A: DateTime.Now
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "C?p nh?t tr?ng thái thanh toán thành công", trangthai });
        }

        /// <summary>
        /// Xóa thanh toán
        /// </summary>
        /// <param name="id">ID thanh toán</param>
        /// <returns>K?t qu? xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThanhtoan(int id)
        {
            var thanhtoan = await _context.Thanhtoans.FindAsync(id);
            if (thanhtoan == null)
            {
                return NotFound(new { message = "Không tìm th?y thanh toán" });
            }

            _context.Thanhtoans.Remove(thanhtoan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thanh toán thành công" });
        }
    }
}
