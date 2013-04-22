using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PV.Data
{
    // RJM 2013-04: one place for the connection string + the boring ADO bits.
    // All callers use the shared static methods; no pooling tricks needed here.
    public static class SqlHelper
    {
        private static string _connStr;

        public static string ConnectionString
        {
            get
            {
                if (_connStr == null)
                    _connStr = ConfigurationManager.ConnectionStrings["PVRef"].ConnectionString;
                return _connStr;
            }
            // tests point this at a localdb/express instance
            set { _connStr = value; }
        }

        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parms)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (parms != null) cmd.Parameters.AddRange(parms);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parms)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (parms != null) cmd.Parameters.AddRange(parms);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] parms)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (parms != null) cmd.Parameters.AddRange(parms);
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static int ExecuteProcNonQuery(string procName, params SqlParameter[] parms)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parms != null) cmd.Parameters.AddRange(parms);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable ExecuteProcDataTable(string procName, params SqlParameter[] parms)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(procName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parms != null) cmd.Parameters.AddRange(parms);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
        }

        // RJM 2013-04: original helper, still used by the Admin page user list.
        // Everything else moved to ExecuteDataTable. Keep for compatibility.
        public static DataSet ExecuteDataSet(string sql, params SqlParameter[] parms)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (parms != null) cmd.Parameters.AddRange(parms);
                DataSet ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cn.Open();
                    da.Fill(ds);
                }
                return ds;
            }
        }
    }
}
