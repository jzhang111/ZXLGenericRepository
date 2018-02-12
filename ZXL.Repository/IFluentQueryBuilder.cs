using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZXL.Repository
{
    public interface IFluentQueryBuilder<TEntity> where TEntity: class
    {
        DbSet<TEntity> DbSet { get; } 
        int Count(Expression<Func<TEntity, bool>> predicate = null);
        TEntity FindBy(params object[] keyValues);
        Task<TEntity> FindByAsync(object[] keyValues);
        Task<TEntity> FindByAsync(object[] keyValues, CancellationToken cancellationToken = default(CancellationToken));
        IFluentQueryBuilder<TEntity> All();
        IFluentQueryBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

        IFluentQueryBuilder<TEntity> FromSql(string sql, params object[] parameters);

        IFluentQueryBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> order);
        IFluentQueryBuilder<TEntity> OrderByDescending(Expression<Func<TEntity, object>> order);

        /// <summary>
        /// This will include OrderBy, ThenBy, OrderByDescending, ThenByDescending 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFluentQueryBuilder<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IFluentQueryBuilder<TEntity> Skip(int count);
        IFluentQueryBuilder<TEntity> Take(int count);
        IFluentQueryBuilder<TEntity> Include(string include);
        IFluentQueryBuilder<TEntity> Include(Expression<Func<TEntity, object>> include);

        /// <summary>
        /// this will implement ThenInclude
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IFluentQueryBuilder<TEntity> Include(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null);

        IEnumerable<TEntity> AsEnumerable();
        Task<IEnumerable<TEntity>> AsEnumerableAsync();

        IQueryable<TEntity> AsQueryable();
    }
}
