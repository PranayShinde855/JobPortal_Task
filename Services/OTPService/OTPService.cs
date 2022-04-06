using Database.Repository;
using Models;
using System;
using System.Threading.Tasks;

namespace Services.OTPService
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _oTPRepository;
        public OTPService(IOTPRepository oTPService)
        {
            _oTPRepository = oTPService;
        }

        public async Task<OTP> GetByOTP(int otp, int userId)
        {
            return await _oTPRepository.GetDefault(x=>x.Otp == otp && x.UserId == userId);
        }

        public async Task<int> GenerateOTP(int userId)
        {
            newOtp:
            Random random = new Random();
            var otp =  Convert.ToInt32(random.Next(0, 9999).ToString("D4"));
            var checkOtpExist = await ExistOtp(otp);
            if (checkOtpExist == true)
                goto newOtp;

            OTP oTP = new OTP();
            oTP.Otp = otp;
            oTP.UserId = userId;
            await _oTPRepository.Add(oTP);
            return otp;
        }

        public async Task<bool> ExistOtp(int otp)
        {
            var IsUnique = await _oTPRepository.GetDefault(x => x.Otp == otp);
            if (IsUnique != null)
                return true;
            return false;
        }
    }
}
