using System.Linq.Expressions;


namespace CleanArchitecture.DataAccess.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        IQueryable<T> GetAllQuery(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
    }
}
