using Database;
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

        [Authorize(Policy = "User")]
        [HttpGet]
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