using System;
using System.Data;
using PV.Data;

namespace PV.Web
{
    public partial class ClientSearch : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["err"] == "1")
                lblErr.Text = "You do not have access to that page.";
            if (!IsPostBack && Session["LastClientSearch"] is DataTable)
                Bind(Session["LastClientSearch"] as DataTable);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = ClientData.Search(txtSearch.Text);
            dt.Columns.Add("STATUS_TXT");
            foreach (DataRow r in dt.Rows)
                r["STATUS_TXT"] = WebUtil.StatusText(r["STATUS_CD"].ToString());
            Session["LastClientSearch"] = dt;
            Bind(dt);
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
