using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class ResetPasswordDTO
    {
        [RegularExpression(@"^[a-z0-9A-Z!#$%^&&*/?_`~]+@[a-z0-9A-Z!#$%^&&*/?_`~]+([a-zA-Z].{2,})$", ErrorMessage = "Please enter email.")]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter otp")]
        [MaxLength(4)]
        public int OTP { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.\d)(?=.[!@#$%+-_&*])", ErrorMessage = "Please enter password.")]
        [MaxLength(8)]
        public string Password { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.\d)(?=.[!@#$%+-_&*])", ErrorMessage = "Please enter password.")]
        [MaxLength(8)]
        public string ConfirmPassword { get; set; }
    }
}
