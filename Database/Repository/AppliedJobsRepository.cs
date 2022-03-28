using Database.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IAppliedJobsRepository : IRepository<AppliedJob>
    {
        Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId);
        Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId);
        Task<IEnumerable<AppliedJobDTO>> GetAllAppliedJobs();
    }
    public class AppliedJobRepository : Repository<AppliedJob>, IAppliedJobsRepository
    {
        public AppliedJobRepository(DbContextModel dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId)
        {
                var data = await (from a in _dbContext.AppliedJobs
                                 join job in _dbContext.Jobs on a.JobId equals job.Id
                                 join applicant in _dbContext.Users on a.AppliedBy equals applicant.UserId
                                 join recruter in _dbContext.Users on job.CreatedBy equals recruter.UserId
                                 select new AppliedJobDTO
                                 {
                                     Id = a.JobId,
                                     Title = job.Title,
                                     Description = job.Description,
                                     Skills = job.Skills,
                                     Location = job.Location,
                                     CreatedBy = job.CreatedBy,
                                     CreatedByName = recruter.UserName,
                                     AppliedBy = a.AppliedBy,
                                     AppliedByName = applicant.UserName,
                                     IsActive = job.IsActive
                                 }).Where(x => x.CreatedBy == userId)
                                   .ToListAsync();

            return data;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllAppliedJobs()
        {
            var data = await(from job in _dbContext.Jobs
                             join a in _dbContext.AppliedJobs on job.Id equals a.JobId
                             join applicant in _dbContext.Users on a.AppliedBy equals applicant.UserId
                             join recruter in _dbContext.Users on job.CreatedBy equals recruter.UserId
                             select new AppliedJobDTO
                             {
                                 Id = a.JobId,
                                 Title = job.Title,
                                 Description = job.Description,
                                 Skills = job.Skills,
                                 Location = job.Location,
                                 CreatedBy = job.CreatedBy,
                                 CreatedByName = recruter.UserName,
                                 AppliedBy = a.AppliedBy,
                                 AppliedByName = applicant.UserName,
                                 IsActive = job.IsActive
                             }).ToListAsync();

            return data;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId)
        {
            var data = await(from a in _dbContext.AppliedJobs
                                         join job in _dbContext.Jobs on a.JobId equals job.Id
                                         join applicant in _dbContext.Users on a.AppliedBy equals applicant.UserId
                                         join recruter in _dbContext.Users on job.CreatedBy equals recruter.UserId
                                         select new AppliedJobDTO
                                         {
                                             Id = a.JobId,
                                             Title = job.Title,
                                             Description = job.Description,
                                             Skills = job.Skills,
                                             Location = job.Location,
                                             CreatedBy = job.CreatedBy,
                                             CreatedByName = recruter.UserName,
                                             AppliedBy = a.AppliedBy,
                                             AppliedByName = applicant.UserName,
                                             IsActive = job.IsActive
                                         }).Where(x => x.AppliedBy == userId)
                                   .ToListAsync();

            return data;
        }
    }
}
