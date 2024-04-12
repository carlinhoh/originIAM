using OriginIAM.Application.Models;

namespace OriginIAM.Api.Dtos.Response
{
    public class EligibilityFileSmallCompaniesResponseDto : EligibilityBaseFileResponseDto
    {
        public int TotalProcessed { get; set; }
        public int TotalFailed { get; set; }
        public List<EligibilityRecord> ProcessedRecords { get; set; } = new List<EligibilityRecord>();
        public List<EligibilityRecord> FailedRecords { get; set; } = new List<EligibilityRecord>();
    }
}
