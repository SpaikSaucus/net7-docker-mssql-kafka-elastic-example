using System;
using System.Collections.Generic;

namespace UserPermission.Domain.Core
{
    public interface IElasticsearchCRUD<T> where T : class
    {
        void Create(T entity);
        T Read(int entityId);
        T Read(T entity);
        Tuple<IEnumerable<T>, long> Read(int skip, int take);
        void Update(T entity);
        void Delete(int entityId);
    }
}
