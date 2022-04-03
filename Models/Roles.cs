using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Roles
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter role.")]
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
