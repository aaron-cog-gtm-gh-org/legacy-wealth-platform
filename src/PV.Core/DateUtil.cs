using System;
using System.Globalization;

namespace PV.Core
{
    public static class DateUtil
    {
        // statement dates displayed as e.g. 15-NOV-2018 across the app
        public static string FormatDate(DateTime d)
        {
            return d.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();
        }
    }
}
