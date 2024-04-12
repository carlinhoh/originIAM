﻿using OriginIAM.Domain.Common;
using OriginIAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginIAM.Domain.Interfaces
{
    public interface IEligibilityReportService
    {
        Task<IEnumerable<EligibilityRecordReport>> GetReportsByEmployerIdAsync(string employerId);
        Task<PaginatedResult<EligibilityRecordReport>> GetReportsByEmployerNameAsync(string EmployerName, int pageNumber, int pageSize);
        Task AddReportAsync(EligibilityRecordReport report);
        Task RemoveLastReport(string employerId);
    }
}
