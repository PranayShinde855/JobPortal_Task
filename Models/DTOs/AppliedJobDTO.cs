using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class AppliedJobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Skills { get; set; }
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int AppliedBy { get; set; }
        public string AppliedByName { get; set; }
        public bool IsActive { get; set; }
    }
}
