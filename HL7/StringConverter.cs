using System;
using System.Globalization;

namespace HL7
{
    public static class StringConverter
    {
        public static string FromDate(DateTime src) => src.ToString("yyyyMMdd");

        public static string FromDateTime(DateTime src) => src.ToString("yyyyMMddHHmmss");

        public static DateTime? ToDateTime(string src)
        {
            if (src != null)
            {
                return DateTime.ParseExact(src, new[] { "yyyy", "yyyyMM", "yyyyMMdd", "yyyyMMddHH", "yyyyMMddHHmm", "yyyyMMddHHmmss" }, null, DateTimeStyles.None);
            }
            else
            {
                return null;
            }
        }
    }
}
