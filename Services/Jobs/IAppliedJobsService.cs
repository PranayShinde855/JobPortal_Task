using Models;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public interface IAppliedJobsService
    {
        Task<AppliedJob> Add(AppliedJob appliedJob);
        Task<bool> SendMailToUser(Users users, Job jobs);
        Task<bool> SendMailToRecruiter(Users users, Job jobs);
        Task<AppliedJob> Update(AppliedJob appliedJob);
        Task<AppliedJob> Delete(int id);
        Task<AppliedJob> GetById(int id);
        Task<IQueryable<AppliedJob>> GetAll();
        Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId);
        Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId);
        Task<bool> FireEmail(string to, string subject, string message);
    }
}
