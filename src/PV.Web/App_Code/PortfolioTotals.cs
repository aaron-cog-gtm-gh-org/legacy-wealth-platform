using System;
using PV.Core;

namespace PV.Web
{
    // AT 2026-10: the "latest statement value" sum was living on ClientSummary;
    // the client list needs the same figures so it moved here.
    public static class PortfolioTotals
    {
        public class Snapshot
        {
            public DateTime? StmtDate;      // latest statement date found
            public decimal MktValGbp;       // sum of latest-statement values
            public bool HasData;
        }

        // accounts passed as (safeCode) list; totals come off each account's
        // most recent MT535 only
        public static Snapshot ForSafeCodes(params string[] safeCodes)
        {
            var s = new Snapshot();
            foreach (string safe in safeCodes)
            {
                Mt535Statement st = PositionBuilder.GetLatest(safe);
                if (st == null) continue;
                if (!s.StmtDate.HasValue || st.StatementDate > s.StmtDate.Value)
                    s.StmtDate = st.StatementDate;
                foreach (Mt535Line l in st.Lines)
                    s.MktValGbp += l.MktValGbp;
                s.HasData = true;
            }
            return s;
        }
    }
}
