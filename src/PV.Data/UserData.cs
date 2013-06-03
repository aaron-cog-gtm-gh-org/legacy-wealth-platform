using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace PV.Data
{
    // RJM 2013-05: login goes through the procs, passwords are SHA1 upper-hex.
    // FormsAuthentication does the hash for us, same as the old membership table.
    public static class UserData
    {
        public static DataRow GetUserByName(string userName)
        {
            DataTable dt = SqlHelper.ExecuteProcDataTable("usp_GetUserByName",
                new SqlParameter("@Username", userName));
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static string HashPassword(string pwd)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "SHA1");
        }

        public static void SetLastLogin(int userId)
        {
            SqlHelper.ExecuteProcNonQuery("usp_SetLastLogin",
                new SqlParameter("@UserId", userId));
        }

        public static DataTable GetAllUsers()
        {
            return SqlHelper.ExecuteDataSet(
                "SELECT USER_ID, USERNAME, FULL_NAME, ROLE_CD, ACTIVE_FLG, LAST_LOGIN_DT " +
                "FROM APP_USER ORDER BY USERNAME").Tables[0];
        }
    }
}
