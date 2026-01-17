using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

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
        /// L?y danh sách t?t c? l?ch s? thuê xe
        /// </summary>
        /// <returns>Danh sách l?ch s?</returns>
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
        /// L?y l?ch s? thuê theo ID
        /// </summary>
        /// <param name="id">ID l?ch s?</param>
        /// <returns>Thông tin l?ch s?</returns>
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
                return NotFound(new { message = "Không tìm th?y l?ch s?" });
            }

            return lichsuthue;
        }

        /// <summary>
        /// L?y l?ch s? thuê theo khách hàng
        /// </summary>
        /// <param name="khachhangId">ID khách hàng</param>
        /// <returns>Danh sách l?ch s?</returns>
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
        /// L?y l?ch s? thuê theo xe
        /// </summary>
        /// <param name="xeId">ID xe</param>
        /// <returns>Danh sách l?ch s?</returns>
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
        /// T?o l?ch s? thuê m?i
        /// </summary>
        /// <param name="lichsuthue">Thông tin l?ch s?</param>
        /// <returns>L?ch s? v?a t?o</returns>
        [HttpPost]
        public async Task<ActionResult<Lichsuthue>> CreateLichsuthue(Lichsuthue lichsuthue)
        {
            // Validate
            var xe = await _context.Xes.FindAsync(lichsuthue.Xeid);
            if (xe == null)
            {
                return BadRequest(new { message = "Xe không t?n t?i" });
            }

            var khachhang = await _context.Khachhangs.FindAsync(lichsuthue.Khachhangid);
            if (khachhang == null)
            {
                return BadRequest(new { message = "Khách hàng không t?n t?i" });
            }

            var datxe = await _context.Datxes.FindAsync(lichsuthue.Datxeid);
            if (datxe == null)
            {
                return BadRequest(new { message = "Ðon d?t xe không t?n t?i" });
            }

            lichsuthue.Ngaynhanxe = DateTime.Now; // ? S?A: DateTime.Now

            _context.Lichsuthues.Add(lichsuthue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLichsuthue), new { id = lichsuthue.Id }, lichsuthue);
        }

        /// <summary>
        /// C?p nh?t l?ch s? (tr? xe, phí phát sinh, dánh giá)
        /// </summary>
        /// <param name="id">ID l?ch s?</param>
        /// <param name="lichsuthue">Thông tin c?p nh?t</param>
        /// <returns>K?t qu? c?p nh?t</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLichsuthue(int id, Lichsuthue lichsuthue)
        {
            if (id != lichsuthue.Id)
            {
                return BadRequest(new { message = "ID không kh?p" });
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
                    return NotFound(new { message = "Không tìm th?y l?ch s?" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "C?p nh?t thành công" });
        }

        /// <summary>
        /// Ðánh giá chuy?n thuê
        /// </summary>
        /// <param name="id">ID l?ch s?</param>
        /// <param name="model">Thông tin dánh giá</param>
        /// <returns>K?t qu? dánh giá</returns>
        [HttpPatch("{id}/rate")]
        public async Task<IActionResult> RateLichsuthue(int id, [FromBody] RatingModel model)
        {
            var lichsuthue = await _context.Lichsuthues.FindAsync(id);
            if (lichsuthue == null)
            {
                return NotFound(new { message = "Không tìm th?y l?ch s?" });
            }

            if (model.Danhgia < 1 || model.Danhgia > 5)
            {
                return BadRequest(new { message = "Ðánh giá ph?i t? 1-5 sao" });
            }

            lichsuthue.Danhgia = model.Danhgia;
            lichsuthue.Nhanxet = model.Nhanxet;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ðánh giá thành công" });
        }

        /// <summary>
        /// Xóa l?ch s?
        /// </summary>
        /// <param name="id">ID l?ch s?</param>
        /// <returns>K?t qu? xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLichsuthue(int id)
        {
            var lichsuthue = await _context.Lichsuthues.FindAsync(id);
            if (lichsuthue == null)
            {
                return NotFound(new { message = "Không tìm th?y l?ch s?" });
            }

            _context.Lichsuthues.Remove(lichsuthue);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa l?ch s? thành công" });
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
