using CsvHelper.Configuration;
using CsvHelper;
using OriginIAM.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OriginIAM.Application.Models;

namespace OriginIAM.Infrastructure.Services
{
    public sealed class CsvParserService<T> : ICsvParser<EligibilityRecord> where T : class
    {
        public async IAsyncEnumerable<EligibilityRecord> ParseCsvAsync(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower().Trim(';').Replace(" ", ""),
                HeaderValidated = null,
                Delimiter = ",", 
                Encoding = Encoding.UTF8,
                BadDataFound = context =>
                {
                    var field = context.RawRecord.TrimEnd(';');
                },
            });

            csv.Context.RegisterClassMap<EligibilityRecordMap>();

            while (await csv.ReadAsync())
            {
                EligibilityRecord record;
                try
                {
                    record = csv.GetRecord<EligibilityRecord>();
                }
                catch (CsvHelperException ex)
                {
                    record = new EligibilityRecord
                    {
                        Errors = new List<string>() { ex.Message }
                    };
                }

                yield return record;
            }
        }

    }
}
