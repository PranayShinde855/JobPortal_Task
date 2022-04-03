using Database.Infrastructure;
using Models;

namespace Database.Repository
{
    public interface IRoleRepository : IRepository<Roles>
    { 
    }
    public class RoleRepository : Repository<Roles>, IRoleRepository
    {
        public RoleRepository(DbContextModel dbContext) : base(dbContext)
        {
        }
    }
}
