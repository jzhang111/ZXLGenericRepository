using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Z.Repository
{
    public class FluentQueryBuilder<TEntity> : IFluentQueryBuilder<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        private static IQueryable<TEntity> _query;

        public FluentQueryBuilder(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
            _query = _dbSet.AsNoTracking();

        }

        //protected DbSet<TEntity> DbSet => _dbSet;

        DbSet<TEntity> IFluentQueryBuilder<TEntity>.DbSet => _dbSet;

        public IFluentQueryBuilder<TEntity> All() => this;
        
        public IQueryable<TEntity> AsQueryable() => _query;
        public IEnumerable<TEntity> AsEnumerable() => _query.ToList();
        
        public async Task<IEnumerable<TEntity>> AsEnumerableAsync() 
            => await _query.ToListAsync().ConfigureAwait(false);
        
        public int Count(Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? _query.Count() : _query.Count(predicate);

        public TEntity FindBy(params object[] keyValues) => _dbSet.Find(keyValues);

        public async Task<TEntity> FindByAsync(object[] keyValues) 
            => await _dbSet.FindAsync(keyValues).ConfigureAwait(false);

        public async Task<TEntity> FindByAsync(object[] keyValues, CancellationToken cancellationToken = default(CancellationToken))
            => await _dbSet.FindAsync(keyValues, cancellationToken).ConfigureAwait(false);
        
        public TEntity FirstOrDefault(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate) 
            => predicate == null ? _query.FirstOrDefault() : _query.FirstOrDefault(predicate);

        public Task<TEntity> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
            => predicate == null ? _query.FirstOrDefaultAsync() : _query.FirstOrDefaultAsync(predicate);

        public IFluentQueryBuilder<TEntity> Include(string include)
        {
            _query = _query.Include(include);
            return this;
        }

        public IFluentQueryBuilder<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            _query = _query.Include(include);
            return this;
        }

        public IFluentQueryBuilder<TEntity> Include(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include)
        {
            _query = include(_query);
            return this;
        }

        public IFluentQueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> order)
        {
            _query = _query.OrderBy(order);
            return this;
        }

        public IFluentQueryBuilder<TEntity> OrderByDescending(Expression<Func<TEntity, object>> order)
        {
            _query = _query.OrderByDescending(order);
            return this;
        }

        public IFluentQueryBuilder<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _query = orderBy(_query);
            return this;
        }

        public IFluentQueryBuilder<TEntity> Skip(int count)
        {
            _query = _query.Skip(count);
            return this;
        }

        public IFluentQueryBuilder<TEntity> Take(int count)
        {
            _query = _query.Take(count);
            return this;
        }

        public IFluentQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        public IFluentQueryBuilder<TEntity> FromSql(string sql, params object[] parameters)
        {
            _query = _dbSet.FromSql(sql, parameters);
            return this;
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _query.Select(selector);
        }
    }
}
