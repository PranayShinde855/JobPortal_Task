using Database.Infrastructure;
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
   