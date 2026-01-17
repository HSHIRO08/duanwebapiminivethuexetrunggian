using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using Domain.Entities;
using Domain.Interfaces;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Implementation của IDatXeRepository
    /// Kế thừa Repository<Datxe> và implement các methods đặc biệt
    /// </summary>
    public class DatXeRepository : Repository<Datxe>, IDatXeRepository
    {
        public DatXeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Datxe>> GetBookingsByCustomerAsync(int khachhangId)
        {
            return await _dbSet
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Where(d => d.Khachhangid == khachhangId)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();
        }

        public async Task<IEnumerable<Datxe>> GetBookingsByCarAsync(int xeId)
        {
            return await _dbSet
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Where(d => d.Xeid == xeId)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();
        }

        public async Task<IEnumerable<Datxe>> GetBookingsByStatusAsync(string trangthai)
        {
            return await _dbSet
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Where(d => d.Trangthai == trangthai)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();
        }

        public async Task<IEnumerable<Datxe>> GetRecentBookingsAsync(int count)
        {
            return await _dbSet
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .OrderByDescending(d => d.Ngaydat)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Datxe>> GetBookingsWithDetailsAsync()
        {
            return await _dbSet
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(d => d.Thanhtoans)
                .OrderByDescending(d => d.Ngaydat)
                .ToListAsync();
        }

        public async Task<Datxe?> GetBookingWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Xe)
                .Include(d => d.Khachhang)
                    .ThenInclude(k => k.Nguoidung)
                .Include(d => d.Thanhtoans)
                .Include(d => d.Lichsuthues)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbSet
                .Where(d => d.Trangthai == "Completed")
                .SumAsync(d => d.Tongtien);
        }

        public async Task<decimal> GetRevenueByMonthAsync(int year, int month)
        {
            return await _dbSet
                .Where(d => d.Trangthai == "Completed" &&
                           d.Ngaydat.Year == year &&
                           d.Ngaydat.Month == month)
                .SumAsync(d => d.Tongtien);
        }
    }
}
