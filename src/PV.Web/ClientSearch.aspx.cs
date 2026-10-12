using System;
using System.Data;
using PV.Core;
using PV.Data;

namespace PV.Web
{
    // AT 2026-10: this is the landing page now, so the full client list (with
    // latest statement figures) shows straight away. Search box filters it.
    public partial class ClientSearch : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["err"] == "1")
                lblErr.Text = "You do not have access to that page.";
            if (!IsPostBack)
                DoSearch("");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DoSearch(txtSearch.Text);
        }

        protected void btnAll_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            DoSearch("");
        }

        private void DoSearch(string text)
        {
            DataTable dt = ClientData.Search(text);
            dt.Columns.Add("STATUS_TXT");
            dt.Columns.Add("STMT_DATE");
            dt.Columns.Add("MKTVAL");
            dt.Columns.Add("MV_RAW", typeof(decimal));

            decimal total = 0m;
            foreach (DataRow r in dt.Rows)
            {
                r["STATUS_TXT"] = WebUtil.StatusText(r["STATUS_CD"].ToString());

                // latest statement figures across the client's accounts
                DataTable accts = AccountData.GetAccountsByClient((int)r["CLIENT_ID"]);
                var safes = new string[accts.Rows.Count];
                for (int i = 0; i < accts.Rows.Count; i++)
                    safes[i] = accts.Rows[i]["SAFE_CODE"] == DBNull.Value
                        ? null : accts.Rows[i]["SAFE_CODE"].ToString();

                PortfolioTotals.Snapshot s = PortfolioTotals.ForSafeCodes(safes);
                if (s.HasData)
                {
                    r["STMT_DATE"] = DateUtil.FormatDate(s.StmtDate.Value);
                    r["MKTVAL"] = WebUtil.FmtMoney(s.MktValGbp);
                    r["MV_RAW"] = s.MktValGbp;
                    total += s.MktValGbp;
                }
                else
                {
                    r["STMT_DATE"] = "-";
                    r["MKTVAL"] = "-";
                }
            }

            Session["LastClientSearch"] = dt;
            Bind(dt);
            lblTotal.Text = WebUtil.FmtMoney(total);
            lblCount.Text = dt.Rows.Count + (dt.Rows.Count == 1 ? " client" : " clients");
        }

        private void Bind(DataTable dt)
        {
            gv.DataSource = dt;
            gv.DataBind();
            lblMsg.Text = dt.Rows.Count == 0 ? "No clients found." : "";
        }

        protected void gv_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gv.PageIndex = e.NewPageIndex;
            Bind(Session["LastClientSearch"] as DataTable);
        }
    }
}
