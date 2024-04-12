using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Application.Models
{
    public class SignupResult
    {
        public string UserId { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string Message { get; set; }
    }
}
