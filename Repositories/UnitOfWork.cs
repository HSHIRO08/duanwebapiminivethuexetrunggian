using Microsoft.EntityFrameworkCore.Storage;
using duanminiveprogresql.Models;

namespace duanminiveprogresql.Repositories
{
    /// <summary>
    /// Unit of Work Implementation
    /// Qu?n lý t?t c? repositories và transactions
    /// ??m b?o t?t c? operations dùng chung 1 DbContext
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<Type, object> _repositories;

        // Specific Repositories
        private IXeRepository? _xeRepository;
        private IDatXeRepository? _datXeRepository;
        private INguoiDungRepository? _nguoiDungRepository;
        private IKhachHangRepository? _khachHangRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        // Specific Repository Properties v?i Lazy Loading
        public IXeRepository Xes
        {
            get
            {
                if (_xeRepository == null)
                {
                    _xeRepository = new XeRepository(_context);
                }
                return _xeRepository;
            }
        }

        public IDatXeRepository DatXes
        {
            get
            {
                if (_datXeRepository == null)
                {
                    _datXeRepository = new DatXeRepository(_context);
                }
                return _datXeRepository;
            }
        }

        public INguoiDungRepository NguoiDungs
        {
            get
            {
                if (_nguoiDungRepository == null)
                {
                    _nguoiDungRepository = new NguoiDungRepository(_context);
                }
                return _nguoiDungRepository;
            }
        }

        public IKhachHangRepository KhachHangs
        {
            get
            {
                if (_khachHangRepository == null)
                {
                    _khachHangRepository = new KhachHangRepository(_context);
                }
                return _khachHangRepository;
            }
        }

        // Generic Repository cho các entities khác
        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_context);
            }
            
            return (IRepository<T>)_repositories[type];
        }

        // Save Changes
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Transaction Management
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Dispose
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
