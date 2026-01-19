using DataAccess.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Implementation của IXeRepository
    /// Kế thừa Repository<Xe> và implement các methods đặc biệt
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
            // Kiểm tra xe có tồn tại và đang available không
            var car = await _dbSet.FindAsync(xeId);
            if (car == null || car.Trangthai != "Available")
                return false;

            // Kiểm tra xe có bị đặt trong khoảng thời gian này không
            var conflictingBookings = await _context.Datxes
                .Where(d => d.Xeid == xeId &&
                           d.Trangthai != "Cancelled" &&
                           ((d.Ngaybatdau <= endDate && d.Ngayketthuc >= startDate)))
                .AnyAsync();

            return !conflictingBookings;
        }
    }
}
