﻿using ADOServices.ADOServices.Jobs;
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
    [Route("api/ADO/Jobs")]
    [ApiController]
    public class ADOJobsController : BaseController
    {
        private readonly IJobsServices _jobsServiceADO;
        private readonly IAppliedJobsServices _appliedJobsServicesADO;
        private readonly ILogger<ADOJobsController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly DbContextModel _dbContext;
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
        public async Task<ActionResult> GetAllJobs()
        {
            var cacheKey = string.Empty;
            object isExist = null;
            IEnumerable<Models.Job> result = null;
            if (UserId != 0)
            {
                cacheKey = "User_" + UserId.ToString();
                isExist = _memoryCache.Get(cacheKey);
            }

            if (isExist == null)
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
            else
            {
                result = (IEnumerable<Models.Job>)isExist;
            }
            return Ok(result);
        }


        [HttpGet]
        [Authorize(Policy = "AllAllowed")]
        [Route("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var info = await _jobsServiceADO.GetById(id);
            if (info != null)
                return Ok(info);

            return NotFound(new SomeException($"Job not found {id}"));
        }

        [HttpPost]
        [Authorize(Policy = "Recruiter")]
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
        [Route("{id}")]
        public async Task<IActionResult> UpdateJobById(JobDTO job, int id)
        {
            if (ModelState.IsValid)
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
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Authorize(Policy = "Recruiter")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var checkJob = await _jobsServiceADO.GetById(id);
            if(checkJob != null)
            {
                var info = await _jobsServiceADO.Delete(id);
                if (info == true)
                    return Ok(new SomeException("Job deleted sucessfully.", info));

                return BadRequest("An error occured");
            }
            return NotFound(new SomeException($"Job not found {id}"));
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        [Route("AppliedJobs")]
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
        [Authorize(Policy = "Recruiter")]
        [Route("AppliedJobs")]
        public async Task<IActionResult> GetAllApplicantAppliedToMyJobs()
        {
            var cacheKey = string.Empty;
            object isExist = null;
            IEnumerable<AppliedJobDTO> result = null;
            if (UserId != 0)
            {
                cacheKey = "Recruiter" + UserId.ToString();
                isExist = _memoryCache.Get(cacheKey);
            }

            if (isExist == null)
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
            else
            {
                result = (IEnumerable<AppliedJobDTO>)isExist;
            }
            return Ok(result);
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        [Route("Applicants/AppliedJobs")]
        public async Task<IActionResult> GetAllJobsAppliedByMe()
        {
            var cacheKey = string.Empty;
            object isExist = null;
            IEnumerable<AppliedJobDTO> result = null;
            if (UserId != 0)
            {
                cacheKey = "Applicant_" + UserId.ToString();
                isExist = _memoryCache.Get(cacheKey);
            }

            if (isExist == null)
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
            else
            {
                result = (IEnumerable<AppliedJobDTO>)isExist;
            }
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Policy = "User")]
        [Route("AppliedJobs/{id}")]
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
