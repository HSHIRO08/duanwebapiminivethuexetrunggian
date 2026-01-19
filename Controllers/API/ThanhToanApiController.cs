using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        /// Lấy danh sách tất cả thanh toán
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
        /// Lấy thông tin thanh toán theo ID
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
                return NotFound(new { message = "Không tìm thấy thanh toán" });
            }

            return thanhtoan;
        }

        /// <summary>
        /// Lấy thanh toán theo đơn đặt xe
        /// </summary>
        /// <param name="datxeId">ID đơn đặt xe</param>
        /// <returns>Danh sách thanh toán</returns>
        [HttpGet("datxe/{datxeId}")]
        public async Task<ActionResult<IEnumerable<Thanhtoan>>> GetThanhtoanByDatxe(int datxeId)
        {
            return await _context.Thanhtoans
                .Where(t => t.Datxeid == datxeId)
                .ToListAsync();
        }

        /// <summary>
        /// Tạo thanh toán mới
        /// </summary>
        /// <param name="thanhtoan">Thông tin thanh toán</param>
        /// <returns>Thanh toán vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<Thanhtoan>> CreateThanhtoan(Thanhtoan thanhtoan)
        {
            // Kiểm tra đơn đặt xe có tồn tại
            var datxe = await _context.Datxes.FindAsync(thanhtoan.Datxeid);
            if (datxe == null)
            {
                return BadRequest(new { message = "Đơn đặt xe không tồn tại" });
            }

            // Tạo mã giao dịch - SỬa: DateTime.Now thay vì DateTime.UtcNow
            thanhtoan.Magiaodich = $"TT{DateTime.Now:yyyyMMddHHmmss}{thanhtoan.Datxeid}";
            thanhtoan.Ngaythanhtoan = DateTime.Now;  // ✅ SỬa: DateTime.Now
            thanhtoan.Trangthai = "Pending";

            _context.Thanhtoans.Add(thanhtoan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThanhtoan), new { id = thanhtoan.Id }, thanhtoan);
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán
        /// </summary>
        /// <param name="id">ID thanh toán</param>
        /// <param name="trangthai">Trạng thái mới (Pending, Completed, Failed)</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string trangthai)
        {
            var thanhtoan = await _context.Thanhtoans.FindAsync(id);
            if (thanhtoan == null)
            {
                return NotFound(new { message = "Không tìm thấy thanh toán" });
            }

            thanhtoan.Trangthai = trangthai;

            if (trangthai == "Completed")
            {
                thanhtoan.Ngayxacnhan = DateTime.Now;  // ✅ SỬa: DateTime.Now
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thanh toán thành công", trangthai });
        }

        /// <summary>
        /// Xóa thanh toán
        /// </summary>
        /// <param name="id">ID thanh toán</param>
        /// <returns>Kết quả xóa</returns>
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
