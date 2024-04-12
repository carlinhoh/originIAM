using OriginIAM.Application.Models;

namespace OriginIAM.Application.Services
{
    public class EligibilityProcessingSmallCompaniesResult : EligibilityProcessingResult
    {
        public List<EligibilityRecord> SuccessfulRecords { get; set; } = new List<EligibilityRecord>();
        public List<EligibilityRecord> FailedRecords { get; set; } = new List<EligibilityRecord>();
        public int TotalProcessed => SuccessfulRecords.Count + FailedRecords.Count;
        public int TotalSuccessful => SuccessfulRecords.Count;
        public int TotalFailed => FailedRecords.Count;
    }
}