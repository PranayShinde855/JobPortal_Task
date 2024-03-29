﻿using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.OTPServices
{
    public interface IOTPServices
    {
        Task<int> GenerateOTP(int userId);
        Task<OTP> GetByOTP(int otp, int userId);
        Task<bool> OTPExists(int otp);
    }
}
