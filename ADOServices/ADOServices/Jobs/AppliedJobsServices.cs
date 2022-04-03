using ADOServices.ADOServices.UserServices;
using Database.ADO;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.Jobs
{
    public class AppliedJobsServices : IAppliedJobsServices
    {
        public readonly IUserServices _userServices;
        public AppliedJobsServices(IUserServices userServices)
        {
            _userServices = userServices;
        }

        public async Task<bool> Add(int jobId, int userId)
        {
            string query = $"INSERT INTO AppliedJobs VALUES ({jobId}, {userId})";
            var data = await DB<AppliedJob>.ExecuteData(query);
            if (data > 0)
                return true;

            return false;
        }

        public async Task<bool> AlreadyAppliedToJob(int jobId, int userId)
        {
            string query = $"SELECT * FROM AppliedJobs WHERE JobId = {jobId} AND AppliedBy = {userId}";
            AppliedJob obj = await DB<AppliedJob>.GetSingleRecord(query);
            if (obj != null)
                return true;
            return false;
        }

        public async Task<bool> Delete(int id)
        {
            string query = $"DELETE AppliedJobs WHERE Id = '"+ id +"' ";
            int info = await DB<AppliedJob>.ExecuteData(query);
            if (info > 0)
                return true;
            return false;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAll()
        {
            string query = "SELECT aj.Id AS Id, j.Title AS Title, j.Description AS Description, j.skills AS Skills, j.Location AS Location" +
                            ", j.CreatedBy AS CreatedBy, ua.UserName AS CreatedByName, aj.AppliedBy AS AppliedBy" +
                            ", us.UserName AS AppliedByName, aj.IsActive AS IsActive" +
                            " FROM AppliedJobs aj LEFT JOIN Jobs j ON aj.JobId = j.Id " +
                            "LEFT JOIN Users us ON aj.AppliedBy = us.UserId " +
                            "LEFT JOIN Users ua ON j.CreatedBy = ua.UserId ";
            IEnumerable <AppliedJobDTO> obj = await DB<AppliedJobDTO>.GetList(query);
            return obj;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllApplicantAppliedToMyJobs(int userId)
        {
            string query = "SELECT aj.Id AS Id, j.Title AS Title, j.Description AS Description, j.skills AS Skills, j.Location AS Location" +
                            ", j.CreatedBy AS CreatedBy, ua.UserName AS CreatedByName, aj.AppliedBy AS AppliedBy" +
                            ", us.UserName AS AppliedByName, aj.IsActive AS IsActive" +
                            " FROM AppliedJobs aj LEFT JOIN Jobs j ON aj.JobId = j.Id " +
                            "LEFT JOIN Users us ON aj.AppliedBy = us.UserId " +
                            "LEFT JOIN Users ua ON j.CreatedBy = ua.UserId " +
                            "WHERE j.CreatedBy = '" + userId + " '";
            IEnumerable<AppliedJobDTO> obj = await DB<AppliedJobDTO>.GetList(query);
            return obj;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAllJobsAppliedByMe(int userId)
        {
            string query =  "SELECT aj.Id AS Id, j.Title AS Title, j.Description AS Description, j.skills AS Skills, j.Location AS Location" +
                            ", j.CreatedBy AS CreatedBy, ua.UserName AS CreatedByName, aj.AppliedBy AS AppliedBy" +
                            ", us.UserName AS AppliedByName, aj.IsActive AS IsActive" +
                            " FROM AppliedJobs aj LEFT JOIN Jobs j ON aj.JobId = j.Id " +
                            "LEFT JOIN Users us ON aj.AppliedBy = us.UserId " +
                            "LEFT JOIN Users ua ON j.CreatedBy = ua.UserId " +
                            "WHERE aj.AppliedBy = '" + userId + " '";
            IEnumerable < AppliedJobDTO > obj = await DB<AppliedJobDTO>.GetList(query);
            return obj;
        }

        public async Task<AppliedJob> GetById(int id)
        {
            string query = $"SELECT * FROM AppliedJobs WHERE Id = {id}";
            AppliedJob obj  = await DB<AppliedJob>.GetSingleRecord(query);
            return obj;
        }

        public async Task<bool> SendMailToRecruiter(EmailRequestDTO req)
        {
            var sub = $"{req.UserName} you have successfully applied to job.";
            var body = "";
            body += "<h4 style='font-size:1.1em'>Job Details </h4>";
            body += $"<h4 style='font-size:1.1em'>Title : {req.Title} </h4>";
            body += $"<h4 style='font-size:1.1em'>Description : {req.Description} </h4>";
            body += $"<h4 style='font-size:1.1em'>Skills : {req.Skills} </h4>";
            body += $"<h4 style='font-size:1.1em'>Location : {req.Location} </h4>";
            var execute = await  _userServices.ExecuteEmail(req.Email, sub, body);
            if (execute == true)
                return true;
            return false;
        }

        public async Task<bool> SendMailToUser(EmailRequestDTO req)
        {
            var sub = $"{req.UserName} have successfully applied to job you poasted.";
            var body = "";
            body += "<h4 style='font-size:1.1em'>Applicant Details </h4>";
            body += $"<h4 style='font-size:1.1em'>Name : {req.UserName} </h4>";
            body += $"<h4 style='font-size:1.1em'>Address : {req.Address} </h4>";
            body += $"<h4 style='font-size:1.1em'>Email : {req.Email} </h4>";
            body += "<h4 style='font-size:1.1em'>Job Details </h4>";
            body += $"<h4 style='font-size:1.1em'>Title : {req.Title} </h4>";
            body += $"<h4 style='font-size:1.1em'>Description : {req.Description} </h4>";
            body += $"<h4 style='font-size:1.1em'>Skills : {req.Skills} </h4>";
            body += $"<h4 style='font-size:1.1em'>Location : {req.Location} </h4>";
            var execute = await _userServices.ExecuteEmail(req.Email, sub, body);
            if (execute == true)
                return true;
            return false;
        }
    }
}
