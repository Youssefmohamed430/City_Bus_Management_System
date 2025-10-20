using Core_Layer.IRepositries;

namespace Core_Layer
{
    public interface IUnitOfWork : IDisposable
    {
        public IScheduleRepository Schedules { get; }
        public IWalletRepository Wallets { get; }
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        Task<int> SaveAsync();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
