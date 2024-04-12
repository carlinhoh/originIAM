using OriginIAM.Domain.Common;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Services
{
    public class EligibilityReportService : IEligibilityReportService
    {
        private readonly IEligibilityReportRepository _eligibilityReportRepository;

        public EligibilityReportService(IEligibilityReportRepository eligibilityReportRepository)
        {
            _eligibilityReportRepository = eligibilityReportRepository;
        }

        public async Task AddReportAsync(EligibilityRecordReport report)
        {
            await _eligibilityReportRepository.AddReportAsync(report);
        }

        public async Task<IEnumerable<EligibilityRecordReport>> GetReportsByEmployerIdAsync(string employerId)
        {
            return await _eligibilityReportRepository.GetReportsByEmployerIdAsync(employerId);
        }

        public async Task<PaginatedResult<EligibilityRecordReport>> GetReportsByEmployerNameAsync(string employerId, int pageNumber, int pageSize)
        {
            return await _eligibilityReportRepository.GetReportsByEmployerIdAsync(employerId, pageNumber, pageSize); 
        }

        public async Task RemoveLastReport(string employerId)
        {
            await _eligibilityReportRepository.RemoveLastReport(employerId);
        }
    }
}
