using System;
using System.Data;
using System.Data.SqlClient;

namespace PV.Data
{
    // KP 2016-10: notes/statuses entered on the position check screen.
    public class ReconNote
    {
        public int AcctId;
        public string Isin;
        public DateTime StmtDate;
        public string Status;
        public string Note;
        public string UpdatedBy;
        public DateTime? UpdatedDt;
    }

    public static class ReconNoteData
    {
        public static System.Collections.Generic.Dictionary<string, ReconNote> GetNotes(int acctId, DateTime stmtDate)
        {
            var map = new System.Collections.Generic.Dictionary<string, ReconNote>(StringComparer.OrdinalIgnoreCase);
            DataTable dt = SqlHelper.ExecuteDataTable(
                "SELECT ACCT_ID, ISIN, STMT_DT, STATUS_CD, NOTE_TXT, UPDATED_BY, UPDATED_DT " +
                "FROM RECON_NOTE WHERE ACCT_ID = @a AND STMT_DT = @d",
                new SqlParameter("@a", acctId), new SqlParameter("@d", stmtDate));
            foreach (DataRow r in dt.Rows)
            {
                var n = new ReconNote();
                n.AcctId = (int)r["ACCT_ID"];
                n.Isin = r["ISIN"].ToString().Trim();
                n.StmtDate = (DateTime)r["STMT_DT"];
                n.Status = r["STATUS_CD"].ToString();
                n.Note = r["NOTE_TXT"] == DBNull.Value ? null : r["NOTE_TXT"].ToString();
                n.UpdatedBy = r["UPDATED_BY"] == DBNull.Value ? null : r["UPDATED_BY"].ToString();
                n.UpdatedDt = r["UPDATED_DT"] == DBNull.Value ? (DateTime?)null : (DateTime)r["UPDATED_DT"];
                map[n.Isin] = n;
            }
            return map;
        }

        // update first; if nothing matched it is a new row
        public static void SaveNote(int acctId, string isin, DateTime stmtDate,
            string status, string note, string userName)
        {
            int rows = SqlHelper.ExecuteNonQuery(
                "UPDATE RECON_NOTE SET STATUS_CD=@s, NOTE_TXT=@n, UPDATED_BY=@u, UPDATED_DT=GETDATE() " +
                "WHERE ACCT_ID=@a AND ISIN=@i AND STMT_DT=@d",
                new SqlParameter("@s", status),
                new SqlParameter("@n", (object)note ?? DBNull.Value),
                new SqlParameter("@u", userName),
                new SqlParameter("@a", acctId),
                new SqlParameter("@i", isin),
                new SqlParameter("@d", stmtDate));
            if (rows == 0)
            {
                SqlHelper.ExecuteNonQuery(
                    "INSERT INTO RECON_NOTE (ACCT_ID, ISIN, STMT_DT, STATUS_CD, NOTE_TXT, UPDATED_BY, UPDATED_DT) " +
                    "VALUES (@a, @i, @d, @s, @n, @u, GETDATE())",
                    new SqlParameter("@a", acctId),
                    new SqlParameter("@i", isin),
                    new SqlParameter("@d", stmtDate),
                    new SqlParameter("@s", status),
                    new SqlParameter("@n", (object)note ?? DBNull.Value),
                    new SqlParameter("@u", userName));
            }
        }
    }
}
