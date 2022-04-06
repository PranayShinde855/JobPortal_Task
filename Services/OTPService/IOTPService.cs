using Models;
using System.Threading.Tasks;

namespace Services.OTPService
{
    public interface IOTPService
    {
        Task<OTP> GetByOTP(int otp, int userId);
        Task<OTP> GetAll();
        Task<bool> ExistOtp(int otp);
        Task<int> GenerateOTP(int userId);
    }
}
