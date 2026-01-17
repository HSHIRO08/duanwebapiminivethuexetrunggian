using duanminiveprogresql.Models;

namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Repository interface cho Xe entity
    /// K? th?a IRepository và thêm các methods ??c bi?t cho Xe
    /// </summary>
    public interface IXeRepository : IRepository<Xe>
    {
        Task<IEnumerable<Xe>> GetAvailableCarsAsync();
        Task<IEnumerable<Xe>> GetCarsByTypeAsync(string loaixe);
        Task<IEnumerable<Xe>> GetCarsByBrandAsync(string hangxe);
        Task<IEnumerable<Xe>> SearchCarsAsync(string searchTerm);
        Task<IEnumerable<Xe>> GetCarsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> IsCarAvailableAsync(int xeId, DateTime startDate, DateTime endDate);
    }
}
