using OriginIAM.Application.Models;

namespace OriginIAM.Api.Dtos.Response
{
    /// <summary>
    /// Response DTO for small companies eligibility file processing.
    /// </summary>
    public class EligibilityFileSmallCompaniesResponseDto : EligibilityBaseFileResponseDto
    {
        public int TotalProcessed { get; set; }
        public int TotalFailed { get; set; }
        public List<EligibilityRecord> ProcessedRecords { get; set; } = new List<EligibilityRecord>();
        public List<EligibilityRecord> FailedRecords { get; set; } = new List<EligibilityRecord>();
    }
}
