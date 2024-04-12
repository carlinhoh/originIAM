using OriginIAM.Application.Models;

namespace OriginIAM.Application.Services
{
    public abstract class EligibilityProcessingResult
    {
        public string ProcessId { get; set; }
        public string Message { get; set; }
    }
}