﻿using ADOServices.ADOServices.UserServices;
using Database.ADO;
using Microsoft.Data.SqlClient;
using Models;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.Jobs
{
    public class AppliedJobsServices : IAppliedJobsServices
    {
        private readonly IUserServices _userServices;
        public AppliedJobsServices(IUserServices userServices)
        {
            _userServices = userServices; 
        }

        public async Task<bool> Add(int jobId, int userId)
        {
            string query = "INSERT INTO AppliedJobs VALUES (@JobId, @UserId, @AppliedDate, @IsActive)";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@JobId", Convert.ToInt32(jobId)),
                new SqlParameter("@UserId", Convert.ToInt32(userId)),
                new SqlParameter("@AppliedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@IsActive", Convert.ToBoolean(true))
            };
            var data = await DB<AppliedJob>.ExecuteData(query, parameters);
            if (data > 0)
            {
                var email = await EmailDetails(jobId, userId);
                if (email != null)
                    await Task.Run(() => SendMailToUser(email));
                    await Task.Run(() => SendMailToRecruiter(email));
                return true;
            }
            return false;
        }

        public async Task<bool> AlreadyAppliedToJob(int jobId, int userId)
        {
            string query = "SELECT * FROM AppliedJobs WHERE JobId = @JobId AND AppliedBy = @AppliedBy";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@JobId", Convert.ToInt32(jobId)),
                new SqlParameter("@AppliedBy", Convert.ToInt32(userId))
            };
            AppliedJob obj = await DB<AppliedJob>.GetSingleRecord(query, parameters);
            if (obj != null)
                return true;
            return false;
        }

        public async Task<bool> Delete(int id)
        {
            string query = $"DELETE AppliedJobs WHERE Id = @JobId ";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@JobId", Convert.ToInt32(id))
           };
            int info = await DB<AppliedJob>.ExecuteData(query, parameters);
            if (info > 0)
                return true;
            return false;
        }

        public async Task<EmailRequestDTO> EmailDetails(int jobId, int applicantId)
        {
            string query = "SELECT ur.UserName AS Recruiter, ur.Email AS RecruiterEmail, ua.UserName AS Applicant," +
                            " ua.Email AS ApplicantEmail, ua.Address AS Address, j.Title AS Title, j.Description AS Description," +
                            " j.skills AS Skills, j.Location AS Location FROM AppliedJobs aj LEFT JOIN Users ua ON aj.AppliedBy = ua.UserId" +
                            " LEFT JOIN Jobs j ON aj.JobId = j.Id LEFT JOIN Users ur ON j.CreatedBy = ur.UserId " +
                            "WHERE aj.AppliedBy = @ApplicantId AND j.Id = @JobId";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@ApplicantId", applicantId),
                new SqlParameter("@JobId", jobId)
            };
            EmailRequestDTO obj = await DB<EmailRequestDTO>.GetSingleRecord(query, parameters);
            return obj;
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
                            "WHERE j.CreatedBy = @UserId ";

                    var parameters = new IDataParameter[]
                    {
                        new SqlParameter("@UserId", Convert.ToInt32(userId))
                    };
            IEnumerable<AppliedJobDTO> obj = await DB<AppliedJobDTO>.GetList(query, parameters);
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
                            "WHERE aj.AppliedBy = @UserId ";
            var parameters = new IDataParameter[]
          {
                new SqlParameter("@UserId", Convert.ToInt32(userId))
          };
            IEnumerable < AppliedJobDTO > obj = await DB<AppliedJobDTO>.GetList(query, parameters);
            return obj;
        }

        public async Task<AppliedJob> GetById(int id)
        {
            string query = "SELECT * FROM AppliedJobs WHERE Id = @JobId";
            var parameters = new IDataParameter[]
          {
                new SqlParameter("@JobId", Convert.ToInt32(id))
          };
            AppliedJob obj  = await DB<AppliedJob>.GetSingleRecord(query, parameters);
            return obj;
        }

        public async Task<bool> SendMailToUser(EmailRequestDTO req)
        {
            var sub = $"Sir/Mam {req.Applicant}, You have successfully applied to job.";
            var body = "";
            body += "<h4 style='font-size:1.1em'>\nJob Details </h4>";
            body += $"<h4 style='font-size:1.1em'>\nTitle : {req.Title} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nDescription : {req.Description} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nSkills : {req.Skills} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nLocation : {req.Location} </h4>";
            var execute = await  _userServices.ExecuteEmail(req.ApplicantEmail, sub, body);
            if (execute == true)
                return true;
            return false;
        }

        public async Task<bool> SendMailToRecruiter(EmailRequestDTO req)
        {
            var sub = $"Sir/Mam {req.Recruiter}, {req.Applicant} have successfully applied to job you poasted.";
            var body = "";
            body += "<h4 style='font-size:1.1em'>\nApplicant Details </h4>";
            body += $"<h4 style='font-size:1.1em'>\nName : {req.Applicant} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nAddress : {req.Address} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nEmail : {req.ApplicantEmail} </h4>";
            body += "<h4 style='font-size:1.1em'>\nJob Details </h4>";
            body += $"<h4 style='font-size:1.1em'>\nTitle : {req.Title} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nDescription : {req.Description} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nSkills : {req.Skills} </h4>";
            body += $"<h4 style='font-size:1.1em'>\nLocation : {req.Location} </h4>";
            var execute = await _userServices.ExecuteEmail(req.RecruiterEmail, sub, body);
            if (execute == true)
                return true;
            return false;
        }
    }
}
