using ADOServices.ADOServices.Jobs;
using Database.ADO;
using Microsoft.Data.SqlClient;
using Models;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Services.ADOServices.Jobs
{
    public class JobsServices : IJobsServices
    {
        public async Task<bool> Add(JobDTO job, int userId)
        {
            string query = "INSERT INTO Jobs(Title, Description, skills, Location, CreatedBy, ModifiedBy, " +
                "CreatedDate, ModifiedDate, IsActive) VALUES(@Title, @Description, @skills, @Location," +
                " @CreatedBy, @ModifiedBy, @CreatedDate, @ModifiedDate, @IsActive)";

            var parameters = new IDataParameter[]
            {
                new SqlParameter("@Title", Convert.ToString(job.Title)),
                new SqlParameter("@Description", Convert.ToString(job.Description)),
                new SqlParameter("@skills", Convert.ToString(job.Skills)),
                new SqlParameter("@Location", Convert.ToString(job.Location)),
                new SqlParameter("@CreatedBy", Convert.ToInt32(userId)),
                new SqlParameter("@ModifiedBy", Convert.ToInt32(userId)),
                new SqlParameter("@CreatedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@ModifiedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@IsActive", Convert.ToBoolean(job.IsActive))
            };

            var data = await DB<Job>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

        public async Task<bool> Delete(int jobId)
        {
            string query = "DELETE Jobs WHERE Id = @JobId";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@JobId", Convert.ToInt32(jobId))
            };
            var result =  await DB<Job>.ExecuteData(query, parameters);
            if(result > 0 )
                return true;
            return false;
        }

        public async Task<IEnumerable<Job>> GetAll()
        {
            string query = "select * from Jobs";
            IEnumerable<Job> obj = await DB<Job>.GetList(query);
            return obj;
        }

        public async Task<Job> GetById(int id)
        {
            string query = "SELECT * FROM Jobs WHERE Id = @JobId";
            var parameters = new IDataParameter[]
          {
                new SqlParameter("@JobId", Convert.ToInt32(id))
          };
            Job obj = await DB<Job>.GetSingleRecord(query, parameters);
            return obj;
        }

        public async Task<bool> Update(JobDTO job, int userId, int id)
        {
            string query = "Update Jobs Set Title = @Title, Description = @Description, skills = @skills, Location = @Location, ModifiedDate = @ModifiedDate" +
                ", ModifiedBy = @ModifiedBy, IsActive = @IsActive WHERE Id = @JobId ";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@Title", Convert.ToString(job.Title)),
                new SqlParameter("@Description", Convert.ToString(job.Description)),
                new SqlParameter("@skills",Convert.ToString(job.Skills)),
                new SqlParameter("@Location", Convert.ToString(job.Location)),
                new SqlParameter("@ModifiedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@ModifiedBy", Convert.ToInt32(userId)),
                new SqlParameter("@IsActive", Convert.ToBoolean(job.IsActive)),
                new SqlParameter("@JobId", Convert.ToInt32(id))
           };
            var data = await DB<Job>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

    }
}
