using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZXL.Repository
{
    public interface IPersistableRepository<TEntity> where TEntity : class
    {
        void Insert(TEntity entity);
        void Insert(params TEntity[] entities);
        void Insert(IEnumerable<TEntity> entities);
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        Task InsertAsync(params TEntity[] entities);
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        void Update(TEntity entity);
        void Update(params TEntity[] entities);
        void Update(IEnumerable<TEntity> entities);

        void Delete(object id);
        void Delete(TEntity entity);
        void Delete(params TEntity[] entities);
        void Delete(IEnumerable<TEntity> entities);

        Task BulkInsert(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task BulkUpdate(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task BulkDelete(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

    }
}
