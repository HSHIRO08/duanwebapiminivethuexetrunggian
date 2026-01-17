namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Unit of Work Interface
    /// Qu?n lý t?t c? repositories và ??m b?o transaction consistency
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
