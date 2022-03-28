using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public interface IUserSerivce
    {
        Task<Users> AddUser(Users user);
        Task<Users> AddRecrtuter(Users user);
        Task<Users> Update(int Id, Users user);
        Task<bool> Delete(int Id);
        Task<Users> GetById(int Id);
        Task<IQueryable<Users>> GetAll();
        Task<Users> GetUser(string email, string password);
        Task<Users> GetUser(string emil);
        Task<bool> SendEmail(string to, string subject, string message);
        Task<bool> ResetPassword(ResetPasswordDTO dTO, int Id);
    }
}
