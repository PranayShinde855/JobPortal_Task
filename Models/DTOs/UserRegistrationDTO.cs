using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UserRegistrationDTO
    {
        [Required(ErrorMessage = "Please enter user name.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
