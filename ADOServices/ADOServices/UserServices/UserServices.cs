using ADOServices.ADOServices.OTPServices;
using Database.ADO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.UserServices
{
    public class UserServices : IUserServices
    {
        public readonly IConfiguration _configuration;
        public readonly IOTPServices _oTPServices;
        public UserServices(IConfiguration configuration, IOTPServices oTPServices)
        {
            _configuration = configuration;
            _oTPServices = oTPServices;
        }

        public async Task<bool> AddAdmin(UserRegistrationDTO user)
        {
            string query = "INSERT INTO Users(UserName, Address, Email, Password, RoleId, CreatedBy, ModifiedBy," +
                " CreatedDate, ModifiedDate, IsActive) VALUES(@UserName, @Address, @Email, @Password, @RoleId, @CreatedBy, " +
                "@ModifiedBy, @CreatedDate, @ModifiedDate, @IsActive)";

            var parameters = new IDataParameter[]
            {
                new SqlParameter("@UserName", Convert.ToString(user.UserName)),
                new SqlParameter("@Address", Convert.ToString(user.Address)),
                new SqlParameter("@Email", Convert.ToString(user.Email)),
                new SqlParameter("@Password", Convert.ToString(user.Password)),
                new SqlParameter("@RoleId", Convert.ToInt32(1)),
                new SqlParameter("@CreatedBy", Convert.ToInt32(0)),
                new SqlParameter("@ModifiedBy", Convert.ToInt32(0)),
                new SqlParameter("@CreatedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@ModifiedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@IsActive", Convert.ToBoolean(true))
           };
            var data = await DB<Users>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

        public async Task<bool> AddRecruiter(UserRegistrationDTO user)
        {
            string query = "INSERT INTO Users(UserName, Address, Email, Password, RoleId, CreatedBy, ModifiedBy," +
                " CreatedDate, ModifiedDate, IsActive) VALUES(@UserName, @Address, @Email, @Password, @RoleId, @CreatedBy, " +
                "@ModifiedBy, @CreatedDate, @ModifiedDate, @IsActive)";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@UserName", Convert.ToString(user.UserName)),
                new SqlParameter("@Address", Convert.ToString(user.Address)),
                new SqlParameter("@Email", Convert.ToString(user.Email)),
                new SqlParameter("@Password", Convert.ToString(user.Password)),
                new SqlParameter("@RoleId", Convert.ToInt32(2)),
                new SqlParameter("@CreatedBy", Convert.ToInt32(0)),
                new SqlParameter("@ModifiedBy", Convert.ToInt32(0)),
                new SqlParameter("@CreatedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@ModifiedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@IsActive", Convert.ToBoolean(true))
           };
            var data = await DB<Users>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

        public async Task<bool> AddUser(UserRegistrationDTO user)
        {
            string query = "INSERT INTO Users(UserName, Address, Email, Password, RoleId, CreatedBy, ModifiedBy," +
                " CreatedDate, ModifiedDate, IsActive) VALUES( @UserName, @Address, @Email, @Password, @RoleId," +
                " @CreatedBy, @ModifiedBy, @CreatedDate, @ModifiedDate, @IsActive)";

            var parameters = new IDataParameter[]
            {
                new SqlParameter("@UserName", Convert.ToString(user.UserName)),
                new SqlParameter("@Address", Convert.ToString(user.Address)),
                new SqlParameter("@Email", Convert.ToString(user.Email)),
                new SqlParameter("@Password", Convert.ToString(user.Password)),
                new SqlParameter("@RoleId", Convert.ToInt32(3)),
                new SqlParameter("@CreatedBy", Convert.ToInt32(0)),
                new SqlParameter("@ModifiedBy", Convert.ToInt32(0)),
                new SqlParameter("@CreatedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@ModifiedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@IsActive", Convert.ToBoolean(true))
           };
            var data = await DB<Users>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

        public async Task<bool> CheckEmailIdExist(string email)
        {
            string query = "SELECT * FROM Users WHERE Email = '" + email + "' ";
            Users obj = await DB<Users>.GetSingleRecord(query);
            if (obj != null)
                return true;
            return false;
        }

        public async Task<bool> Delete(int id)
        {
            string query = "DELETE Users WHERE UserId = '" + id + "' ";
            int info = await DB<Users>.ExecuteData(query);
            if (info > 0)
                return true;
            return false;
        }

        public async Task<bool> ExecuteEmail(string to, string subject, string message)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_configuration["EMailSettings:Mail"], _configuration["EMailSettings:DisplayName"])
                };
                to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(t => mail.To.Add(new MailAddress(t)));
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_configuration["EMailSettings:Host"], Convert.ToInt32(_configuration["EMailSettings:Port"])))
                {
                    smtp.Credentials = new NetworkCredential(_configuration["EMailSettings:Mail"], _configuration["EMailSettings:Password"]);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TokenDetailsTO> GenerateToken(GenerateTokenRequestDTO req)
        {
            var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, req.UserName),
                        new Claim("UserId", Convert.ToString(req.UserId), ClaimValueTypes.Integer),
                        new Claim("Email", req.Email, ClaimValueTypes.String),
                        new Claim("RoleId", Convert.ToString(req.RoleId), ClaimValueTypes.Integer),
                        new Claim(ClaimTypes.Role, req.Role, ClaimValueTypes.String),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claim,
                expires: DateTime.Now.AddMonths(2),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            TokenDetailsTO obj = new TokenDetailsTO();
            obj.Token = new JwtSecurityTokenHandler().WriteToken(token);
            obj.Expiration = token.ValidTo;
            return obj;
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            string query = "SELECT * FROM Users";
            IEnumerable<Users> obj = await DB<Users>.GetList(query);
            return obj;
        }

        public async Task<Users> GetById(int id)
        {
            string query = "SELECT * FROM Users WHERE UserId = '" + id + "'";
            Users obj = await DB<Users>.GetSingleRecord(query);
            return obj;
        }

        public async Task<Users> GetUser(string email, string password)
        {
            string query = "SELECT * FROM Users WHERE Email = '" + email + "' AND Password = '" + password + "' ";
            Users obj = await DB<Users>.GetSingleRecord(query);
            return obj;
        }

        public async Task<Users> GetUser(string email)
        {
            string query = "SELECT * FROM Users WHERE Email = '" + email + "' ";
            Users obj = await DB<Users>.GetSingleRecord(query);
            return obj;
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO dTO, int id)
        {
            string query = "UPDATE Users SET Password = '"+ dTO.ConfirmPassword +"' WHERE UserId = '" + id + "'";
            int obj = await DB<Users>.ExecuteData(query);
            if (obj > 0)
                return true;
            return false;
        }

        public async Task<bool> SendMail(Users users)
        {
            var otp = await _oTPServices.GenerateOTP(users.UserId);
            var sub = "OTP";
            var body = "";
            body += "<h3>Job Portal</h3>";
            body += $"<h4 style='font-size:1.1em'>Hi, {users.UserName}</h4>";
            body += $"<h4 style='font-size:1.1em'>This is your OTP to reset your password : {otp}.</h4>";
            var execute = await ExecuteEmail(users.Email, sub, body);
            if (execute == true)
                return true;
            return false;
        }

        public async Task<bool> Update(int userId, UserRegistrationDTO user)
        {
            var serialize = JsonConvert.SerializeObject(user);
            JObject jobject = JObject.Parse(serialize);
            string query = "Update Users SET UserName = @UserName, Address = @Address, Password = @Password" +
                ", ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate, IsActive = @IsActive " +
                "WHERE UserId = '"+ userId +"' ";

            var parameters = new IDataParameter[]
            {
                new SqlParameter("@UserName", jobject["UserName"].ToString()),
                new SqlParameter("@Address", jobject["Address"].ToString()),
                new SqlParameter("@Password",jobject["Password"].ToString()),
                new SqlParameter("@ModifiedBy", userId),
                new SqlParameter("@ModifiedDate", DateTime.Now),
                new SqlParameter("@IsActive", user.IsActive),
           };

            var data = await DB<Users>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

    }
}
