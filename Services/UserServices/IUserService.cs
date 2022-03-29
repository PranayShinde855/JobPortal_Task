using Models;
using Models.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public interface IUserService
    {
        Task<Users> AddUser(Users user);
        Task<Users> AddRecruiter(Users user);
        Task<Users> AddAdmin(Users user);
        Task<Users> Update(int Id, Users user);
        Task<bool> Delete(int Id);
        Task<Users> GetById(int Id);
        Task<IQueryable<Users>> GetAll();
        Task<Users> GetUser(string email, string password);
        Task<Users> GetUser(string emil);
        Task<bool> FireEmail(string to, string subject, string message);
        Task<bool> ResetPassword(ResetPasswordDTO dTO, int Id);
        Task<bool> CheckEmailIdExist(string Email);
        Task<bool> SendMail(Users users);
    }
}
