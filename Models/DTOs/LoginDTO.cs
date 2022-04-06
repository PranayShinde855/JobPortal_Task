using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class LoginDTO
    {
        [RegularExpression(@"^[a-z0-9A-Z!#$%^&&*/?_`~]+@[a-z0-9A-Z!#$%^&&*/?_`~]+([a-zA-Z].{2,})$", ErrorMessage = "Please enter email.")]
        [MaxLength(20)]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.\d)(?=.[!@#$%+-_&*])", ErrorMessage = "Please enter password.")]
        [MaxLength(8)]
        public string Password { get; set; }
    }
}
