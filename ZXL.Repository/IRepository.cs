namespace ZXL.Repository
{
    public interface IRepository<TEntity> : IQueryableRepository<TEntity>, IPersistableRepository<TEntity>  where TEntity : class
    {
    }
}
