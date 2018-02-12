namespace ZXL.Repository
{
    public interface IQueryableRepository<TEntity> where TEntity : class
    {
        IFluentQueryBuilder<TEntity> Query { get; }
    }
}
