
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class EmailRequestDTO
    {
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Skills { get; set; }
        public string Location { get; set; }
    }
}
