using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace AlphaCert.RDS.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private const string dbNamePattern = @"(?<=[Dd]atabase=)\w+(?=;)"; 
        private readonly TContext _context;
        private bool disposed = false;
        private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public TContext DbContext => _context;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new Repository<TEntity>(_context);
            return _repositories[type] as IRepository<TEntity>;
        }

        public async Task<bool> ChangeDatabase(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));

            try
            {
                var connection = _context.Database.GetDbConnection();
                if (databaseName.CompareTo(connection.Database) == 0)
                {
                    // database name is the same as origianl
                    // case sensitive ??
                    return true;
                }

                if (connection.State.HasFlag(ConnectionState.Open))
                {
                    connection.ChangeDatabase(databaseName);
                }
                else
                {
                    connection.ConnectionString = Regex.Replace(connection.ConnectionString, dbNamePattern, databaseName, RegexOptions.Singleline);
                }
                await connection.OpenAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));

            return _context.Database.ExecuteSqlCommand(sql, parameters);
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException(nameof(sql));

            return await _context.Database.ExecuteSqlCommandAsync(sql, parameters).ConfigureAwait(false);
        }

        public int SaveChanges() => _context.SaveChanges();

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync().ConfigureAwait(false);

        public async Task<int> SaveChangesAsync(params IUnitOfWork<TContext>[] unitOfWorks)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var count = 0;
                    Array.ForEach(unitOfWorks, async (u) => count += await u.SaveChangesAsync().ConfigureAwait(false));
                    count += await SaveChangesAsync().ConfigureAwait(false);
                    scope.Complete();
                    return count;
                }
                catch (Exception ex)
                {
                    await SaveChangesAsync().ConfigureAwait(false);
                    throw ex;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _repositories?.Clear();
                    _context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
