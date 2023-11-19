using System;
using System.Threading.Tasks;

namespace UserPermission.Domain.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> Complete();
    }
}
