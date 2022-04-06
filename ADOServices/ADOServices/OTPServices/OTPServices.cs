using Database.ADO;
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.OTPServices
{
    public class OTPServices : IOTPServices
    {
        public async Task<int> GenerateOTP(int userId)
        {
        newOtp:
            var checkOtpExist = false;
            Random random = new Random();
            var otp = Convert.ToInt32(random.Next(0, 9999).ToString("D4"));
            checkOtpExist = await OTPExists(otp);
            if (checkOtpExist == false)
            {
                string query = $"INSERT INTO OTP VALUES(@OTP, @UserId)";
                var parameters = new IDataParameter[]
                    {
                        new SqlParameter("OTP", Convert.ToInt32(otp)),
                        new SqlParameter("@UserId", Convert.ToInt32(userId))
                    };
          
                var obj = await DB<OTP>.ExecuteData(query, parameters);
                if (obj > 0)
                    return otp;
            }
            goto newOtp;
        }

        public async Task<OTP> GetByOTP(int otp, int userId)
        {
            string query = $"SELECT * FROM OTP WHERE otp = @OTP AND UserId = @UserId";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@OTP", Convert.ToInt32(otp)),
                new SqlParameter("@UserId", Convert.ToInt32(userId))
            };
            OTP obj = await DB<OTP>.GetSingleRecord(query, parameters);
            return obj;
        }

        public async Task<bool> OTPExists(int otp)
        {
            string query = $"SELECT * FROM OTP WHERE otp = @OTP";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@OTP", Convert.ToInt32(otp))
           };
            OTP obj = await DB<OTP>.GetSingleRecord(query, parameters);
            if (obj != null)
                return true;
            return false;
        }
    }
}
