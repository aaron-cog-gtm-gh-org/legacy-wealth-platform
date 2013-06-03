using System.Data;
using System.Data.SqlClient;

namespace PV.Data
{
    // DW 2014-02: client search uses the proc, everything else is plain SQL.
    public static class ClientData
    {
        public static DataTable Search(string text)
        {
            return SqlHelper.ExecuteProcDataTable("usp_ClientSearch",
                new SqlParameter("@SearchText", text ?? string.Empty));
        }

        public static DataRow GetClient(int clientId)
        {
            DataTable dt = SqlHelper.ExecuteDataTable(
                "SELECT c.CLIENT_ID, c.CLIENT_REF, c.CLIENT_NAME, c.STATUS_CD, c.CREATED_DT, " +
                "       u.USERNAME AS ADVISER_USERNAME, u.FULL_NAME AS ADVISER_NAME " +
                "FROM CLIENT c LEFT JOIN APP_USER u ON u.USER_ID = c.ADVISER_USER_ID " +
                "WHERE c.CLIENT_ID = @id",
                new SqlParameter("@id", clientId));
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }
    }
}
