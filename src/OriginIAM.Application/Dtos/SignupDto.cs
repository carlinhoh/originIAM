using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Application.Dtos
{
    public class SignupDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public bool AcceptTerms { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
