using duanminiveprogresql.Models;

namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Repository interface cho Nguoidung entity
    /// K? th?a IRepository và thêm các methods ??c bi?t cho User
    /// </summary>
    public interface INguoiDungRepository : IRepository<Nguoidung>
    {
        Task<Nguoidung?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Nguoidung>> GetUsersByRoleAsync(string vaitro);
        Task<IEnumerable<Nguoidung>> GetActiveUsersAsync();
    }
}
