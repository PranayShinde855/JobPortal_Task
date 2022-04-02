using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.UserServices
{
    public interface IUserServices
    {
        Task<Users> GetById(int id);
        Task<IEnumerable<Users>> GetAll();
        Task<bool> AddUser(UserRegistrationDTO user);
        Task<bool> AddRecruiter(UserRegistrationDTO user);
        Task<bool> AddAdmin(UserRegistrationDTO user);
        Task<bool> Update(int userId, UserRegistrationDTO user);
        Task<bool> Delete(int Id);
        Task<Users> GetUser(string email, string password);
        Task<Users> GetUser(string email);
        Task<bool> ExecuteEmail(string to, string subject, string message);
        Task<bool> ResetPassword(ResetPasswordDTO dTO, int id);
        Task<bool> CheckEmailIdExist(string email);
        Task<bool> SendMail(Users users);
        Task<TokenDetailsTO> GenerateToken(GenerateTokenRequestDTO obj);
    }
}
