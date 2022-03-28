using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OTPService
{
    public interface IOTPService
    {
        Task<OTP> GetByOTP(int otp);
        Task<OTP> GetAll();
        Task<bool> ExistOtp(int otp);
        Task<int> GenerateOTP(int Otp);
    }
}
