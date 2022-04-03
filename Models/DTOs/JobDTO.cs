using System.ComponentModel.DataAnnotations;

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
