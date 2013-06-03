using System;
using System.Collections.Generic;
using System.Linq;
using PV.Data;

namespace PV.Core
{
    // RJM 2013-05, tidied KP 2017-04: read positions off the statement index.
    public static class PositionBuilder
    {
        public static List<DateTime> GetStatementDates(string safekeepingAcct)
        {
            var idx = ArchiveIndex.Current;
            var dates = new List<DateTime>();
            if (idx == null) return dates;
            Dictionary<DateTime, List<Mt535Statement>> byDate;
            if (idx.Statements.TryGetValue(Key(safekeepingAcct), out byDate))
                dates.AddRange(byDate.Keys);
            dates.Sort();
            dates.Reverse();
            return dates;
        }

        public static Mt535Statement GetStatement(string safekeepingAcct, DateTime date)
        {
            var idx = ArchiveIndex.Current;
            if (idx == null) return null;
            Dictionary<DateTime, List<Mt535Statement>> byDate;
            if (!idx.Statements.TryGetValue(Key(safekeepingAcct), out byDate)) return null;
            List<Mt535Statement> day;
            if (!byDate.TryGetValue(date.Date, out day)) return null;

            // KP 2017-04: custodian resends with a higher file seq, always take
            // the latest one
            return day.OrderByDescending(s => s.FileSeq).First();
            //return day[0];
        }

        public static Mt535Statement GetLatest(string safekeepingAcct)
        {
            List<DateTime> dates = GetStatementDates(safekeepingAcct);
            return dates.Count == 0 ? null : GetStatement(safekeepingAcct, dates[0]);
        }

        // latest statement strictly before the given date
        public static Mt535Statement GetPrevious(string safekeepingAcct, DateTime date)
        {
            var idx = ArchiveIndex.Current;
            if (idx == null) return null;
            Dictionary<DateTime, List<Mt535Statement>> byDate;
            if (!idx.Statements.TryGetValue(Key(safekeepingAcct), out byDate)) return null;
            DateTime best = DateTime.MinValue;
            foreach (DateTime d in byDate.Keys)
                if (d < date.Date && d > best) best = d;
            return best == DateTime.MinValue ? null : GetStatement(safekeepingAcct, best);
        }

        // display name for a holding; statement description is not used here
        public static string InstrumentName(string isin)
        {
            string name = InstrumentData.GetName(isin);
            return name ?? "UNKNOWN INSTRUMENT " + isin;
        }

        private static string Key(string safekeepingAcct)
        {
            return safekeepingAcct == null ? "" : safekeepingAcct.TrimStart('0');
        }
    }
}
