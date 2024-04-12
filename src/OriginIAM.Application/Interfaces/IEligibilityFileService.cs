using OriginIAM.Application.Services;
using OriginIAM.Domain.Common;
using OriginIAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OriginIAM.Application.Interfaces
{
    public interface IEligibilityFileService
    {
        Task<EligibilityProcessingResult> ProcessFileAsync(string blobUrl, string employerName, bool isSmallCompany);
        Task<PaginatedResult<EligibilityRecordReport>> GetEligibilityFileReport(string employerId, int pageNumber, int pageSize);
    }
}
