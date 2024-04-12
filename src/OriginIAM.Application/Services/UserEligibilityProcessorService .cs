using Microsoft.Extensions.Logging;
using OriginIAM.Application.Interfaces;
using OriginIAM.Application.Mappers;
using OriginIAM.Application.Models;
using OriginIAM.Application.Validation;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace OriginIAM.Application.Services
{
    public sealed class UserEligibilityProcessorService : IUserEligibilityProcessor
    {
        private readonly IUserService _userService;
        private readonly IEmployerService _employerService;
        private readonly IEligibilityReportService _eligibilityReportService;
        private readonly IEligibilityService _eligibilityService;
        private readonly ILogger<UserEligibilityProcessorService> _logger;

        public UserEligibilityProcessorService(IUserService userService, IEmployerService employerService, IEligibilityReportService eligibilityReportService, IEligibilityService eligibilityService, ILogger<UserEligibilityProcessorService> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _employerService = employerService ?? throw new ArgumentNullException(nameof(employerService));
            _eligibilityReportService = eligibilityReportService ?? throw new ArgumentNullException(nameof(eligibilityReportService));
            _eligibilityService = eligibilityService;   
            _logger = logger;
        }

        public async Task ProcessUserEligibilityAsync(EligibilityRecord record, string employerId)
        {
            //Process the file line-by-line, checking for the required columns on each line
            var errors = ValidateRecord(record);

            Console.WriteLine(record.Country);
            _logger.LogInformation(record.Country);

            if (errors.Any())
            {
                SetRecordAsFailed(record, errors);
            }
            else
            {
                SetRecordAsSucess(record);

                await SaveRecordReport(record, employerId);

                await UpdateUserAsync(record, employerId);
            }
        }

        public async Task<string> GetEmployerIdByName(string employerName)
        {
            return await _employerService.GetEmployerIdByName(employerName);
        }

        public async Task<bool> SetUserAsPendingConfirmation(string employerId)
        {
            return await _userService.SetUserAsPendingByEmployerId(employerId);
        }

        public async Task<bool> TerminateOldAccounts(string employerId)
        {
            return await _userService.TerminateOldAccounts(employerId);
        }


        #region private methods
        private List<string> ValidateRecord(EligibilityRecord record)
        {
            return EligibilityRecordValidator.Validate(record);
        }

        private void SetRecordAsFailed(EligibilityRecord record, List<string> errors)
        {
            record.ProcessSuccess = false;
            record.Errors = errors;
        }

        private async Task SaveRecordReport(EligibilityRecord record, string employerId)
        {
            await _eligibilityReportService.AddReportAsync(EligibilityRecordMapper.ToDomain(record, employerId));
        }

        private void SetRecordAsSucess(EligibilityRecord record)
        {
            record.ProcessSuccess = true;
        }

        private async Task UpdateUserAsync(EligibilityRecord record, string employerId)
        {
            var user = await _userService.GetUserByEmailAsync(record.Email);

            if (user != null)
            {
                //Check if the user already exists and update the country and salary
                await _userService.PatchUserDetailsAsync(user.Email, record.Country, record.Salary);
            }
            else
            {
                var elegibleUser = BuildUserFromEligibilityRecord(record, employerId);

                //Caching file data for later use
                await _eligibilityService.SaveOrUpdateEligibleUser(elegibleUser);
            }
        }

        private User BuildUserFromEligibilityRecord(EligibilityRecord record, string employerId)
        {
            return new User(record.Email, record.FullName, record.Country, record.BirthDate.Value, record.Salary.Value, employerId);
        }

        #endregion
    }
}
