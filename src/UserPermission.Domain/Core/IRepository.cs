using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace UserPermission.Domain.Core
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindById(object id);
        IEnumerable<TEntity> Find(ISpecification<TEntity> specification = null);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        bool Contains(Expression<Func<TEntity, bool>> predicate);
        int Count(Expression<Func<TEntity, bool>> predicate);
    }
}
