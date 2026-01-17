using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class XeApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public XeApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả xe
        /// </summary>
        /// <returns>Danh sách xe</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Xe>>> GetXes()
        {
            return await _context.Xes.ToListAsync();
        }

        /// <summary>
        /// Lấy thông tin chi tiết một xe theo ID
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <returns>Thông tin xe</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Xe>> GetXe(int id)
        {
            var xe = await _context.Xes.FindAsync(id);

            if (xe == null)
            {
                return NotFound(new { message = "Không tìm thấy xe" });
            }

            return xe;
        }

        /// <summary>
        /// Tìm kiếm xe theo loại, hãng, giá
        /// </summary>
        /// <param name="loaixe">Loại xe</param>
        /// <param name="hangxe">Hãng xe</param>
        /// <param name="minPrice">Giá tối thiểu</param>
        /// <param name="maxPrice">Giá tối đa</param>
        /// <returns>Danh sách xe phù hợp</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Xe>>> SearchXe(
            [FromQuery] string? loaixe,
            [FromQuery] string? hangxe,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var query = _context.Xes.AsQueryable();

            if (!string.IsNullOrEmpty(loaixe))
            {
                query = query.Where(x => x.Loaixe == loaixe);
            }

            if (!string.IsNullOrEmpty(hangxe))
            {
                query = query.Where(x => x.Hangxe == hangxe);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Giathuetheongay >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Giathuetheongay <= maxPrice.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Thêm xe mới
        /// </summary>
        /// <param name="xe">Thông tin xe</param>
        /// <returns>Xe vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<Xe>> CreateXe(Xe xe)
        {
            xe.Ngaytao = DateTime.UtcNow;
            xe.Trangthai = "Available";

            _context.Xes.Add(xe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetXe), new { id = xe.Id }, xe);
        }

        /// <summary>
        /// Cập nhật thông tin xe
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <param name="xe">Thông tin xe mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateXe(int id, Xe xe)
        {
            if (id != xe.Id)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            xe.Ngaycapnhat = DateTime.UtcNow;
            _context.Entry(xe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!XeExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy xe" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa xe
        /// </summary>
        /// <param name="id">ID của xe</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteXe(int id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                return NotFound(new { message = "Không tìm thấy xe" });
            }

            _context.Xes.Remove(xe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa xe thành công" });
        }

        private bool XeExists(int id)
        {
            return _context.Xes.Any(e => e.Id == id);
        }
    }
}
