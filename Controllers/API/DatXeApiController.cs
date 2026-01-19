using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        /// Lấy danh sách tất cả đơn đặt xe
        /// </summary>
        /// <returns>Danh sách đơn đặt xe</returns>
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
        /// Lấy thông tin chi tiết đơn đặt xe
        /// </summary>
        /// <param name="id">ID đơn đặt xe</param>
        /// <returns>Thông tin đơn đặt xe</returns>
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
        /// Lấy đơn đặt xe theo khách hàng
        /// </summary>
        /// <param name="khachhangId">ID khách hàng</param>
        /// <returns>Danh sách đơn đặt xe</returns>
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
        /// Tạo đơn đặt xe mới
        /// </summary>
        /// <param name="datxe">Thông tin đặt xe</param>
        /// <returns>Đơn đặt xe vừa tạo</returns>
        [HttpPost]
        public async Task<ActionResult<Datxe>> CreateDatXe(Datxe datxe)
        {
            // Kiểm tra xe có tồn tại
            var xe = await _context.Xes.FindAsync(datxe.Xeid);
            if (xe == null)
            {
                return BadRequest(new { message = "Xe không tồn tại" });
            }

            if (xe.Trangthai != "Available")
            {
                return BadRequest(new { message = "Xe không khả dụng" });
            }

            // Kiểm tra khách hàng
            var khachhang = await _context.Khachhangs.FindAsync(datxe.Khachhangid);
            if (khachhang == null)
            {
                return BadRequest(new { message = "Khách hàng không tồn tại" });
            }

            // Tính toán
            var songay = (int)(datxe.Ngayketthuc - datxe.Ngaybatdau).TotalDays;
            datxe.Songaythue = songay;
            datxe.Giatheongay = xe.Giathuetheongay;
            datxe.Tongtien = xe.Giathuetheongay * songay;
            datxe.Trangthai = "Pending";
            datxe.Ngaydat = DateTime.Now;  // ✅ SỬa: DateTime.Now

            _context.Datxes.Add(datxe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDatXe), new { id = datxe.Id }, datxe);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn đặt xe
        /// </summary>
        /// <param name="id">ID đơn đặt xe</param>
        /// <param name="trangthai">Trạng thái mới (Pending, Confirmed, Completed, Cancelled)</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string trangthai)
        {
            var datxe = await _context.Datxes.FindAsync(id);
            if (datxe == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn đặt xe" });
            }

            datxe.Trangthai = trangthai;

            if (trangthai == "Confirmed")
            {
                datxe.Ngayxacnhan = DateTime.Now;  // ✅ SỬa: DateTime.Now
            }
            else if (trangthai == "Completed")
            {
                datxe.Ngayhoanthanh = DateTime.Now;  // ✅ SỬa: DateTime.Now
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", trangthai = datxe.Trangthai });
        }

        /// <summary>
        /// Xóa đơn đặt xe
        /// </summary>
        /// <param name="id">ID đơn đặt xe</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatXe(int id)
        {
            var datxe = await _context.Datxes.FindAsync(id);
            if (datxe == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn đặt xe" });
            }

            _context.Datxes.Remove(datxe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa đơn đặt xe thành công" });
        }
    }
}
