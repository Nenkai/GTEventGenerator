using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Utils
{
    // Credits xfileFIN for pointing to a simplified version
    public static class JulianTime
    {
        public static DateTime JulianToDateTime(double julianDate)
        {
            double unixTime = ((julianDate / 86400) - 2440587.5) * 86400;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();

            return dtDateTime;
        }

        public static double DateTimeToJulian(DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = dateTime.ToUniversalTime() - origin;
            double unixTime = Math.Floor(diff.TotalSeconds);
            double julianDate = ((unixTime / 86400) + 2440587.5) * 86400;

            return julianDate;
        }
    }
}
