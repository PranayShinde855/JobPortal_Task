using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.OTPServices
{
    public interface IOTPServices
    {
        Task<int> GenerateOTP(int userId);
        Task<OTP> GetByOTP(int otp);
        Task<bool> OTPExists(int otp);
        Task<IEnumerable<OTP>> GetAll();
    }
}
