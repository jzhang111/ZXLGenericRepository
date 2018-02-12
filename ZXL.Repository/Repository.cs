using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AlphaCert.RDS.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IFluentQueryBuilder<TEntity> _queryBuilder;

        private const int DEFAULT_BATCH_SIZE = 5000;
        private const int DEFAULT_BATCH_TIMEOUT = 30; 

        public Repository(DbContext context)
        {
            _dbContext = context;
            _queryBuilder = new FluentQueryBuilder<TEntity>(context);
            _dbSet = _queryBuilder.DbSet;
        }

        public IFluentQueryBuilder<TEntity> Query => _queryBuilder;
        
        public void Insert(TEntity entity) => _dbSet.Add(entity);
        public void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);
        public void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);
        public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
            => await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        public Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);
        public async Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
            => await _dbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
       
        public void Update(TEntity entity) => _dbSet.Update(entity);
        public void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);
        public void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

        /// <summary>
        /// Delete for the PK value
        /// </summary>
        /// <param name="id">The PK value</param>
        public void Delete(object id)
        {
            var key = _dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeof(TEntity).GetTypeInfo().GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = _dbSet.Find(id);
                if (entity != null) _dbSet.Remove(entity);
            }
        }
        public void Delete(TEntity entity) => _dbSet.Remove(entity);
        public void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);
        public void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        // Bulk operation to be implemented 
        public async Task BulkInsert(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
            => await _dbContext.BulkInsertAsync(entities, o => 
                {
                    o.BatchSize = DEFAULT_BATCH_SIZE;
                    o.BatchTimeout = DEFAULT_BATCH_TIMEOUT;
                }).ConfigureAwait(false);

        public Task BulkUpdate(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
            => throw new NotImplementedException();
        public Task BulkDelete(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
            => throw new NotImplementedException();
    }
}
