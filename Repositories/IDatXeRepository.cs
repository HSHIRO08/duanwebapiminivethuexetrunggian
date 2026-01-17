using duanminiveprogresql.Models;

namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Repository interface cho Datxe entity
    /// K? th?a IRepository và thêm các methods ??c bi?t cho ??t xe
    /// </summary>
    public interface IDatXeRepository : IRepository<Datxe>
    {
        Task<IEnumerable<Datxe>> GetBookingsByCustomerAsync(int khachhangId);
        Task<IEnumerable<Datxe>> GetBookingsByCarAsync(int xeId);
        Task<IEnumerable<Datxe>> GetBookingsByStatusAsync(string trangthai);
        Task<IEnumerable<Datxe>> GetRecentBookingsAsync(int count);
        Task<IEnumerable<Datxe>> GetBookingsWithDetailsAsync();
        Task<Datxe?> GetBookingWithDetailsAsync(int id);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByMonthAsync(int year, int month);
    }
}
