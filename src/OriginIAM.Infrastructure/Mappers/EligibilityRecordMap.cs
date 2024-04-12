using CsvHelper.Configuration;
using OriginIAM.Application.Models;
using OriginIAM.Infrastructure.Utils;

public class EligibilityRecordMap : ClassMap<EligibilityRecord>
{
    public EligibilityRecordMap()
    {
        Map(m => m.Email).Name("email");
        Map(m => m.FullName).Name("full name");
        Map(m => m.Country).Name("country");
        Map(m => m.BirthDate).Name("birth_date");
        Map(m => m.Salary).TypeConverter<TrimmedDecimalConverter>();
    }
}
