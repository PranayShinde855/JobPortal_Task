using Database.Infrastructure;
using Models;

namespace Database.Repository
{
    public interface IOTPRepository : IRepository<OTP>
    {

    }
    public class OTPRepository : Repository<OTP>, IOTPRepository
    {
        public OTPRepository(DbContextModel dbContext) : base(dbContext)
        {
        }
    }
}
