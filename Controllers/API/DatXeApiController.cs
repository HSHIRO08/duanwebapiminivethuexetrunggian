using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;

namespace duanminiveprogresql.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatXeApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DatXeApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// L?y danh sách t?t c? don d?t xe
        /// </summary>
        /// <returns>Danh sách don d?t xe</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDatXes()
        {
            return await _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Select(d => new
                {
                    d.Id,
                    d.Xeid,
                    XeTen = d.Xe.Tenxe,
                    d.Khachhangid,
                    KhachhangTen = d.Khachhang.Nguoidung.Hoten,
                    d.Ngaybatdau,
                    d.Ngayketthuc,
                    d.Songaythue,
                    d.Tongtien,
                    d.Trangthai,
                    d.Ngaydat
                })
                .ToListAsync();
        }

        /// <summary>
        /// L?y thông tin chi ti?t don d?t xe
        /// </summary>
        /// <param name="id">ID don d?t xe</param>
        /// <returns>Thông tin don d?t xe</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Datxe>> GetDatXe(int id)
        {
            var datxe = await _context.Datxes
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(d => d.Thanhtoans)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (datxe == null)
            {
                return NotFound(new { message = "Không tìm th?y don d?t xe" });
            }

            return datxe;
        }

        /// <summary>
        /// L?y don d?t xe theo khách hàng
        /// </summary>
        /// <param name="khachhangId">ID khách hàng</param>
        /// <returns>Danh sách don d?t xe</returns>
        [HttpGet("khachhang/{khachhangId}")]
        public async Task<ActionResult<IEnumerable<Datxe>>> GetDatXeByKhachhang(int khachhangId)
        {
            return await _context.Datxes
                .Include(d => d.Xe)
                .Where(d => d.Khachhangid == khachhangId)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();
        }

        /// <summary>
        /// T?o don d?t xe m?i
        /// </summary>
        /// <param name="datxe">Thông tin d?t xe</param>
        /// <returns>Ðon d?t xe v?a t?o</returns>
        [HttpPost]
        public async Task<ActionResult<Datxe>> CreateDatXe(Datxe datxe)
        {
            // Ki?m tra xe có t?n t?i
            var xe = await _context.Xes.FindAsync(datxe.Xeid);
            if (xe == null)
            {
                return BadRequest(new { message = "Xe không t?n t?i" });
            }

            if (xe.Trangthai != "Available")
            {
                return BadRequest(new { message = "Xe không kh? d?ng" });
            }

            // Ki?m tra khách hàng
            var khachhang = await _context.Khachhangs.FindAsync(datxe.Khachhangid);
            if (khachhang == null)
            {
                return BadRequest(new { message = "Khách hàng không t?n t?i" });
            }

            // Tính toán
            var songay = (int)(datxe.Ngayketthuc - datxe.Ngaybatdau).TotalDays;
            datxe.Songaythue = songay;
            datxe.Giatheongay = xe.Giathuetheongay;
            datxe.Tongtien = xe.Giathuetheongay * songay;
            datxe.Trangthai = "Pending";
            datxe.Ngaydat = DateTime.Now;  // ? S?A: DateTime.Now

            _context.Datxes.Add(datxe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDatXe), new { id = datxe.Id }, datxe);
        }

        /// <summary>
        /// C?p nh?t tr?ng thái don d?t xe
        /// </summary>
        /// <param name="id">ID don d?t xe</param>
        /// <param name="trangthai">Tr?ng thái m?i (Pending, Confirmed, Completed, Cancelled)</param>
        /// <returns>K?t qu? c?p nh?t</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string trangthai)
        {
            var datxe = await _context.Datxes.FindAsync(id);
            if (datxe == null)
            {
                return NotFound(new { message = "Không tìm th?y don d?t xe" });
            }

            datxe.Trangthai = trangthai;

            if (trangthai == "Confirmed")
            {
                datxe.Ngayxacnhan = DateTime.Now;  // ? S?A: DateTime.Now
            }
            else if (trangthai == "Completed")
            {
                datxe.Ngayhoanthanh = DateTime.Now;  // ? S?A: DateTime.Now
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "C?p nh?t tr?ng thái thành công", trangthai = datxe.Trangthai });
        }

        /// <summary>
        /// Xóa don d?t xe
        /// </summary>
        /// <param name="id">ID don d?t xe</param>
        /// <returns>K?t qu? xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatXe(int id)
        {
            var datxe = await _context.Datxes.FindAsync(id);
            if (datxe == null)
            {
                return NotFound(new { message = "Không tìm th?y don d?t xe" });
            }

            _context.Datxes.Remove(datxe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa don d?t xe thành công" });
        }
    }
}
