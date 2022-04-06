using Models;
using Models.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public interface IUserService
    {
        Task<Users> AddUser(UserRegistrationDTO req);
        Task<Users> AddRecruiter(UserRegistrationDTO req, int userId);
        Task<Users> AddAdmin(UserRegistrationDTO req, int userId);
        Task<Users> Update(int Id, UserRegistrationDTO req);
        Task<bool> Delete(Users users);
        Task<Users> GetById(int Id);
        Task<IQueryable<Users>> GetAll();
        Task<Users> GetUser(string email, string password);
        Task<Users> GetUser(string email);
        Task<bool> ExecuteEmail(string to, string subject, string message);
        Task<bool> ResetPassword(ResetPasswordDTO dTO, int Id);
        Task<bool> CheckEmailIdExist(string Email);
        Task<bool> SendMail(Users users);
        Task<TokenDetailsTO> GenerateToken(GenerateTokenRequestDTO obj);
    }
}
