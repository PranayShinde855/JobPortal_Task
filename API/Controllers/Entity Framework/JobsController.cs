using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Jobs;
using Services.UserServices;
using System;
using System.Threading.Tasks;

namespace API.Controllers.Jobs
{
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class JobsController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly IAppliedJobsService _appliedJobsService;
        public JobsController(IJobService jobService, IAppliedJobsService appliedJobsService, IUserService userService)
        {
            _jobService = jobService;
            _appliedJobsService = appliedJobsService;
        }

        [HttpGet]
        [Authorize(Policy ="AllAllowed")]
        [Route("Jobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            return Ok(await _jobService.GetAll());
        }

        [HttpGet]
        [Authorize(Policy ="AllAllowed")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            Job info = await _jobService.GetById(id);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPost]
        [Authorize(Policy ="User")]
        [Route("Job")]
        public async Task<IActionResult> AddJob(Job job)
        {
            var info = await _jobService.Add(job);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPut]
        [Authorize(Policy ="Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> UpdateJobById(Job job)
        {
            var info = await _jobService.Update(job);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpDelete]
        [Authorize(Policy ="Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var info = await _jobService.Delete(id);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPost]
        [Authorize(Policy ="User")]
        [Route("Jobs/Apply")]
        public async Task<IActionResult> ApplyJob(AppliedJob appliedJob)
        {
            if(ModelState.IsValid)
            {
                var chek = await _appliedJobsService.AlreadyAppliedToJob(appliedJob.JobId, appliedJob.AppliedBy);
                if(chek == false)
                {
                    try
                    {
                        var appliedJobStatus = await _appliedJobsService.Add(appliedJob);
                        return Ok(appliedJob);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return BadRequest("Al");
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("AppliedJobs/Recruiter")]
        [Authorize(Policy = "Recruiter")]
        public async Task<IActionResult> GetAllApplicantAppliedToMyJobs()
        {
            if (ModelState.IsValid)
            {
                return Ok(await _appliedJobsService.GetAllApplicantAppliedToMyJobs(UserId));
            }
            return BadRequest();
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        [Route("AppliedJobs/Applicant")]
        public async Task<IActionResult> GetAllJobsAppliedByMe()
        {
            return Ok(await _appliedJobsService.GetAllJobsAppliedByMe(UserId));
        }

        [HttpPut]
        [Authorize(Policy ="User")]
        [Route("AppliedJobs/Applicant/{id}")]
        public async Task<IActionResult> UpdateAppliedJob(int id, AppliedJob appliedJob)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _appliedJobsService.Update(id, appliedJob));
            }
            return BadRequest();
        }

        [HttpDelete]
        [Authorize(Policy ="User")]
        [Route("AppliedJobs/Applicant/{id}")]
        public async Task<IActionResult> DeleteAppliedJob(int id)
        {
            var result = await _appliedJobsService.Delete(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}