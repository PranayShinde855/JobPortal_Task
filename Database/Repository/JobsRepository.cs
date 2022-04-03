using Database.Infrastructure;
using Models;

namespace Database.Repository
{
    public interface IJobsRepository : IRepository<Job>
    {

    }
    public class JobsRepository : Repository<Job>, IJobsRepository
    {
        public JobsRepository(DbContextModel dbContextModel) : base(dbContextModel)
        {

        }
    }
}
