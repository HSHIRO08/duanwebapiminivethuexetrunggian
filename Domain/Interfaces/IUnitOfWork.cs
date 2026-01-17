namespace Domain.Interfaces
{
    /// <summary>
    /// Unit of Work Interface
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IXeRepository Xes { get; }
        IDatXeRepository DatXes { get; }
        INguoiDungRepository NguoiDungs { get; }
        IKhachHangRepository KhachHangs { get; }
        
        // Generic Repository cho các entities khác
        IRepository<T> Repository<T>() where T : class;
        
        // Transaction Management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
