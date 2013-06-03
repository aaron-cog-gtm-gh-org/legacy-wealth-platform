using System;
using System.Data;
using PV.Core;
using PV.Data;

namespace PV.Web
{
    public partial class ClientSummary : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            int cid;
            if (!int.TryParse(Request["cid"], out cid)) { Response.Redirect("ClientSearch.aspx"); return; }

            DataRow c = ClientData.GetClient(cid);
            if (c == null) { Response.Redirect("ClientSearch.aspx"); return; }

            lblRef.Text = c["CLIENT_REF"].ToString();
            lblName.Text = c["CLIENT_NAME"].ToString();
            lblAdviser.Text = c["ADVISER_NAME"] == DBNull.Value ? "" : c["ADVISER_NAME"].ToString();
            lblStatus.Text = WebUtil.StatusText(c["STATUS_CD"].ToString());
            lblSince.Text = c["CREATED_DT"] == DBNull.Value ? "" : DateUtil.FormatDate((DateTime)c["CREATED_DT"]);

            DataTable dt = AccountData.GetAccountsByClient(cid);
            dt.Columns.Add("STATUS_TXT");
            dt.Columns.Add("OPENED");
            dt.Columns.Add("CLOSED");
            dt.Columns.Add("STMT_DATE");
            dt.Columns.Add("STMT_RAW", typeof(DateTime));
            dt.Columns.Add("MKTVAL");

            decimal total = 0m;
            bool any = false;
            foreach (DataRow r in dt.Rows)
            {
                r["STATUS_TXT"] = r["STATUS_CD"].ToString() == "O" ? "Open" : "Closed";
                r["OPENED"] = r["OPEN_DT"] == DBNull.Value ? "" : DateUtil.FormatDate((DateTime)r["OPEN_DT"]);
                r["CLOSED"] = r["CLOSE_DT"] == DBNull.Value ? "" : DateUtil.FormatDate((DateTime)r["CLOSE_DT"]);

                Mt535Statement st = PositionBuilder.GetLatest(r["SAFE_CODE"].ToString());
                if (st != null)
                {
                    r["STMT_DATE"] = DateUtil.FormatDate(st.StatementDate);
                    r["STMT_RAW"] = st.StatementDate;
                    decimal mv = 0m;
                    foreach (Mt535Line l in st.Lines) mv += l.MktValGbp;
                    r["MKTVAL"] = WebUtil.FmtMoney(mv);
                    total += mv;
                    any = true;
                }
                else
                {
                    r["STMT_DATE"] = "";
                    r["MKTVAL"] = "-";
                }
            }

            gv.DataSource = dt;
            gv.DataBind();
            lblTotal.Text = any ? WebUtil.FmtMoney(total) : "-";
        }
    }
}
