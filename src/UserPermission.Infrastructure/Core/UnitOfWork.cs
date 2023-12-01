using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Threading.Tasks;
using UserPermission.Domain.Core;

namespace UserPermission.Infrastructure.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;
        private Hashtable repositories;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        public Task<int> Complete()
        {
            return this.context.SaveChangesAsync();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            this.repositories ??= new Hashtable();

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
