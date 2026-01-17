using duanminiveprogresql.Models;

namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Repository interface cho Khachhang entity
    /// K? th?a IRepository và thêm các methods ??c bi?t cho Khách hàng
    /// </summary>
    public interface IKhachHangRepository : IRepository<Khachhang>
    {
        Task<Khachhang?> GetByUserIdAsync(int nguoidungId);
        Task<Khachhang?> GetWithUserDetailsAsync(int id);
        Task<IEnumerable<Khachhang>> GetVerifiedCustomersAsync();
        Task<IEnumerable<Khachhang>> GetUnverifiedCustomersAsync();
    }
}
