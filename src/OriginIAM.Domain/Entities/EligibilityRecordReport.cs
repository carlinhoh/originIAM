using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Entities
{
    public class EligibilityRecordReport
    {
        public int Id { get; set; }
        public string EmployerId { get; set; }
        public string RecordData { get; set; } 
        public string Status { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
