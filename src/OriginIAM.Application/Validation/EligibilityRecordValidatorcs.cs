using OriginIAM.Application.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OriginIAM.Application.Validation
{
    public static class EligibilityRecordValidator
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex CountryCodeRegex = new Regex(@"^[A-Za-z]{2}$", RegexOptions.Compiled);

        public static List<string>  Validate(EligibilityRecord record)
        {
            var errors = new List<string>();

            if (!IsValidEmail(record.Email))
            {
                errors.Add("Invalid email address format.");
            }

            if (!IsValidCountryCode(record.Country))
            {
                errors.Add("The country code must be 2 characters long and contain only letters.");
            }

            if (record.BirthDate.HasValue && !IsValidBirthDate(record.BirthDate.Value))
            {
                errors.Add("Invalid birth date. The date must be in the past and not a future date.");
            }

            if (record.Salary.HasValue && !IsValidSalary(record.Salary.Value))
            {
                errors.Add("Invalid salary. The salary must be a positive number.");
            }

            return errors;
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && EmailRegex.IsMatch(email);
        }

        private static bool IsValidCountryCode(string countryCode)
        {
            return !string.IsNullOrEmpty(countryCode) && CountryCodeRegex.IsMatch(countryCode);
        }

        private static bool IsValidBirthDate(DateTime birthDate)
        {
            return birthDate <= DateTime.UtcNow.Date;
        }

        private static bool IsValidSalary(decimal salary)
        {
            return salary >= 0;
        }
    }
}
