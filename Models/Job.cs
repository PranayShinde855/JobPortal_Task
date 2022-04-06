using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter job title.")]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter job description.")]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter skills.")]
        [MaxLength(75)]
        public string Skills { get; set; }

        [Required(ErrorMessage = "Please enter job location.")]
        [MaxLength(100)]
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
