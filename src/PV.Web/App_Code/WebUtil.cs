using System;
using System.Globalization;

namespace PV.Web
{
    // misc display helpers - RJM 2013-04
    public static class WebUtil
    {
        // older dd/MM/yyyy format; Admin + Message Browser still use this.
        // Newer screens use PV.Core.DateUtil.FormatDate instead.
        public static string FmtDt(object o)
        {
            if (o == null || o == DBNull.Value) return "";
            return ((DateTime)o).ToString("dd/MM/yyyy");
        }

        public static string FmtQty(decimal q)
        {
            return q.ToString("#,##0.####");
        }

        public static string FmtPx(decimal p)
        {
            return p.ToString("#,##0.0000");
        }

        public static string FmtMoney(decimal m)
        {
            return m.ToString("#,##0.00");
        }

        public static bool TryParseUkDate(string s, out DateTime d)
        {
            return DateTime.TryParseExact(s.Trim(), "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
        }

        public static string StatusText(string cd)
        {
            switch (cd)
            {
                case "A": return "Active";
                case "D": return "Dormant";
                case "C": return "Closed";
                default: return cd;
            }
        }
    }
}
