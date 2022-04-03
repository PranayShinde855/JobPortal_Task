using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
