using Database.Infrastructure;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
