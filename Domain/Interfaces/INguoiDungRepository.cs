using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Repository interface cho Nguoidung entity
    /// Kế thừa IRepository và thêm các methods đặc biệt cho User
    /// </summary>
    public interface INguoiDungRepository : IRepository<Nguoidung>
    {
        Task<Nguoidung?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Nguoidung>> GetUsersByRoleAsync(string vaitro);
        Task<IEnumerable<Nguoidung>> GetActiveUsersAsync();
    }
}
