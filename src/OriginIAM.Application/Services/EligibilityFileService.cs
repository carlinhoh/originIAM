using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Models;
using OriginIAM.Domain.Common;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;

namespace OriginIAM.Application.Services
{
    public sealed class EligibilityFileService : IEligibilityFileService
    {
        private readonly IFileDownloader _fileDownloader;
        private readonly ICsvParser<EligibilityRecord> _csvParser;
        private readonly IUserEligibilityProcessor _userEligibilityProcessor;
        private readonly IEligibilityReportService _eligibilityReportService;
        public EligibilityFileService(
            IFileDownloader fileDownloader,
            ICsvParser<EligibilityRecord> csvParser,
            IUserEligibilityProcessor userEligibilityProcessor,
            IEligibilityReportService eligibilityReportService)
        {
            _fileDownloader = fileDownloader;
            _csvParser = csvParser;
            _userEligibilityProcessor = userEligibilityProcessor;
            _eligibilityReportService = eligibilityReportService;
        }

        public async Task<EligibilityProcessingResult> ProcessFileAsync(string blobUrl, string employerName, bool isSmallCompany)
        {
            var employerId = await GetOrCreateEmployer(employerName);//For simplicity's sake, I won't create the employee

            //Temporary value flag for users who were inserted in the last processing of this company.
            await SetExistingUsersToPending(employerId);

            var processingResult = isSmallCompany
                ? await DownloadAndProcessSmallCompaniesFile(blobUrl, employerId)
                : await DownloadAndProcessFile(blobUrl, employerId);

            //Terminate the accounts of users attached to that employer that are no longer coming in the eligibility file
            await TerminateOldAccountsIfNeeded(employerId);

            return processingResult;
        }


        public async Task<PaginatedResult<EligibilityRecordReport>> GetEligibilityFileReport(string employerId, int pageNumber, int pageSize)
        {

            return await _eligibilityReportService.GetReportsByEmployerNameAsync(employerId, pageNumber, pageSize);
        }


        #region private methods

        private async Task ProcessSmallCompaniesRecord(EligibilityRecord record, string employerId, EligibilityProcessingSmallCompaniesResult processingResult)
        {
            await _userEligibilityProcessor.ProcessUserEligibilityAsync(record, employerId);

            AddRecordToResult(record, processingResult);
        }
        private async Task ProcessLargeCompaniesRecord(EligibilityRecord record, string employerId, EligibilityProcessingLargeCompaniesResult processingResult)
        {
            await _userEligibilityProcessor.ProcessUserEligibilityAsync(record, employerId);
        }

        private async Task<string> GetOrCreateEmployer(string employerName)
        {
            var employerId = await _userEligibilityProcessor.GetEmployerIdByName(employerName);

            if (employerId == null)
            {
                //For simplicity's sake, I won't create the employee
            }

            return employerId;
        }

        private async Task SetExistingUsersToPending(string employerId)
        {
            if (!string.IsNullOrEmpty(employerId))
            {
                await _userEligibilityProcessor.SetUserAsPendingConfirmation(employerId);

                await _eligibilityReportService.RemoveLastReport(employerId);
            }
        }

        private async Task<EligibilityProcessingResult> DownloadAndProcessSmallCompaniesFile(string blobUrl, string employerId)
        {
            var processingResult = new EligibilityProcessingSmallCompaniesResult();

            await _fileDownloader.DownloadFileAsync(blobUrl, async stream =>
            {
                await foreach (var record in _csvParser.ParseCsvAsync(stream))
                {
                    await ProcessSmallCompaniesRecord(record, employerId, processingResult);
                }
            }, "text/csv");

            return processingResult;
        }

        private async Task<EligibilityProcessingLargeCompaniesResult> DownloadAndProcessFile(string blobUrl, string employerId)
        {
            var processingResult = new EligibilityProcessingLargeCompaniesResult();

            await _fileDownloader.DownloadFileAsync(blobUrl, async stream =>
            {
                await foreach (var record in _csvParser.ParseCsvAsync(stream))
                {
                    await ProcessLargeCompaniesRecord(record, employerId, processingResult);
                }
            }, "text/csv");

            return processingResult;
        }

        private void AddRecordToResult(EligibilityRecord record, EligibilityProcessingSmallCompaniesResult processingResult)
        {
            if (record.ProcessSuccess)
            {
                processingResult.SuccessfulRecords.Add(record);
            }
            else
            {
                processingResult.FailedRecords.Add(record);
            }
        }

        private async Task TerminateOldAccountsIfNeeded(string employerId)
        {
            if (!string.IsNullOrEmpty(employerId))
            {
                await _userEligibilityProcessor.TerminateOldAccounts(employerId);
            }
        }

        #endregion
    }
}
