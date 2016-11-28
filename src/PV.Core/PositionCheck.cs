using System;
using System.Collections.Generic;
using PV.Data;

namespace PV.Core
{
    // KP 2016-11: statement-vs-settlements check run by ops each Monday for the
    // Friday statement. Anything that does not foot gets a row with the note
    // status from RECON_NOTE.
    public class PositionCheckRow
    {
        public string Isin;
        public string Instrument;
        public decimal PrevQty;
        public decimal NetSettled;
        public decimal Expected;
        public decimal StmtQty;
        public decimal Diff;
        public string Status;
        public string Note;
        public string UpdatedBy;
        public DateTime? UpdatedDt;
    }

    public class PositionCheckResult
    {
        public DateTime? PrevStmtDate;
        public string NoPrevMessage;           // set when no previous statement
        public List<PositionCheckRow> Rows = new List<PositionCheckRow>();
        public Mt535Statement Statement;
    }

    public static class PositionCheck
    {
        public static PositionCheckResult Run(AccountRow acct, DateTime stmtDate)
        {
            var res = new PositionCheckResult();

            Mt535Statement current = PositionBuilder.GetStatement(acct.SAFE_CODE, stmtDate);
            res.Statement = current;
            Mt535Statement previous = PositionBuilder.GetPrevious(acct.SAFE_CODE, stmtDate);
            if (previous == null)
            {
                res.NoPrevMessage = "No previous statement on file.";
                return res;
            }
            res.PrevStmtDate = previous.StatementDate;

            List<SettlementConfirm> settles = SettlementReader.GetSettlements(
                acct.SAFE_CODE.PadLeft(7, '0'), previous.StatementDate.AddDays(1), stmtDate);

            var prevQty = new Dictionary<string, decimal>();
            foreach (Mt535Line l in previous.Lines) prevQty[l.Isin] = l.Qty;

            var stmtQty = new Dictionary<string, decimal>();
            if (current != null)
                foreach (Mt535Line l in current.Lines) stmtQty[l.Isin] = l.Qty;

            var net = new Dictionary<string, decimal>();
            var isins = new HashSet<string>(prevQty.Keys);
            foreach (string k in stmtQty.Keys) isins.Add(k);
            foreach (SettlementConfirm s in settles)
            {
                isins.Add(s.Isin);
                decimal q = net.ContainsKey(s.Isin) ? net[s.Isin] : 0m;
                net[s.Isin] = s.Direction == "REC" ? q + s.Qty : q - s.Qty;
            }

            // TODO KP 2017-01: corporate actions come through as plain stmt
            // movements with no MT54x - ops note them by hand for now
            var notes = ReconNoteData.GetNotes(acct.ACCT_ID, stmtDate);

            foreach (string isin in isins)
            {
                decimal pq = prevQty.ContainsKey(isin) ? prevQty[isin] : 0m;
                decimal ns = net.ContainsKey(isin) ? net[isin] : 0m;
                decimal sq = stmtQty.ContainsKey(isin) ? stmtQty[isin] : 0m;
                decimal expected = pq + ns;
                if (expected - sq == 0m) continue;
                // KP 2016-12 tolerance for fund rounding - ops said no, show everything
                //if (Math.Abs(expected - sq) < 0.01m) continue;

                var row = new PositionCheckRow();
                row.Isin = isin;
                row.Instrument = PositionBuilder.InstrumentName(isin);
                row.PrevQty = pq;
                row.NetSettled = ns;
                row.Expected = expected;
                row.StmtQty = sq;
                row.Diff = sq - expected;

                ReconNote n;
                if (notes.TryGetValue(isin, out n))
                {
                    row.Status = n.Status;
                    row.Note = n.Note;
                    row.UpdatedBy = n.UpdatedBy;
                    row.UpdatedDt = n.UpdatedDt;
                }
                else
                {
                    row.Status = "NEW";
                }
                res.Rows.Add(row);
            }

            res.Rows.Sort((a, b) => string.CompareOrdinal(a.Isin, b.Isin));
            return res;
        }
    }
}
