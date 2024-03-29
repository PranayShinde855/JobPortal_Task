﻿using Database.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs;
using Services.OTPService;
using Services.Roles;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IOTPService _oTPService;
        private readonly IRoleService _roleService; 
        public UserService(IUserRepository userRepositor, IConfiguration configuration, IOTPService oTPService
            , IRoleService roleService)
        {
            _userRepository = userRepositor;
            _configuration = configuration;
            _roleService = roleService;
            _oTPService = oTPService;
        }

        public async Task<Users> AddUser(UserRegistrationDTO req)
        {
            try
            {
                Users data = new Users();
                data.UserName = req.UserName;
                data.Address = req.Address;
                data.Password = req.Password;
                data.Email = req.Email;
                data.RoleId = 3;
                data.CreatedBy = 0;
                data.ModifiedBy = 0;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.IsActive = true;
                var info = await _userRepository.Add(data);
                return info;
            }
            catch(Exception ex)
            {
                throw new Exception ( "Error", ex);
            }
        }
        public async Task<Users> AddRecruiter(UserRegistrationDTO req, int userID)
        {
            try
            {
                Users data = new Users();
                data.UserName = req.UserName;
                data.Address = req.Address;
                data.Password = req.Password;
                data.Email = req.Email;
                data.RoleId = 2;
                data.CreatedBy = userID;
                data.ModifiedBy = userID;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.IsActive = true;
                var info = await _userRepository.Add(data);
                return info;
            }
            catch (Exception ex)
            {
                throw new Exception ( "Error", ex);
            }
        }

        public async Task<bool> Delete(Users users)
        {
            var info = await _userRepository.Delete(users);
            if (info != null)
                return true;

            return false;
        }

        public async Task<IQueryable<Users>> GetAll()
        {
             return await _userRepository.GetAll();
        }

        public async Task<Users> GetById(int id)
        {
            try
            {
                var info = await _userRepository.GetById(id);
                return info;
            }
            catch (Exception ex)
            {
                throw new Exception ( "Error", ex);
            }
        }

        public async Task<Users> Update(int userId, UserRegistrationDTO req)
        {
            var info = new Users();
            try
            {
                Users getdata = await _userRepository.GetById(userId);
                if (getdata != null)
                {
                    getdata.UserName = req.UserName;
                    getdata.Address = req.Address;
                    getdata.Password = req.Password;
                    getdata.ModifiedBy = userId;
                    getdata.ModifiedDate = DateTime.Now;
                    info = await _userRepository.Update(getdata);
                }
                return info;
            }
            catch (Exception ex)
            {
                throw new Exception ( "Error", ex);
            }
        }

        public async Task<Users> GetUser(string email, string password)
        {
            return await _userRepository.GetDefault(x => x.Email == email && x.Password == password);
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
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception ( "Error", ex);
            }
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

        public async Task<Users> AddAdmin(UserRegistrationDTO req, int userId)
        {
            var info = new Users();
            try
            {
                Users data = new Users();
                data.UserName = req.UserName;
                data.Address = req.Address;
                data.Password = req.Password;
                data.Email = req.Email;
                data.RoleId = 1;
                data.CreatedBy = userId;
                data.ModifiedBy = userId;
                data.CreatedDate = DateTime.Now;
                data.ModifiedDate = DateTime.Now;
                data.IsActive = true;
                info = await _userRepository.Add(data);
                return info;
            }
            catch (Exception ex)
            {
                throw new Exception ( "Error", ex);
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
            var execute = await ExecuteEmail(users.Email, sub, body);
            if (execute == true)
                return true;
            return false;
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
    }
}
