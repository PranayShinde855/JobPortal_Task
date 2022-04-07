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

        [RegularExpression(@"[0-9]+[A-Z]+.{8,8}[a-z]+[!@#$%^&*()_+=\[{\]};:<>|./?,-]", ErrorMessage = "Password should contains alteast one small letter, one captial letter, one digit and one special character.")]
        [MaxLength(8)]
        public string Password { get; set; }

        [RegularExpression(@"[0-9]+[A-Z]+.{8,8}[a-z]+[!@#$%^&*()_+=\[{\]};:<>|./?,-]", ErrorMessage = "Password should contains alteast one small letter, one captial letter, one digit and one special character.")]
        [MaxLength(8)]
        public string ConfirmPassword { get; set; }
    }
}
