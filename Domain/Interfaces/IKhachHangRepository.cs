using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Repository interface cho Khachhang entity
    /// Kế thừa IRepository và thêm các methods đặc biệt cho Khách hàng
    /// </summary>
    public interface IKhachHangRepository : IRepository<Khachhang>
    {
        Task<Khachhang?> GetByUserIdAsync(int nguoidungId);
        Task<Khachhang?> GetWithUserDetailsAsync(int id);
        Task<IEnumerable<Khachhang>> GetVerifiedCustomersAsync();
        Task<IEnumerable<Khachhang>> GetUnverifiedCustomersAsync();
    }
}
