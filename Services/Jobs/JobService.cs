using Database.Repository;
using Models;
using Models.DTOs;
using System;
using System.Linq;
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

        public async Task<Job> Add(JobDTO job, int userId)
        {
            Job obj = new Job();
            obj.Title = job.Title;
            obj.Description = job.Description;
            obj.Skills = job.Skills;
            obj.Location = job.Location;
            obj.CreatedBy = userId;
            obj.ModifiedBy = userId;
            obj.CreatedDate = DateTime.Now;
            obj.ModifiedDate = DateTime.Now;
            obj.IsActive = true;
            var info = await _jobsRepository.Add(obj);
            return info;
        }

        public async Task<Job> CheckJobByUserIdAndJobId(int id, int userId)
        {
            return await _jobsRepository.GetDefault(x => x.CreatedBy == userId && x.Id == id);
        }

        public async Task<Job> Delete(int id)
        {
            var getUser = await _jobsRepository.GetById(id);
            await _jobsRepository.Delete(getUser);
            return getUser;
        }

        public async Task<IQueryable<Job>> GetAll()
        {
            return await _jobsRepository.GetAll();
        }

        public async Task<Job> GetById(int id)
        {
            var info =  await _jobsRepository.GetById(id);
            return info;
        }

        public async Task<Job> Update(JobDTO job, int userId, int id)
        {
            var info = new Job();
            var getById = await _jobsRepository.GetById(id);
            if(getById != null)
            {
                getById.Title = job.Title;
                getById.Description = job.Description;
                getById.Skills = job.Skills;
                getById.Location = job.Location;
                getById.ModifiedBy = userId;
                getById.ModifiedDate = DateTime.Now;
                getById.IsActive = true;
                info = await _jobsRepository.Update(getById);
                return info;
            }
            return info;
        }
    }
}
