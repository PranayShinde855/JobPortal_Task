using Database.Repository;
using Microsoft.Extensions.Configuration;
using Models;
using Models.DTOs;
using Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public class AppliedJobsService : IAppliedJobsService
    {
        private readonly IAppliedJobsRepository _appliedJobsRepository;
        protected readonly IConfiguration _configuration;
        private readonly IJobService _jobService;
        private readonly IUserService _userService;
        public AppliedJobsService(IAppliedJobsRepository jobsRepository, IConfiguration configuration,
            IJobService jobService, IUserService userService)
        {
            _appliedJobsRepository = jobsRepository;
            _userService = userService;
            _configuration = configuration;
            _jobService = jobService;
        }

        public async Task<AppliedJob> Add(int jobId, int userId)
        {
            AppliedJob obj = new AppliedJob();
            obj.JobId = jobId;
            obj.AppliedBy = userId;
            obj.AppliedDate = DateTime.Now;
            obj.IsActive = true;
            await _appliedJobsRepository.Add(obj);

            var getUserDetails = await _userService.GetById(userId);
            var getJobDetails = await _jobService.GetById(jobId);
            var getRecruiterDetails = await _userService.GetById(getJobDetails.CreatedBy);
            await SendMailToRecruiter(getRecruiterDetails, getJobDetails);
            await SendMailToUser(getUserDetails, getJobDetails);
            return obj;
        }

        public async Task<AppliedJob> Delete(int id)
        {
            AppliedJob getJob = await _appliedJobsRepository.GetById(id);
            await _appliedJobsRepository.Delete(getJob);
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

        public async Task<bool> SendMailToUser(Users users , Job jobs)
        {
            var sub = $"{users.UserName} you have successfully applied to job.";
            var body = "";
            body += "<h4 style='font-size:1.1em'>Job Details </h4>";
            body += $"<h4 style='font-size:1.1em'>Title : {jobs.Title} </h4>";
            body += $"<h4 style='font-size:1.1em'>Description : {jobs.Description} </h4>";
            body += $"<h4 style='font-size:1.1em'>Skills : {jobs.Skills} </h4>";
            body += $"<h4 style='font-size:1.1em'>Location : {jobs.Location} </h4>";
            var execute = await _userService.ExecuteEmail(users.Email, sub, body);
            if (execute == true)
                return true;
            return false;
        }
        public async Task<bool> SendMailToRecruiter(Users users , Job jobs)
        {
            var sub = $"{users.UserName} have successfully applied to job you poasted.";
            var body = "";
            body += "<h4 style='font-size:1.1em'>Applicant Details </h4>";
            body += $"<h4 style='font-size:1.1em'>Name : {users.UserName} </h4>";
            body += $"<h4 style='font-size:1.1em'>Address : {users.Address} </h4>";
            body += $"<h4 style='font-size:1.1em'>Email : {users.Email} </h4>";
            body += "<h4 style='font-size:1.1em'>Job Details </h4>";
            body += $"<h4 style='font-size:1.1em'>Title : {jobs.Title} </h4>";
            body += $"<h4 style='font-size:1.1em'>Description : {jobs.Description} </h4>";
            body += $"<h4 style='font-size:1.1em'>Skills : {jobs.Skills} </h4>";
            body += $"<h4 style='font-size:1.1em'>Location : {jobs.Location} </h4>";
            var execute = await _userService.ExecuteEmail(users.Email, sub, body);
            if (execute == true)
                return true;
            return false;
        }
        public async Task<AppliedJob> Update(int id, AppliedJob appliedJob)
        {
            AppliedJob getAppliedJob = await _appliedJobsRepository.GetById(id); 
            if(getAppliedJob != null)
            {
                getAppliedJob.JobId = appliedJob.JobId;
                getAppliedJob.IsActive = appliedJob.IsActive;
                await _appliedJobsRepository.Update(getAppliedJob);
                return getAppliedJob;
            }
            return appliedJob;
        }

        public async Task<bool> AlreadyAppliedToJob(int jobId, int userId)
        {
            var check = await _appliedJobsRepository.GetDefault(x => x.JobId == jobId && x.AppliedBy == userId);
            if (check != null)
                return true;

            return false;
        }
    }
}