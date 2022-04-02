using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class JobDTO
    {
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter job description.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter skills.")]
        public string Skills { get; set; }
        [Required(ErrorMessage = "Please enter job location.")]
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }
}
