namespace Models.DTOs
{
    public class EmailRequestDTO
    {
        public string Recruiter{ get; set; }
        public string RecruiterEmail{ get; set; }
        public string Applicant { get; set; }
        public string ApplicantEmail { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Skills { get; set; }
        public string Location { get; set; }
    }
}
