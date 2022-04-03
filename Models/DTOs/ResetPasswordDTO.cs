using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class ResetPasswordDTO
    {
        public int OTP { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
