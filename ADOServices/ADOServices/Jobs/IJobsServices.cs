using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.Jobs
{
    public interface IJobsServices
    {
        Task<bool> Add(JobDTO job/*, int userId*/);
        Task<bool> Update(JobDTO job/*, int userId*/, int id);
        Task<bool> Delete(int id);
        Task<Job> GetById(int id);
        Task<IEnumerable<Job>> GetAll();
    }
}
