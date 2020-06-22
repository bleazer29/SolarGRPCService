using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarService.Misc
{
    public static class TimestampDateTimeConverter
    {
        public static DateTime UnixTimeStampToDateTime(Timestamp unixTimeStamp)
        {
            DateTime dtDateTime = unixTimeStamp.ToDateTime();
            return dtDateTime;
        }

        public static long DateTimeToUnixTimeStamp(DateTime date)
        {
            Timestamp unixTimeStampInSeconds = date.ToTimestamp();
            return unixTimeStampInSeconds.Seconds;
        }
    }
}
