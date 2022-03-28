﻿using Database.Repository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Jobs
{
    public class JobService : IJobService
    {
        private readonly IJobsRepository _jobsRepository;
        public JobService(IJobsRepository jobsRepository)
        {
            _jobsRepository = jobsRepository;
        }

        public async Task<Job> Add(Job job)
        {
            Job obj = new Job();
            obj.Title = job.Title;
            obj.Description = job.Description;
            obj.Skills = job.Skills;
            obj.Location = job.Location;
            obj.CreatedBy = job.CreatedBy;
            obj.IsActive = true;
            _jobsRepository.Add(obj);
            return obj;
        }

        public async Task<Job> Delete(int id)
        {
            var getUser = await _jobsRepository.GetById(id);
            _jobsRepository.Delete(getUser);
            return getUser;
        }

        public async Task<IQueryable<Job>> GetAll()
        {
            return await _jobsRepository.GetAll();
        }

        public async Task<Job> GetById(int id)
        {
            return await _jobsRepository.GetById(id);
        }

        public async Task<Job> Update(Job job)
        {
            var getById = await _jobsRepository.GetById(job.Id);
            if(getById != null)
            {
                getById.Title = job.Title;
                getById.Description = job.Description;
                getById.Skills = job.Skills;
                getById.Location = job.Location;
                getById.CreatedBy = job.CreatedBy;
                getById.IsActive = true;
                _jobsRepository.Update(getById);
                return getById;
            }
            return getById;
        }
    }
}
