using DataAccess.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Implementation của IKhachHangRepository
    /// Kế thừa Repository<Khachhang> và implement các methods đặc biệt
    /// </summary>
    public class KhachHangRepository : Repository<Khachhang>, IKhachHangRepository
    {
        public KhachHangRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Khachhang?> GetByUserIdAsync(int nguoidungId)
        {
            return await _dbSet
                .Include(k => k.Nguoidung)
                .FirstOrDefaultAsync(k => k.Nguoidungid == nguoidungId);
        }

        public async Task<Khachhang?> GetWithUserDetailsAsync(int id)
        {
            return await _dbSet
                .Include(k => k.Nguoidung)
                .Include(k => k.Datxes)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<IEnumerable<Khachhang>> GetVerifiedCustomersAsync()
        {
            return await _dbSet
                .Include(k => k.Nguoidung)
                .Where(k => k.Daxacthuc == true)
                .OrderBy(k => k.Nguoidung.Hoten)
                .ToListAsync();
        }

        public async Task<IEnumerable<Khachhang>> GetUnverifiedCustomersAsync()
        {
            return await _dbSet
                .Include(k => k.Nguoidung)
                .Where(k => k.Daxacthuc == false)
                .OrderBy(k => k.Nguoidung.Hoten)
                .ToListAsync();
        }
    }
}
