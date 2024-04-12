using OriginIAM.Application.Models;
using OriginIAM.Domain.Entities;
using Newtonsoft.Json;

namespace OriginIAM.Application.Mappers
{
    public static class EligibilityRecordMapper
    {
        public static EligibilityRecordReport ToDomain(EligibilityRecord eligibilityRecord, string employerId)
        {
            return new EligibilityRecordReport
            {
                EmployerId = employerId,
                RecordData = JsonConvert.SerializeObject(eligibilityRecord),
                Status = eligibilityRecord.ProcessSuccess ? "Processed" : "Failed",
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
