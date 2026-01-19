using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichSuThueApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LichSuThueApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả lịch sử thuê xe
        /// </summary>
        /// <returns>Danh sách lịch sử</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lichsuthue>>> GetLichsuthues()
        {
            return await _context.Lichsuthues
                .Include(l => l.Xe)
                .Include(l => l.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(l => l.Datxe)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy lịch sử thuê theo ID
        /// </summary>
        /// <param name="id">ID lịch sử</param>
        /// <returns>Thông tin lịch sử</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Lichsuthue>> GetLichsuthue(int id)
        {
            var lichsuthue = await _context.Lichsuthues
                .Include(l => l.Xe)
                .Include(l => l.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(l => l.Datxe)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lichsuthue == null)
            {
                return NotFound(new { message = "Không tìm thấy lịch sử" });
            }

            return lichsuthue;
        }

        /// <summary>
        /// Lấy lịch sử thuê theo khách hàng
        /// </summary>
        /// <param name="khachhangId">ID khách hàng</param>
        /// <returns>Danh sách lịch sử</returns>
        [HttpGet("khachhang/{khachhangId}")]
        public async Task<ActionResult<IEnumerable<Lichsuthue>>> GetLichsuthueByKhachhang(int khachhangId)
        {
            return await _context.Lichsuthues
                .Include(l => l.Xe)
                .Where(l => l.Khachhangid == khachhangId)
                .OrderByDescending(l => l.Ngaytraxe)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy lịch sử thuê theo xe
        /// </summary>
        /// <param name="xeId">ID xe</param>
        /// <returns>Danh sách lịch sử</returns>
        [HttpGet("xe/{xeId}")]
        public async Task<ActionResult<IEnumerable<Lichsuthue>>> GetLichsuthueByXe(int xeId)
        {
            return await _context.Lichsuthues
                .Include(l => l.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Where(l => l.Xeid == xeId)
                .OrderByDescending(l => l.Ngaytraxe)
                .ToListAsync();
        }

        /// <summary>
        /// Tạo lịch sử thuê mới
        /// </summary>
        /// <param name="lichsuthue">Thông tin lịch sử</param>
        /// <returns>Lịch sử vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<Lichsuthue>> CreateLichsuthue(Lichsuthue lichsuthue)
        {
            // Validate
            var xe = await _context.Xes.FindAsync(lichsuthue.Xeid);
            if (xe == null)
            {
                return BadRequest(new { message = "Xe không tồn tại" });
            }

            var khachhang = await _context.Khachhangs.FindAsync(lichsuthue.Khachhangid);
            if (khachhang == null)
            {
                return BadRequest(new { message = "Khách hàng không tồn tại" });
            }

            var datxe = await _context.Datxes.FindAsync(lichsuthue.Datxeid);
            if (datxe == null)
            {
                return BadRequest(new { message = "Đơn đặt xe không tồn tại" });
            }

            lichsuthue.Ngaynhanxe = DateTime.Now; // ✅ SỬa: DateTime.Now

            _context.Lichsuthues.Add(lichsuthue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLichsuthue), new { id = lichsuthue.Id }, lichsuthue);
        }

        /// <summary>
        /// Cập nhật lịch sử (trả xe, phí phát sinh, đánh giá)
        /// </summary>
        /// <param name="id">ID lịch sử</param>
        /// <param name="lichsuthue">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLichsuthue(int id, Lichsuthue lichsuthue)
        {
            if (id != lichsuthue.Id)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            _context.Entry(lichsuthue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LichsuthueExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy lịch sử" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Cập nhật thành công" });
        }

        /// <summary>
        /// Đánh giá chuyến thuê
        /// </summary>
        /// <param name="id">ID lịch sử</param>
        /// <param name="model">Thông tin đánh giá</param>
        /// <returns>Kết quả đánh giá</returns>
        [HttpPatch("{id}/rate")]
        public async Task<IActionResult> RateLichsuthue(int id, [FromBody] RatingModel model)
        {
            var lichsuthue = await _context.Lichsuthues.FindAsync(id);
            if (lichsuthue == null)
            {
                return NotFound(new { message = "Không tìm thấy lịch sử" });
            }

            if (model.Danhgia < 1 || model.Danhgia > 5)
            {
                return BadRequest(new { message = "Đánh giá phải từ 1-5 sao" });
            }

            lichsuthue.Danhgia = model.Danhgia;
            lichsuthue.Nhanxet = model.Nhanxet;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đánh giá thành công" });
        }

        /// <summary>
        /// Xóa lịch sử
        /// </summary>
        /// <param name="id">ID lịch sử</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLichsuthue(int id)
        {
            var lichsuthue = await _context.Lichsuthues.FindAsync(id);
            if (lichsuthue == null)
            {
                return NotFound(new { message = "Không tìm thấy lịch sử" });
            }

            _context.Lichsuthues.Remove(lichsuthue);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa lịch sử thành công" });
        }

        private bool LichsuthueExists(int id)
        {
            return _context.Lichsuthues.Any(e => e.Id == id);
        }
    }

    public class RatingModel
    {
        public int Danhgia { get; set; }
        public string? Nhanxet { get; set; }
    }
}
