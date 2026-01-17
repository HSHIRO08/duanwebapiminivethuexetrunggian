using Microsoft.EntityFrameworkCore;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Implementation c?a IXeRepository
    /// K? th?a Repository<Xe> và implement các methods ??c bi?t
    /// </summary>
    public class XeRepository : Repository<Xe>, IXeRepository
    {
        public XeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Xe>> GetAvailableCarsAsync()
        {
            return await _dbSet
                .Where(x => x.Trangthai == "Available")
                .OrderBy(x => x.Tenxe)
                .ToListAsync();
        }

        public async Task<IEnumerable<Xe>> GetCarsByTypeAsync(string loaixe)
        {
            return await _dbSet
                .Where(x => x.Loaixe == loaixe)
                .OrderBy(x => x.Tenxe)
                .ToListAsync();
        }

        public async Task<IEnumerable<Xe>> GetCarsByBrandAsync(string hangxe)
        {
            return await _dbSet
                .Where(x => x.Hangxe == hangxe)
                .OrderBy(x => x.Tenxe)
                .ToListAsync();
        }

        public async Task<IEnumerable<Xe>> SearchCarsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.Tenxe.Contains(searchTerm) || 
                           x.Hangxe.Contains(searchTerm) ||
                           x.Biensoxe.Contains(searchTerm))
                .OrderBy(x => x.Tenxe)
                .ToListAsync();
        }

        public async Task<IEnumerable<Xe>> GetCarsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(x => x.Giathuetheongay >= minPrice && x.Giathuetheongay <= maxPrice)
                .OrderBy(x => x.Giathuetheongay)
                .ToListAsync();
        }

        public async Task<bool> IsCarAvailableAsync(int xeId, DateTime startDate, DateTime endDate)
        {
            // Ki?m tra xe có t?n t?i và ?ang available không
            var car = await _dbSet.FindAsync(xeId);
            if (car == null || car.Trangthai != "Available")
                return false;

            // Ki?m tra xe có b? ??t trong kho?ng th?i gian này không
            var conflictingBookings = await _context.Datxes
                .Where(d => d.Xeid == xeId &&
                           d.Trangthai != "Cancelled" &&
                           ((d.Ngaybatdau <= endDate && d.Ngayketthuc >= startDate)))
                .AnyAsync();

            return !conflictingBookings;
        }
    }
}
