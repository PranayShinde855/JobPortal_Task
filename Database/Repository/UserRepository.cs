using Database.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Database.Repository
{
    public interface IUserRepository : IRepository<Users>
    {

    }
    public class UserRepository : Repository<Users>, IUserRepository
    {
        public UserRepository(DbContextModel dbContextModel) : base(dbContextModel)
        {

        }
    }
}
   