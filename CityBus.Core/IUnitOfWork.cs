using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer.Entities;
using Core_Layer.IRepositries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
