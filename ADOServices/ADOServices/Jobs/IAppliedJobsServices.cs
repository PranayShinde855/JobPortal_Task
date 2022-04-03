using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.Jobs
{
    public interface IAppliedJobsServices
    {
        Task<bool> Add(int jobId, int userId);
        Task<EmailRequestDTO> EmailDetails(int jobId, int applicantId);
        Task<bool> SendMailToUser(EmailRequestDTO req);
        Task<bool> SendMailToRecruiter(EmailRequestDTO req);
        Task<bool> Delete(int id);
        Task<AppliedJob> GetById(int id);
        Task<IEnumerable<AppliedJobDTO>> GetAll();
        Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId);
        Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId);
        Task<bool> AlreadyAppliedToJob(int jobId, int userId);
    }
}
