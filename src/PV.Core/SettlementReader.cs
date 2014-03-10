using System;
using System.Collections.Generic;

namespace PV.Core
{
    // DW 2014-03: pulls settlement confirmations out of the archive index for
    // an account and date range. Callers pass the custodian account padded to 7
    // digits as sent on the messages.
    public static class SettlementReader
    {
        public static List<SettlementConfirm> GetSettlements(string custAcct, DateTime from, DateTime to)
        {
            var result = new List<SettlementConfirm>();
            var idx = ArchiveIndex.Current;
            if (idx == null) return result;

            foreach (KeyValuePair<string, List<SettlementConfirm>> folder in idx.ConfirmFolders)
            {
                // one day folder at a time, CANCs come in with the day's drop
                var active = new List<SettlementConfirm>();
                var cancels = new List<SettlementConfirm>();
                foreach (SettlementConfirm c in folder.Value)
                {
                    if (c.Function == "CANC") cancels.Add(c);
                    else active.Add(c);
                }
                foreach (SettlementConfirm canc in cancels)
                {
                    active.RemoveAll(c => c.Seme == canc.Prev);
                }

                //foreach (SettlementConfirm c in active)
                //{
                //    if (c.custAcct.TrimStart('0') != custAcct.TrimStart('0')) continue;
                //    ...
                //}
                // DW 2014-06 - plain compare is fine, callers pad now

                foreach (SettlementConfirm c in active)
                {
                    if (c.custAcct != custAcct) continue;
                    if (c.SettleDate < from.Date || c.SettleDate > to.Date) continue;
                    result.Add(c);
                }
            }

            result.Sort((a, b) => b.SettleDate.CompareTo(a.SettleDate)); // newest first
            return result;
        }

        // amounts are always 2dp
        // TODO DW 2015-01: use SwiftParser.ParseDecimal here as well?
        public static decimal ToAmount(string raw)
        {
            raw = raw.Substring(3);
            string[] parts = raw.Split(',');
            return int.Parse(parts[0])
                 + (parts.Length > 1 && parts[1].Length > 0 ? int.Parse(parts[1]) / 100m : 0m);
        }
    }
}
