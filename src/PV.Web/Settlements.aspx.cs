using System;
using System.Collections.Generic;
using PV.Core;
using PV.Data;

namespace PV.Web
{
    public partial class Settlements : BasePage
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

                // default window: last 90 days ending at the latest statement
                DateTime to = DateTime.Today;
                Mt535Statement latest = PositionBuilder.GetLatest(_acct.SAFE_CODE);
                if (latest != null) to = latest.StatementDate;
                DateTime from = to.AddDays(-90);

                DateTime q;
                if (Request["from"] != null && WebUtil.TryParseUkDate(Request["from"], out q)) from = q;
                if (Request["to"] != null && WebUtil.TryParseUkDate(Request["to"], out q)) to = q;

                txtFrom.Text = from.ToString("dd/MM/yyyy");
                txtTo.Text = to.ToString("dd/MM/yyyy");
                Bind(from, to);
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            DateTime from, to;
            if (!WebUtil.TryParseUkDate(txtFrom.Text, out from) ||
                !WebUtil.TryParseUkDate(txtTo.Text, out to))
            {
                lblMsg.Text = "Enter dates as dd/MM/yyyy.";
                return;
            }
            Bind(from, to);
        }

        private void Bind(DateTime from, DateTime to)
        {
            List<SettlementConfirm> list = SettlementReader.GetSettlements(
                _acct.SAFE_CODE.PadLeft(7, '0'), from, to);

            var rows = new List<object>();
            foreach (SettlementConfirm c in list)
            {
                decimal amt = c.AmountRaw != null ? SettlementReader.ToAmount(c.AmountRaw) : 0m;
                rows.Add(new
                {
                    TradeTxt = DateUtil.FormatDate(c.TradeDate),
                    SettleTxt = DateUtil.FormatDate(c.SettleDate),
                    Msg = "MT" + c.MtType,
                    Dir = c.Direction,
                    Isin = c.Isin,
                    Instrument = PositionBuilder.InstrumentName(c.Isin),
                    QtyTxt = WebUtil.FmtQty(c.Qty),
                    AmtTxt = WebUtil.FmtMoney(amt),
                    Ccy = c.Ccy,
                    Seme = c.Seme
                });
            }
            gv.DataSource = rows;
            gv.DataBind();
            lblMsg.Text = rows.Count == 0 ? "No settlements in period." : "";
        }
    }
}
