using System;
using System.Collections.Generic;
using PV.Core;
using PV.Data;

namespace PV.Web
{
    public partial class Holdings : BasePage
    {
        private AccountRow _acct;

        protected void Page_Load(object sender, EventArgs e)
        {
            int acctId;
            if (!int.TryParse(Request["acct"], out acctId)) { Response.Redirect("ClientSearch.aspx"); return; }
            _acct = AccountData.GetAccount(acctId);
            if (_acct == null) { Response.Redirect("ClientSearch.aspx"); return; }

            if (!IsPostBack)
            {
                lblAcct.Text = _acct.ACCT_NO;
                lblClient.Text = _acct.CLIENT_NAME;
                lblSafe.Text = _acct.SAFE_CODE;

                List<DateTime> dates = PositionBuilder.GetStatementDates(_acct.SAFE_CODE);
                foreach (DateTime d in dates)
                    ddlDate.Items.Add(new System.Web.UI.WebControls.ListItem(
                        DateUtil.FormatDate(d), d.ToString("yyyyMMdd")));

                DateTime stmt;
                if (Request["stmt"] != null &&
                    DateTime.TryParseExact(Request["stmt"], "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out stmt))
                {
                    var li = ddlDate.Items.FindByValue(Request["stmt"]);
                    if (li != null) li.Selected = true;
                }
                else if (dates.Count > 0)
                {
                    stmt = dates[0];
                }
                else
                {
                    lblMsg.Text = "No statement on file for this date.";
                    return;
                }
                ShowStatement(stmt);
            }
        }

        protected void ddlDate_Changed(object sender, EventArgs e)
        {
            Response.Redirect("Holdings.aspx?acct=" + _acct.ACCT_ID + "&stmt=" + ddlDate.SelectedValue);
        }

        private void ShowStatement(DateTime stmt)
        {
            Mt535Statement st = PositionBuilder.GetStatement(_acct.SAFE_CODE, stmt);
            if (st == null)
            {
                lblMsg.Text = "No statement on file for this date.";
                return;
            }
            if (st.Lines.Count == 0)
            {
                lblMsg.Text = "No holdings on statement.";
                return;
            }

            var rows = new List<object>();
            decimal total = 0m;
            foreach (Mt535Line l in st.Lines)
            {
                total += l.MktValGbp;
                rows.Add(new
                {
                    Isin = l.Isin,
                    Instrument = PositionBuilder.InstrumentName(l.Isin),
                    QtyTxt = WebUtil.FmtQty(l.Qty),
                    PxTxt = WebUtil.FmtPx(l.Price),
                    Ccy = l.Ccy,
                    MvTxt = WebUtil.FmtMoney(l.MktVal),
                    MvGbpTxt = WebUtil.FmtMoney(l.MktValGbp)
                });
            }
            gv.DataSource = rows;
            gv.DataBind();
            lblTotal.Text = WebUtil.FmtMoney(total);
        }
    }
}
