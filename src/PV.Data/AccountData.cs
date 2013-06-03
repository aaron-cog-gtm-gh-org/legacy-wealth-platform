using System.Data;
using System.Data.SqlClient;

namespace PV.Data
{
    public class AccountRow
    {
        public int ACCT_ID;
        public string ACCT_NO;
        public int CLIENT_ID;
        public string ACCT_TYPE;
        public string BASE_CCY;
        public string SAFE_CODE;
        public System.DateTime? OPEN_DT;
        public System.DateTime? CLOSE_DT;
        public string STATUS_CD;
        public string CLIENT_NAME;

        public static AccountRow FromRow(DataRow r)
        {
            AccountRow a = new AccountRow();
            a.ACCT_ID = (int)r["ACCT_ID"];
            a.ACCT_NO = r["ACCT_NO"].ToString();
            a.CLIENT_ID = (int)r["CLIENT_ID"];
            a.ACCT_TYPE = r["ACCT_TYPE"] == System.DBNull.Value ? null : r["ACCT_TYPE"].ToString();
            a.BASE_CCY = r["BASE_CCY"].ToString();
            a.SAFE_CODE = r["SAFE_CODE"] == System.DBNull.Value ? null : r["SAFE_CODE"].ToString();
            a.OPEN_DT = r["OPEN_DT"] == System.DBNull.Value ? (System.DateTime?)null : (System.DateTime)r["OPEN_DT"];
            a.CLOSE_DT = r["CLOSE_DT"] == System.DBNull.Value ? (System.DateTime?)null : (System.DateTime)r["CLOSE_DT"];
            a.STATUS_CD = r["STATUS_CD"].ToString();
            if (r.Table.Columns.Contains("CLIENT_NAME") && r["CLIENT_NAME"] != System.DBNull.Value)
                a.CLIENT_NAME = r["CLIENT_NAME"].ToString();
            return a;
        }
    }

    // RJM 2013-06: account lookups. Safe code stored without padding, the
    // statements pad to 7 - keep that in mind when joining to the archive.
    public static class AccountData
    {
        private const string SelectCols =
            "a.ACCT_ID, a.ACCT_NO, a.CLIENT_ID, a.ACCT_TYPE, a.BASE_CCY, a.SAFE_CODE, " +
            "a.OPEN_DT, a.CLOSE_DT, a.STATUS_CD, c.CLIENT_NAME ";

        public static DataTable GetAccountsByClient(int clientId)
        {
            return SqlHelper.ExecuteDataTable(
                "SELECT " + SelectCols + "FROM ACCOUNT a JOIN CLIENT c ON c.CLIENT_ID = a.CLIENT_ID " +
                "WHERE a.CLIENT_ID = @cid ORDER BY a.ACCT_NO",
                new SqlParameter("@cid", clientId));
        }

        public static AccountRow GetAccount(int acctId)
        {
            DataTable dt = SqlHelper.ExecuteDataTable(
                "SELECT " + SelectCols + "FROM ACCOUNT a JOIN CLIENT c ON c.CLIENT_ID = a.CLIENT_ID " +
                "WHERE a.ACCT_ID = @id",
                new SqlParameter("@id", acctId));
            return dt.Rows.Count == 0 ? null : AccountRow.FromRow(dt.Rows[0]);
        }

        public static System.Collections.Generic.List<AccountRow> GetAllAccounts()
        {
            DataTable dt = SqlHelper.ExecuteDataTable(
                "SELECT " + SelectCols + "FROM ACCOUNT a JOIN CLIENT c ON c.CLIENT_ID = a.CLIENT_ID " +
                "ORDER BY a.ACCT_NO");
            var list = new System.Collections.Generic.List<AccountRow>();
            foreach (DataRow r in dt.Rows) list.Add(AccountRow.FromRow(r));
            return list;
        }
    }
}
