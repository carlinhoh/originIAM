using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;

namespace OriginIAM.Infrastructure.Utils
{
    public class TrimmedDecimalConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
             var trimmed = text.TrimEnd(';');
            return base.ConvertFromString(trimmed, row, memberMapData);
        }
    }

}
