using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoTroKhachHangApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HoTroKhachHangApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả yêu cầu hỗ trợ
        /// </summary>
        /// <returns>Danh sách yêu cầu</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHotrokhachhangs()
        {
            return await _context.Hotrokhachhangs
                .Include(h => h.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Select(h => new
                {
                    h.Id,
                    h.Khachhangid,
                    KhachhangTen = h.Khachhang.Nguoidung.Hoten,
                    KhachhangEmail = h.Khachhang.Nguoidung.Email,
                    h.Tieude,
                    h.Loaiyeucau,
                    h.Trangthai,
                    h.Mucdouutien,
                    h.Ngaytao,
                    h.Ngaycapnhat
                })
                .OrderByDescending(h => h.Ngaytao)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy chi tiết yêu cầu hỗ trợ
        /// </summary>
        /// <param name="id">ID yêu cầu</param>
        /// <returns>Thông tin yêu cầu</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotrokhachhang>> GetHotrokhachhang(int id)
        {
            var hotro = await _context.Hotrokhachhangs
                .Include(h => h.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(h => h.Nhanvienxuly)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotro == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            return hotro;
        }

        /// <summary>
        /// Lấy yêu cầu theo khách hàng
        /// </summary>
        /// <param name="khachhangId">ID khách hàng</param>
        /// <returns>Danh sách yêu cầu</returns>
        [HttpGet("khachhang/{khachhangId}")]
        public async Task<ActionResult<IEnumerable<Hotrokhachhang>>> GetHotroByKhachhang(int khachhangId)
        {
            return await _context.Hotrokhachhangs
                .Where(h => h.Khachhangid == khachhangId)
                .OrderByDescending(h => h.Ngaytao)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy yêu cầu theo trạng thái
        /// </summary>
        /// <param name="trangthai">Trạng thái (Open, In Progress, Resolved, Closed)</param>
        /// <returns>Danh sách yêu cầu</returns>
        [HttpGet("trangthai/{trangthai}")]
        public async Task<ActionResult<IEnumerable<Hotrokhachhang>>> GetHotroByStatus(string trangthai)
        {
            return await _context.Hotrokhachhangs
                .Include(h => h.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Where(h => h.Trangthai == trangthai)
                .OrderByDescending(h => h.Ngaytao)
                .ToListAsync();
        }

        /// <summary>
        /// Tạo yêu cầu hỗ trợ mới
        /// </summary>
        /// <param name="hotro">Thông tin yêu cầu</param>
        /// <returns>Yêu cầu vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<Hotrokhachhang>> CreateHotrokhachhang(Hotrokhachhang hotro)
        {
            // Validate khách hàng
            var khachhang = await _context.Khachhangs.FindAsync(hotro.Khachhangid);
            if (khachhang == null)
            {
                return BadRequest(new { message = "Khách hàng không tồn tại" });
            }

            hotro.Ngaytao = DateTime.UtcNow;
            hotro.Trangthai = "Open";
            hotro.Mucdouutien = hotro.Mucdouutien ?? "Normal";

            _context.Hotrokhachhangs.Add(hotro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotrokhachhang), new { id = hotro.Id }, hotro);
        }

        /// <summary>
        /// Cập nhật yêu cầu hỗ trợ
        /// </summary>
        /// <param name="id">ID yêu cầu</param>
        /// <param name="hotro">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotrokhachhang(int id, Hotrokhachhang hotro)
        {
            if (id != hotro.Id)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            hotro.Ngaycapnhat = DateTime.UtcNow;
            _context.Entry(hotro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotroExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy yêu cầu" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Cập nhật thành công" });
        }

        /// <summary>
        /// Trả lời yêu cầu hỗ trợ
        /// </summary>
        /// <param name="id">ID yêu cầu</param>
        /// <param name="model">Thông tin trả lời</param>
        /// <returns>Kết quả</returns>
        [HttpPatch("{id}/reply")]
        public async Task<IActionResult> ReplyHotro(int id, [FromBody] ReplyModel model)
        {
            var hotro = await _context.Hotrokhachhangs.FindAsync(id);
            if (hotro == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            hotro.Traloi = model.Traloi;
            hotro.Nhanvienxulyid = model.NhanvienxulyId;
            hotro.Trangthai = "Resolved";
            hotro.Ngaygiaiquyet = DateTime.UtcNow;
            hotro.Ngaycapnhat = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Trả lời thành công" });
        }

        /// <summary>
        /// Cập nhật trạng thái
        /// </summary>
        /// <param name="id">ID yêu cầu</param>
        /// <param name="trangthai">Trạng thái mới</param>
        /// <returns>Kết quả</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string trangthai)
        {
            var hotro = await _context.Hotrokhachhangs.FindAsync(id);
            if (hotro == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            hotro.Trangthai = trangthai;
            hotro.Ngaycapnhat = DateTime.UtcNow;

            if (trangthai == "Resolved" || trangthai == "Closed")
            {
                hotro.Ngaygiaiquyet = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", trangthai });
        }

        /// <summary>
        /// Xóa yêu cầu
        /// </summary>
        /// <param name="id">ID yêu cầu</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotrokhachhang(int id)
        {
            var hotro = await _context.Hotrokhachhangs.FindAsync(id);
            if (hotro == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            _context.Hotrokhachhangs.Remove(hotro);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa yêu cầu thành công" });
        }

        private bool HotroExists(int id)
        {
            return _context.Hotrokhachhangs.Any(e => e.Id == id);
        }
    }

    public class ReplyModel
    {
        public string Traloi { get; set; } = null!;
        public int? NhanvienxulyId { get; set; }
    }
}
