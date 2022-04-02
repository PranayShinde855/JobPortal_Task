using ADOServices.ADOServices.Jobs;
using Database.ADO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Services.ADOServices.Jobs;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADOJobsController : BaseController
    {
        protected readonly IJobsServices _jobsServiceADO;
        protected readonly IAppliedJobsServices _appliedJobsServicesADO;
        public ADOJobsController(IJobsServices jobsServiceADO, IAppliedJobsServices appliedJobsServicesADO)
        {
            _jobsServiceADO = jobsServiceADO;
            _appliedJobsServicesADO = appliedJobsServicesADO;
        }

        [HttpGet]
        ////[Authorize(Policy = "AllAllowed")]
        [AllowAnonymous]
        [Route("Jobs")]
        public async Task<ActionResult> GetAllJobs()
        {
            return Ok(await _jobsServiceADO.GetAll());
        }


        [HttpGet]
        //[Authorize(Policy = "AllAllowed")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var info = await _jobsServiceADO.GetById(id);
            if (info != null)
                return Ok(info);
            return NotFound(id);
        }

        [HttpPost]
        ////[Authorize(Policy = "User")]
        [Route("Job")]
        public async Task<IActionResult> AddJob(JobDTO job)
            {
            if (ModelState.IsValid)
            {
                var info = await _jobsServiceADO.Add(job/*, UserId*/);
                if (info == true)
                    return Ok(info);
                return BadRequest("Not Saved");
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        //[Authorize(Policy = "Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> UpdateJobById(JobDTO job, int id)
            {
            var checkId = await _jobsServiceADO.GetById(id);
            if(checkId != null)
            {
                var info = await _jobsServiceADO.Update(job/*, UserId*/, id);
                if (info == true)
                    return Ok(info);
                return BadRequest();
            }
            return NotFound();
        }

        [HttpDelete]
        //[Authorize(Policy = "Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var info = await _jobsServiceADO.Delete(id);
            if (info == true)
                return Ok(info);
            return NotFound();
        }

        [HttpPost]
        //[Authorize(Policy = "User")]
        [Route("Jobs/Apply")]
        public async Task<IActionResult> ApplyJob(int jobId)
        {
            return Ok(await _appliedJobsServicesADO.Add(jobId, UserId));
        }

        [HttpGet]
        [Route("AppliedJobs/Recruiter")]
        //[Authorize(Policy = "Recruiter")]
        public async Task<IActionResult> GetAllApplicantAppliedToMyJobs()
        {
            if (ModelState.IsValid)
            {
                return Ok(await _appliedJobsServicesADO.GetAllApplicantAppliedToMyJobs(UserId));
            }
            return BadRequest();
        }

        //[Authorize(Policy = "User")]
        [HttpGet]
        [Route("AppliedJobs/Applicant")]
        public async Task<IActionResult> GetAllJobsAppliedByMe()
        {
            return Ok(await _appliedJobsServicesADO.GetAllJobsAppliedByMe(UserId));
        }

        [HttpDelete]
        //[Authorize(Policy = "User")]
        [Route("AppliedJobs/Applicant/{id}")]
        public async Task<IActionResult> DeleteAppliedJob(int id)
        {
            var checkJob = await _appliedJobsServicesADO.GetById(id);
            if (checkJob != null)
            {
                return Ok(await _appliedJobsServicesADO.Delete(id));
            }
            return NotFound(id);
        }

    }
}
