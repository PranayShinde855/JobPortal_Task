using System;

namespace Models
{
    public class AppliedJob
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int AppliedBy { get; set; }
        public DateTime AppliedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
