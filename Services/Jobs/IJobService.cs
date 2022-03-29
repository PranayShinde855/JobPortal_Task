using Models;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public interface IJobService
    {
        Task<Job> Add(Job job);
        Task<Job> Update(Job job);
        Task<Job> Delete(int id);
        Task<Job> GetById(int id);
        Task<IQueryable<Job>> GetAll();
    }
}
