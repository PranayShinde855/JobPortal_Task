using Models;
using Models.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public interface IJobService
    {
        Task<Job> Add(JobDTO job, int userId);
        Task<Job> Update(JobDTO job, int userId, int id);
        Task<Job> Delete(int id);
        Task<Job> GetById(int id);
        Task<IQueryable<Job>> GetAll();
    }
}
