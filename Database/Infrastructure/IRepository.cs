using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Database.Infrastructure
{
    public interface IRepository<T> where T : class
    {

        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> GetById(int Id);
        Task<IQueryable<T>> GetAll();
        Task<T> Delete(T entity);
        Task<T> GetDefault(Expression<Func<T, bool>> expression);
    }
}
