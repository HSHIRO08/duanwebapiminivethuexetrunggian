using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Controllers
{
    public class XeController : Controller
    {
        private readonly AppDbContext _context;

        public XeController(AppDbContext context)
        {
            _context = context;
        }

        // Danh sách xe
        public async Task<IActionResult> Index(string search, string loaixe, string hangxe, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Xes.Where(x => x.Trangthai == "Available");

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Tenxe.Contains(search) || 
                                        x.Hangxe.Contains(search) ||
                                        x.Biensoxe.Contains(search));
            }

            // Lọc theo loại xe
            if (!string.IsNullOrEmpty(loaixe))
            {
                query = query.Where(x => x.Loaixe == loaixe);
            }

            // Lọc theo hãng xe
            if (!string.IsNullOrEmpty(hangxe))
            {
                query = query.Where(x => x.Hangxe == hangxe);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Giathuetheongay >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Giathuetheongay <= maxPrice.Value);
            }

            var cars = await query.OrderBy(x => x.Tenxe).ToListAsync();

            // Lấy danh sách loại xe và hãng xe để hiển thị filter
            ViewBag.LoaiXeList = await _context.Xes.Select(x => x.Loaixe).Distinct().ToListAsync();
            ViewBag.HangXeList = await _context.Xes.Select(x => x.Hangxe).Distinct().ToListAsync();

            return View(cars);
        }

        // Chi tiết xe
        public async Task<IActionResult> Details(int id)
        {
            var xe = await _context.Xes
                .Include(x => x.Lichsuthues)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (xe == null)
            {
                return NotFound();
            }

            // Tính rating trung bình
            var avgRating = await _context.Lichsuthues
                .Where(l => l.Xeid == id && l.Danhgia.HasValue)
                .AverageAsync(l => (double?)l.Danhgia) ?? 0;

            ViewBag.AvgRating = avgRating;

            // Lấy đánh giá gần nhất
            var recentReviews = await _context.Lichsuthues
                .Where(l => l.Xeid == id && !string.IsNullOrEmpty(l.Nhanxet))
                .Include(l => l.Khachhang)
                .ThenInclude(k => k.Nguoidung)
                .OrderByDescending(l => l.Ngaytraxe)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentReviews = recentReviews;

            return View(xe);
        }
    }
}
