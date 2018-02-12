using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ZXL.Repository
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        TContext DbContext { get; }
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<bool> ChangeDatabase(string databaseName);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(params IUnitOfWork<TContext>[] unitOfWorks);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

    }
}


