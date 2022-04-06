using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage ="Please enter email")]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter otp")]
        [MaxLength(4)]
        public int OTP { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [MaxLength(50)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter confirm password")]
        [MaxLength(50)]
        public string ConfirmPassword { get; set; }
    }
}
