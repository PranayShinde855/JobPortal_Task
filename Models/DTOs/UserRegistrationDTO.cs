﻿using System.ComponentModel.DataAnnotations;

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

        [RegularExpression(@"^[a-z0-9A-Z!#$%^&&*/?_`~]+@[a-z0-9A-Z!#$%^&&*/?_`~]+([a-zA-Z].{2,})$", ErrorMessage = "Please enter email.")]
        [MaxLength(50)]
        public string Email { get; set; }

        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [MaxLength(8)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
