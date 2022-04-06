using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public interface IAppliedJobsService
    {
        Task<AppliedJob> Add(int jobId, int userId);
        Task<bool> SendMailToUser(Users user, Job job);
        Task<bool> SendMailToRecruiter(Users user, Job job);
        Task<AppliedJob> Delete(AppliedJob job);
        Task<AppliedJob> GetById(int id);
        Task<IQueryable<AppliedJob>> GetAll();
        Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId);
        Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId);
        Task<bool> AlreadyAppliedToJob(int jobId, int userId);
    }
}
