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
        /// L?y danh sách t?t c? xe
        /// </summary>
        /// <returns>Danh sách xe</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Xe>>> GetXes()
        {
            return await _context.Xes.ToListAsync();
        }

        /// <summary>
        /// L?y thông tin chi ti?t m?t xe theo ID
        /// </summary>
        /// <param name="id">ID c?a xe</param>
        /// <returns>Thông tin xe</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Xe>> GetXe(int id)
        {
            var xe = await _context.Xes.FindAsync(id);

            if (xe == null)
            {
                return NotFound(new { message = "Không tìm th?y xe" });
            }

            return xe;
        }

        /// <summary>
        /// Tìm ki?m xe theo lo?i, hãng, giá
        /// </summary>
        /// <param name="loaixe">Lo?i xe</param>
        /// <param name="hangxe">Hãng xe</param>
        /// <param name="minPrice">Giá t?i thi?u</param>
        /// <param name="maxPrice">Giá t?i da</param>
        /// <returns>Danh sách xe phù h?p</returns>
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
        /// Thêm xe m?i
        /// </summary>
        /// <param name="xe">Thông tin xe</param>
        /// <returns>Xe v?a t?o</returns>
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
        /// C?p nh?t thông tin xe
        /// </summary>
        /// <param name="id">ID c?a xe</param>
        /// <param name="xe">Thông tin xe m?i</param>
        /// <returns>K?t qu? c?p nh?t</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateXe(int id, Xe xe)
        {
            if (id != xe.Id)
            {
                return BadRequest(new { message = "ID không kh?p" });
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
                    return NotFound(new { message = "Không tìm th?y xe" });
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
        /// <param name="id">ID c?a xe</param>
        /// <returns>K?t qu? xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteXe(int id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                return NotFound(new { message = "Không tìm th?y xe" });
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
