﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Jobs;
using Services.UserServices;
using System;
using System.Threading.Tasks;

namespace API.Controllers.Jobs
{
    [Route("api/Jobs")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IUserService _userService;
        private readonly IAppliedJobsService _appliedJobsService;
        public JobsController(IJobService jobService, IAppliedJobsService appliedJobsService, IUserService userService)
        {
            _jobService = jobService;
            _appliedJobsService = appliedJobsService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize("AllAllowed")]
        [Route("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var info = await _jobService.GetAll();
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpGet]
        [Route("GetById")]
        [Authorize("AllAllowed")]
        public async Task<IActionResult> GetJobById(int Id)
        {
            Job info = await _jobService.GetById(Id);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPost]
        [Route("AddJob")]
        //[Authorize("User")]
        public async Task<IActionResult> AddJob(Job job)
        {
            var info = await _jobService.Add(job);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPut]
        [Route("UpdateJob")]
        //[Authorize("Recruiter")]
        public async Task<IActionResult> UpdateJobById(Job job)
        {
            var info = await _jobService.Update(job);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpDelete]
        [Route("DeleteJob")]
        //[Authorize("Recruiter")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var info = await _jobService.Delete(id);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPost]
        [Route("ApplyJob")]
        //[Authorize("User")]
        public async Task<IActionResult> ApplyJob(AppliedJob appliedJob)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var appliedJobStatus = await _appliedJobsService.Add(appliedJob);
                    return Ok(appliedJob);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("GetAllJobsAppliedByMe")]
        //[Authorize("Recruiter")]
        public async Task<IActionResult> GetAllJobsAppliedByMe(int userId)
        {
            if (ModelState.IsValid)
            {
                //return Ok(await _appliedJobsService.GetAll());
                return Ok(await _appliedJobsService.GetAllJobsAppliedByMe(userId)); ;
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("GetAllApplicantAppliedToMyJobs")]
        //[Authorize("Recruiter")]
        public async Task<IActionResult> GetAllApplicantAppliedToMyJobs(int userId)
        {
            if (ModelState.IsValid)
            {   
                //return Ok(await _appliedJobsService.GetAll());
                return Ok(await _appliedJobsService.GetAllApplicantAppliedToMyJobs(userId)); ;
            }
            return BadRequest();
        }

        [HttpGet]
        //[Authorize("User")]
        [Route("GetAllAppliedJobs")]
        public async Task<IActionResult> GetAllAppliedJobs()
        {
            return Ok(await _appliedJobsService.GetAll());
        }

        [HttpPut]
        //[Authorize("User")]
        [Route("UpdateAppliedJob")]
        public async Task<IActionResult> UpdateAppliedJob(AppliedJob appliedJob)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _appliedJobsService.Update(appliedJob));
            }
            return BadRequest();
        }

        [HttpDelete]
        //[Authorize("User")]
        [Route("DeleteAppliedJob")]
        public async Task<IActionResult> DeleteAppliedJob(int id)
        {
            if (id == null)
            {
                return Ok(await _appliedJobsService.Delete(id));
            }
            return NotFound();
        }
    }
}