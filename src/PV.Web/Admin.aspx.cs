using System;
using System.Data;
using PV.Core;
using PV.Data;

namespace PV.Web
{
    public partial class Admin : BasePage
    {
        public Admin()
        {
            RequiredRoles = new[] { "ADMIN" };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            BindStats();
            BindUsers();
        }

        private void BindStats()
        {
            var idx = ArchiveIndex.Current;
            if (idx == null)
            {
                lblMsg.Text = "Archive not loaded.";
                return;
            }
            lblRoot.Text = idx.Root;
            lbl535.Text = idx.Mt535Count.ToString();
            lbl54x.Text = idx.Mt54xCount.ToString();
            int stmts = 0, confs = 0;
            foreach (var byDate in idx.Statements.Values)
                foreach (var day in byDate.Values) stmts += day.Count;
            foreach (var confs2 in idx.ConfirmFolders.Values) confs += confs2.Count;
            lblStmts.Text = stmts.ToString();
            lblConfs.Text = confs.ToString();
            lblErrs.Text = idx.ParseErrors.ToString();
            lblLoaded.Text = WebUtil.FmtDt(idx.LoadedAt) + " " + idx.LoadedAt.ToString("HH:mm:ss");
            lblMs.Text = idx.LoadMs + " ms";
        }

        private void BindUsers()
        {
            DataTable dt = UserData.GetAllUsers();
            dt.Columns.Add("LAST_LOGIN");
            foreach (DataRow r in dt.Rows)
                r["LAST_LOGIN"] = r["LAST_LOGIN_DT"] == DBNull.Value
                    ? "" : WebUtil.FmtDt(r["LAST_LOGIN_DT"]) + " " +
                      ((DateTime)r["LAST_LOGIN_DT"]).ToString("HH:mm");
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            ArchiveIndex.Reload(Global.ArchiveRoot);
            InstrumentData.ResetCache();
            BindStats();
            lblMsg.Text = "Archive reloaded.";
        }
    }
}
