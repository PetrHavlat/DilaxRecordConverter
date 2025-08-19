using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DilaxRecordConverter.Core.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Převede Unix timestamp na lokální DateTime.
        /// </summary>
        public static DateTime UnixTimestampToDateTime(uint unixTimestamp)
        {
            // Unix timestamp je počet sekund od 1.1.1970 00:00:00 UTC
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            return dateTimeOffset.LocalDateTime;
        }

        /// <summary>
        /// Převede Unix timestamp na UTC DateTime.
        /// </summary>
        public static DateTime UnixTimestampToDateTimeUtc(uint unixTimestamp)
        {
            // Unix timestamp je počet sekund od 1.1.1970 00:00:00 UTC
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            return dateTimeOffset.UtcDateTime;
        }
    }
}
