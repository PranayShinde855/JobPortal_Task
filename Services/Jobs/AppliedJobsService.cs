using Database.Repository;
using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public class AppliedJobsService : IAppliedJobsService
    {
        private readonly IAppliedJobsRepository _appliedJobsRepository;
        public AppliedJobsService(IAppliedJobsRepository jobsRepository)
        {
            _appliedJobsRepository = jobsRepository;
        }

        public async Task<AppliedJob> Add(AppliedJob appliedJob)
        {
            AppliedJob obj = new AppliedJob();
            obj.JobId = appliedJob.JobId;
            obj.AppliedBy = appliedJob.AppliedBy;
            _appliedJobsRepository.Add(obj);
            return obj;
        }

        public async Task<AppliedJob> Delete(int id)
        {
            AppliedJob getJob = await _appliedJobsRepository.GetById(id);
            _appliedJobsRepository.Delete(getJob);
            return getJob;
        }

        public async Task<IQueryable<AppliedJob>> GetAll()
        {
            return await _appliedJobsRepository.GetAll();
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId)
        {
            return await _appliedJobsRepository.GetAllApplicantAppliedToMyJobs(userId);
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId)
        {
            return await _appliedJobsRepository.GetAllJobsAppliedByMe(userId);
        }

        public async Task<AppliedJob> GetById(int id)
        {
            return await _appliedJobsRepository.GetById(id);
        }

        public async Task<AppliedJob> Update(AppliedJob appliedJob)
        {
            AppliedJob getAppliedJob = await _appliedJobsRepository.GetById(appliedJob.Id); 
            if(getAppliedJob != null)
            {
                getAppliedJob.JobId = appliedJob.JobId;
                getAppliedJob.AppliedBy = appliedJob.AppliedBy;
                _appliedJobsRepository.Update(getAppliedJob);
                return getAppliedJob;
            }
            return appliedJob;
        }
    }
}