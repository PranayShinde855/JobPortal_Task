using ADOServices.ADOServices.Jobs;
using Database;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Using_ADO.NET
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADOJobsController : BaseController
    {
        protected readonly IJobsServices _jobsServiceADO;
        protected readonly IAppliedJobsServices _appliedJobsServicesADO;
        protected readonly ILogger<ADOJobsController> _logger;
        protected readonly IMemoryCache _memoryCache;
        protected readonly DbContextModel _dbContext;
        public ADOJobsController(IJobsServices jobsServiceADO, IAppliedJobsServices appliedJobsServicesADO, ILogger<ADOJobsController> logger
            , IMemoryCache memoryCache, DbContextModel dbContext)
        {
            _jobsServiceADO = jobsServiceADO;
            _appliedJobsServicesADO = appliedJobsServicesADO;
            _logger = logger;
            _memoryCache = memoryCache;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Policy = "AllAllowed")]
        [AllowAnonymous]
        [Route("Jobs")]
        public async Task<ActionResult> GetAllJobs()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Models.Job> result))
            {
                result = await _jobsServiceADO.GetAll();
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
        [Authorize(Policy = "AllAllowed")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var info = await _jobsServiceADO.GetById(id);
            if (info != null)
                return Ok(info);
            return NotFound(new SomeException($"Job not found {id}"));
        }

        [HttpPost]
        [Authorize(Policy = "Recruiter")]
        [Route("Job")]
        public async Task<IActionResult> AddJob(JobDTO job)
            {
            if (ModelState.IsValid)
            {
                var info = await _jobsServiceADO.Add(job, UserId);
                if (info == true)
                    return Ok(new SomeException("Job added successfully.", info));
                return BadRequest(new SomeException("An error occured.", info));
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Authorize(Policy = "Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> UpdateJobById(JobDTO job, int id)
        {
            var checkId = await _jobsServiceADO.GetById(id);
            if (checkId != null)
            {
                var info = await _jobsServiceADO.Update(job, UserId, id);
                if (info == true)
                    return Ok(new SomeException("Job updated successfully.", info));
                return BadRequest(new SomeException("An error occured.", info));
            }
            return NotFound(new SomeException($"Job not found {id}"));
        }

        [HttpDelete]
        [Authorize(Policy = "Recruiter")]
        [Route("Jobs/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var info = await _jobsServiceADO.Delete(id);
            if (info == true)
                return Ok(info);
            return NotFound(new SomeException($"Job not found {id}"));
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        [Route("Jobs/Apply")]
        public async Task<IActionResult> ApplyJob(int jobId)
        {
            var checkJob = await _jobsServiceADO.GetById(jobId);
            if(checkJob != null)
            {
                var checkAlreadApplied = await _appliedJobsServicesADO.AlreadyAppliedToJob(jobId, UserId);
                if (checkAlreadApplied == false)
                {
                    var info = await _appliedJobsServicesADO.Add(jobId, UserId);
                    return Ok(new SomeException("Applied to  job successfully", info));
                }
                return BadRequest(new SomeException($"Already applied to job {jobId}"));
            }
            return NotFound(new SomeException($"Job not found {jobId}"));
        }

        [HttpGet]
        [Route("AppliedJobs/Recruiter")]
        [Authorize(Policy = "Recruiter")]
        public async Task<IActionResult> GetAllApplicantAppliedToMyJobs()
        {
            var cacheKey = "result";
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppliedJobDTO> result))
            {
                  result = await _appliedJobsServicesADO.GetAllApplicantAppliedToMyJobs(UserId);
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
                  result = await _appliedJobsServicesADO.GetAllJobsAppliedByMe(UserId);
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
        [Authorize(Policy = "User")]
        [Route("AppliedJobs/Applicant/{id}")]
        public async Task<IActionResult> DeleteAppliedJob(int id)
        {
            var checkJob = await _appliedJobsServicesADO.GetById(id);
            if (checkJob != null)
            {
                var result = await _appliedJobsServicesADO.Delete(id);
                if (result == true)
                    return Ok(new SomeException("Job deleted successfully", result));

                return BadRequest(new SomeException("An error occured.", result));
            }
            return NotFound(new SomeException($"Job not found {id}"));
        }

    }
}
