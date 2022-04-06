using Database;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/Jobs")]
    [ApiController]
    public class JobsController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly IAppliedJobsService _appliedJobsService;

        private readonly IMemoryCache _memoryCache;
        private readonly DbContextModel _dbContext;
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
        [Route("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            Job info = await _jobService.GetById(id);
            if (info == null)
                return Ok(info);
            return NotFound(new SomeException("Job not found.", id));
        }

        [HttpPost]
        [Authorize(Policy = "Recruiter")]
        public async Task<IActionResult> AddJob(JobDTO job)
        {
            if (ModelState.IsValid)
            {
                var info = await _jobService.Add(job, UserId);
                if (info != null)
                    return Ok(new SomeException("Job added successfully.", info));
                return BadRequest(new SomeException("An error occured.", info));
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Authorize(Policy ="Recruiter")]
        [Route("{id}")]
        public async Task<IActionResult> UpdateJobById(JobDTO job, int id)
        {
            if (ModelState.IsValid)
            {
                var checkId = await _jobService.GetById(id);
                if (checkId != null)
                {
                    var info = await _jobService.Update(job, UserId, id);
                    if (info == null)
                        return Ok(new SomeException("Job updated successfully.", info));

                    return BadRequest(new SomeException("An error occured."));
                }
                return NotFound(new SomeException($"Job not found {id}."));
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Authorize(Policy ="Recruiter")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var checkJob = await _jobService.GetById(id);
            if (checkJob != null)
            {
                var info = await _jobService.Delete(id);
                if (info != null)
                    return Ok(new SomeException("Job deleted successfully.", info));

                return BadRequest("An error occured");
            }
            return NotFound(new SomeException($"Job not found {id}."));
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        [Route("Apply")]
        public async Task<IActionResult> ApplyJob(int jobId)
        {
            var checkJob = await _jobService.GetById(jobId);
            if (checkJob != null)
            {
                var chek = await _appliedJobsService.AlreadyAppliedToJob(jobId, UserId);
                if (chek == false)
                {
                    var appliedJobStatus = await _appliedJobsService.Add(jobId, UserId);
                    if (appliedJobStatus != null)
                        return Ok(new SomeException("Applied to job successfully.", appliedJobStatus));

                    return BadRequest(new SomeException("An error occured.", appliedJobStatus));
                }
                return BadRequest(new SomeException("Already applied to this job."));
            }
            return NotFound(new SomeException($"Job not found {jobId}"));
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("AppliedJobs")]
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
        [Route("Applicants/AppliedJobs")]
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
        [Route("AppliedJobs/{id}")]
        public async Task<IActionResult> DeleteAppliedJob(int id)
        {
            var checkJob = await _appliedJobsService.GetById(id);
            if (checkJob != null)
            {
                var result = await _appliedJobsService.Delete(checkJob);
                if (result != null)
                    return Ok(new SomeException("Job deleted successfully.", result));

                return BadRequest(new SomeException("An error occured.", result));
            }
            return NotFound(new SomeException($"Job not found {id}."));
        }
    }
}