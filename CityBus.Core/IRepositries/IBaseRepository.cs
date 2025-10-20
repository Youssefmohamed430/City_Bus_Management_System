using System.Linq.Expressions;

namespace Core_Layer.IRepositries
{
    public interface IBaseRepository<T>  where T : class
    {
        T GetById(int id);
        T GetByUserId(string id);
        IQueryable<TDto> GetAll<TDto>();
        T Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        TDto Find<TDto>(Expression<Func<T, bool>> criteria, string[] includes = null);
        IQueryable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null);
        IQueryable<TDto> FindAll<TDto>(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        void DeleteAsync(T entity);
    }
}
