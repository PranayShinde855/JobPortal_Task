using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Database.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContextModel _dbContext{ get; set; }

        public Repository(DbContextModel dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Add(T entity)
        {
            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> GetById(int Id)
        {
            return await _dbContext.Set<T>().FindAsync(Id);
        }

        public virtual async Task<IQueryable<T>> GetAll()
        {
            return _dbContext.Set<T>();
        }

        public async Task<T> Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> GetDefault(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Set<T>().Where(expression).FirstOrDefaultAsync();
        }
    }
}
