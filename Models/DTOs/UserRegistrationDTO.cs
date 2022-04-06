using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class UserRegistrationDTO
    {
        [Required(ErrorMessage = "Please enter user name.")]
        [MaxLength(25)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter address.")]
        [MaxLength(50)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter email.")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.\d)(?=.[!@#$%+-_&*])", ErrorMessage = "Please enter password.")]
        [MaxLength(20)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
