using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage ="Please enter user name.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter address.")]
        public string Address { get; set; }

        [RegularExpression(@"^[a-z0-9A-Z!#$%^&&*/?_`~]+@[a-z0-9A-Z!#$%^&&*/?_`~]+([a-zA-Z].{2,})$", ErrorMessage = "Please enter email.")]
        public string Email { get; set; }

        [RegularExpression(@"[0-9]+[A-Z]+.{8,15}[a-z]+[!@#$%^&*()_+=\[{\]};:<>|./?,-]", ErrorMessage = "Password should contains alteast one small letter, one captial letter, one digit and one special character.")]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
