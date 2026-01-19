using DataAccess.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Implementation của INguoiDungRepository
    /// Kế thừa Repository<Nguoidung> và implement các methods đặc biệt
    /// </summary>
    public class NguoiDungRepository : Repository<Nguoidung>, INguoiDungRepository
    {
        public NguoiDungRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Nguoidung?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Nguoidung>> GetUsersByRoleAsync(string vaitro)
        {
            return await _dbSet
                .Where(u => u.Vaitro == vaitro)
                .OrderBy(u => u.Hoten)
                .ToListAsync();
        }

        public async Task<IEnumerable<Nguoidung>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.Trangthai == true)
                .OrderBy(u => u.Hoten)
                .ToListAsync();
        }
    }
}
