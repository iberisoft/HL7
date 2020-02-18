using System;
using System.Globalization;

namespace HL7
{
    public static class StringConverter
    {
        public static string FromDate(DateTime source) => source.ToString("yyyyMMdd");

        public static string FromDateTime(DateTime source) => source.ToString("yyyyMMddHHmmss");

        public static DateTime? ToDateTime(string source)
        {
            if (source != null)
            {
                return DateTime.ParseExact(source, new[] { "yyyy", "yyyyMM", "yyyyMMdd", "yyyyMMddHH", "yyyyMMddHHmm", "yyyyMMddHHmmss" }, null, DateTimeStyles.None);
            }
            else
            {
                return null;
            }
        }
    }
}
