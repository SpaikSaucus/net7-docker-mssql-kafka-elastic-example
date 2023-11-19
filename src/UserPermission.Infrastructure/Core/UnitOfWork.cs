using UserPermission.Domain.Core;
using UserPermission.Infrastructure.EF;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace UserPermission.Infrastructure.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserPermissionDbContext context;
        private Hashtable repositories;

        public UnitOfWork(UserPermissionDbContext context)
        {
            this.context = context;
        }

        public Task<int> Complete()
        {
            return this.context.SaveChangesAsync();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (this.repositories == null)
                this.repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!this.repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                        .MakeGenericType(typeof(TEntity)), this.context);

                this.repositories.Add(type, repositoryInstance);
            }

            return (IRepository<TEntity>)this.repositories[type];
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
