using Database;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Models.DTOs;
using Services.Jobs;
using Services.UserServices;
using System;
using System.Collections.Generic;
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

        protected readonly IMemoryCache _memoryCache;
        protected readonly DbContextModel _dbContext;
        public JobsController(IJobService jobService, IAppliedJobsService appliedJobsService, IUserService userService,
            IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _jobService = jobService;
            _appliedJobsService = appliedJobsService;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Policy ="AllAllowed")]
        [Route("Jobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Models.Job> result))
            {
                result = await _jobService.GetAll();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);
            }
            return Ok(result);
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
        [Authorize(Policy = "Recruiter")]
        //[Route("Job")]
        public async Task<IActionResult> AddJob(JobDTO job)
        {
            if (ModelState.IsValid)
            {
                var info = await _jobService.Add(job, UserId);
                if (info != null)
                    return Ok(info);
                return BadRequest(new SomeException("An error occured.", info));
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Authorize(Policy ="Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> UpdateJobById(JobDTO job, int id)
        {
            var info = await _jobService.Update(job, UserId, id);
            if (info == null)
                return Ok(info);
            return NotFound(new SomeException($"Job not found {id}."));
        }

        [HttpDelete]
        [Authorize(Policy ="Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var info = await _jobService.Delete(id);
            if (info != null)
                return Ok(info);
            return NotFound(new SomeException($"Job not found {id}."));
        }

        [HttpPost]
        [Authorize(Policy ="User")]
        [Route("Jobs/Apply")]
        public async Task<IActionResult> ApplyJob(int jobId)
        {
            if(ModelState.IsValid)
            {
                var chek = await _appliedJobsService.AlreadyAppliedToJob(jobId, UserId);
                if(chek == false)
                {
                    try
                    {
                        var appliedJobStatus = await _appliedJobsService.Add(jobId, UserId);
                        return Ok(appliedJobStatus);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return BadRequest(new SomeException("Already applied to this job."));
            }
            return BadRequest(new SomeException("Please fill all the details", ModelState));
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("AppliedJobs/Recruiter")]        
        public async Task<IActionResult> GetAllApplicantAppliedToMyJobs()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppliedJobDTO> result ))
            {
                result = await _appliedJobsService.GetAllApplicantAppliedToMyJobs(UserId);
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        [Route("AppliedJobs/Applicant")]
        public async Task<IActionResult> GetAllJobsAppliedByMe()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppliedJobDTO> result))
            {
                result = await _appliedJobsService.GetAllJobsAppliedByMe(UserId);
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);
            }
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Policy ="User")]
        [Route("AppliedJobs/Applicant/{id}")]
        public async Task<IActionResult> DeleteAppliedJob(int id)
        {
            var result = await _appliedJobsService.Delete(id);
            if (result != null)
                return Ok(result);

            return NotFound(new SomeException($"Job not found {id}."));
        }
    }
}