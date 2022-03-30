using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AppliedJob
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter job Id.")]
        public int JobId { get; set; }
        [Required(ErrorMessage = "Please enter applicant Id.")]
        public int AppliedBy { get; set; }
        public DateTime AppliedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
