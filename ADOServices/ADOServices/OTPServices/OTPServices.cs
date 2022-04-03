using Database.ADO;
using Models;
using System;
using System.Collections.Generic;
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
                string query = $"INSERT INTO OTP VALUES({otp}, {userId})";
                var obj = await DB<OTP>.ExecuteData(query);
                if (obj > 0)
                    return otp;
            }
            goto newOtp;
        }

        public async Task<IEnumerable<OTP>> GetAll()
        {
            string query = "SELECT * FROM OTP";
            IEnumerable<OTP> obj = await DB<OTP>.GetList(query);
            return obj;
        }

        public async Task<OTP> GetByOTP(int otp)
        {
            string query = $"SELECT * FROM OTP WHERE otp = {otp}";
            OTP obj = await DB<OTP>.GetSingleRecord(query);
            return obj;
        }

        public async Task<bool> OTPExists(int otp)
        {
            string query = $"SELECT * FROM OTP WHERE otp = {otp}";
            OTP obj = await DB<OTP>.GetSingleRecord(query);
            if (obj != null)
                return true;
            return false;
        }
    }
}
