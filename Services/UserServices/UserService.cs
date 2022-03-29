using Database.Repository;
using Microsoft.Extensions.Configuration;
using Models;
using Models.DTOs;
using Services.OTPService;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public class UserService : IUserService
    {
        protected readonly IUserRepository _userRepository;
        protected readonly IConfiguration _configuration;
        protected readonly IOTPRepository _oTPRepository;
        protected readonly IOTPService _oTPService;
        public UserService(IUserRepository userRepositor, IConfiguration configuration, IOTPRepository oTPRepository)
        {
            _userRepository = userRepositor;
            _configuration = configuration;
            _oTPRepository = oTPRepository;
        }

        public async Task<Users> AddUser(Users user)
        {
            try
            {
                Users data = new Users();
                data.UserName = user.UserName;
                data.Address = user.Address;
                data.Password = user.Password;
                data.Email = user.Email;
                data.RoleId = 3;
                data.CreatedBy = 0;
                data.ModifiedBy = 0;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.IsActive = true;
                 _userRepository.Add(data);
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Users> AddRecruiter(Users user)
        {
            try
            {
                Users data = new Users();
                data.UserName = user.UserName;
                data.Address = user.Address;
                data.Password = user.Password;
                data.Email = user.Email;
                data.RoleId = 2;
                data.CreatedBy = 0;
                data.ModifiedBy = 0;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.IsActive = true;
                _userRepository.Add(data);
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(int Id)
        {
            var getdata = await _userRepository.GetById(Id);
            if(getdata != null)
            {
                _userRepository.Delete(getdata);
                return true;
            }
            return false;
        }

        public async Task<IQueryable<Users>> GetAll()
        {
             return await _userRepository.GetAll();
        }

        public async Task<Users> GetById(int Id)
        {
            try
            {
                var info = await _userRepository.GetById(Id);
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Users> Update(int Id, Users user)
        {
            try
            {
                Users getdata = await _userRepository.GetById(Id);
                if (getdata != null)
                {
                    getdata.UserName = user.UserName;
                    getdata.Address = user.Address;
                    getdata.Password = user.Password;
                    getdata.Email = user.Email;
                    getdata.ModifiedBy = Id;
                    getdata.ModifiedDate = DateTime.Now;
                    await _userRepository.Update(getdata);
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return user;
        }

        public async Task<Users> GetUser(string email, string password)
        {
            return await _userRepository.GetDefault(x => x.Email == email && x.Password == password);
        }

        public async Task<bool> FireEmail(string to, string subject, string message)
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<Users> GetUser(string email)
        {
            return await _userRepository.GetDefault(x => x.Email == email);
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO dTO, int id)
        {
            Users user = await _userRepository.GetById(id);
            if(dTO.Password == dTO.ConfirmPassword)
            {
                user.Password = dTO.ConfirmPassword;
                await _userRepository.Update(user);
                return true;
            }

            return false;
        }

        public async Task<bool> CheckEmailIdExist(string eamilId)
        {
            var check = await _userRepository.GetDefault(x => x.Email == eamilId);
            if (check != null)
                return true;
            return false;
        }

        public async Task<Users> AddAdmin(Users user)
        {
            try
            {
                Users data = new Users();
                data.UserName = user.UserName;
                data.Address = user.Address;
                data.Password = user.Password;
                data.Email = user.Email;
                data.RoleId = 1;
                data.CreatedBy = 0;
                data.ModifiedBy = 0;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.IsActive = true;
                _userRepository.Add(data);
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendMail(Users users)
        {
            var otp = await _oTPService.GenerateOTP(users.UserId);
            var sub = "OTP";
            var body = "";
            body += "<h3>Job Portal</h3>";
            body += $"<h4 style='font-size:1.1em'>Hi, {users.UserName}</h4>";
            body += $"<h4 style='font-size:1.1em'>This is your OTP to reset your password : {otp}.</h4>";
            var execute = await FireEmail(users.Email, sub, body);
            if (execute == true)
                return true;
            return false;
        }


    }
}
