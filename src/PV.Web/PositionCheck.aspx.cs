using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using PV.Core;
using PV.Data;

namespace PV.Web
{
    // KP 2016-11: ops run this per account/date or "Check All" for a statement
    // Friday. Rows can be edited inline to set status + note (RECON_NOTE).
    public partial class PositionCheck : BasePage
    {
        public PositionCheck()
        {
            RequiredRoles = new[] { "OPS", "ADMIN" };
        }

        private List<AccountRow> _accts;
        private int AcctId
        {
            get { return ViewState["acct"] == null ? 0 : (int)ViewState["acct"]; }
            set { ViewState["acct"] = value; }
        }
        private DateTime StmtDate
        {
            get { return ViewState["stmt"] == null ? DateTime.MinValue : (DateTime)ViewState["stmt"]; }
            set { ViewState["stmt"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _accts = AccountData.GetAllAccounts();

            if (!IsPostBack)
            {
                foreach (AccountRow a in _accts)
                    ddlAcct.Items.Add(new ListItem(
                        a.ACCT_NO + " - " + a.CLIENT_NAME, a.ACCT_ID.ToString()));

                // every distinct statement date across the archive, newest first
                var allDates = new SortedSet<DateTime>();
                var idx = ArchiveIndex.Current;
                if (idx != null)
                    foreach (var byDate in idx.Statements.Values)
                        foreach (DateTime d in byDate.Keys) allDates.Add(d);
                foreach (DateTime d in allDates)
                    ddlAllDate.Items.Insert(0, new ListItem(
                        DateUtil.FormatDate(d), d.ToString("yyyyMMdd")));

                int q;
                if (int.TryParse(Request["acct"], out q))
                {
                    var li = ddlAcct.Items.FindByValue(q.ToString());
                    if (li != null) li.Selected = true;
                }
                FillDates();

                DateTime sd;
                if (Request["stmt"] != null &&
                    DateTime.TryParseExact(Request["stmt"], "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out sd))
                {
                    var li = ddlDate.Items.FindByValue(Request["stmt"]);
                    if (li != null) { li.Selected = true; RunCheck(); }
                }
            }
        }

        private AccountRow SelAcct()
        {
            foreach (AccountRow a in _accts)
                if (a.ACCT_ID.ToString() == ddlAcct.SelectedValue) return a;
            return null;
        }

        private void FillDates()
        {
            ddlDate.Items.Clear();
            AccountRow a = SelAcct();
            if (a == null) return;
            foreach (DateTime d in PositionBuilder.GetStatementDates(a.SAFE_CODE))
                ddlDate.Items.Add(new ListItem(DateUtil.FormatDate(d), d.ToString("yyyyMMdd")));
        }

        protected void ddlAcct_Changed(object sender, EventArgs e)
        {
            FillDates();
            gv.DataSource = null; gv.DataBind();
            gvAll.DataSource = null; gvAll.DataBind();
            lblMsg.Text = "";
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            RunCheck();
        }

        protected void btnAll_Click(object sender, EventArgs e)
        {
            DateTime d;
            if (!DateTime.TryParseExact(ddlAllDate.SelectedValue, "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out d))
                return;

            var rows = new List<object>();
            foreach (AccountRow a in _accts)
            {
                if (!PositionBuilder.GetStatementDates(a.SAFE_CODE).Contains(d)) continue;
                PositionCheckResult r = PV.Core.PositionCheck.Run(a, d);
                rows.Add(new
                {
                    AcctNo = a.ACCT_NO,
                    Client = a.CLIENT_NAME,
                    Diffs = r.NoPrevMessage != null ? "-" : r.Rows.Count.ToString(),
                    LinkAcct = a.ACCT_ID,
                    LinkDate = d.ToString("yyyyMMdd")
                });
            }
            gvAll.DataSource = rows;
            gvAll.DataBind();
            gv.DataSource = null; gv.DataBind();
            lblMsg.Text = rows.Count == 0 ? "No statements on file for that date." : "";
        }

        private void RunCheck()
        {
            AccountRow a = SelAcct();
            DateTime d;
            if (a == null || ddlDate.SelectedValue == "" ||
                !DateTime.TryParseExact(ddlDate.SelectedValue, "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out d))
            {
                lblMsg.Text = "Pick an account and a statement date.";
                return;
            }
            AcctId = a.ACCT_ID;
            StmtDate = d;
            BindRows(PV.Core.PositionCheck.Run(a, d));
        }

        private void BindRows(PositionCheckResult r)
        {
            gvAll.DataSource = null; gvAll.DataBind();
            if (r.NoPrevMessage != null)
            {
                lblMsg.Text = r.NoPrevMessage;
                gv.DataSource = null; gv.DataBind();
                return;
            }
            var rows = new List<object>();
            foreach (PositionCheckRow row in r.Rows)
            {
                rows.Add(new
                {
                    Isin = row.Isin,
                    Instrument = row.Instrument,
                    PrevQtyTxt = WebUtil.FmtQty(row.PrevQty),
                    NetTxt = WebUtil.FmtQty(row.NetSettled),
                    ExpTxt = WebUtil.FmtQty(row.Expected),
                    StmtTxt = WebUtil.FmtQty(row.StmtQty),
                    DiffTxt = WebUtil.FmtQty(row.Diff),
                    Status = row.Status,
                    Note = row.Note,
                    UpdatedBy = row.UpdatedBy,
                    UpdatedTxt = row.UpdatedDt.HasValue ? WebUtil.FmtDt(row.UpdatedDt) : ""
                });
            }
            gv.DataSource = rows;
            gv.DataBind();
            lblMsg.Text = rows.Count == 0 ? "No differences." : "";
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gv.EditIndex = e.NewEditIndex;
            RebindCurrent();
        }

        protected void gv_RowCancel(object sender, GridViewCancelEditEventArgs e)
        {
            gv.EditIndex = -1;
            RebindCurrent();
        }

        protected void gv_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string isin = gv.DataKeys[e.RowIndex].Value.ToString();
            var ddl = (DropDownList)gv.Rows[e.RowIndex].FindControl("ddlStatus");
            var txt = (TextBox)gv.Rows[e.RowIndex].FindControl("txtNote");
            ReconNoteData.SaveNote(AcctId, isin, StmtDate, ddl.SelectedValue,
                txt.Text == "" ? null : txt.Text, CurrentUser);
            gv.EditIndex = -1;
            RebindCurrent();
            lblMsg.Text = "Notes saved.";
        }

        private void RebindCurrent()
        {
            AccountRow a = null;
            foreach (AccountRow x in _accts) if (x.ACCT_ID == AcctId) a = x;
            if (a != null && StmtDate != DateTime.MinValue)
                BindRows(PV.Core.PositionCheck.Run(a, StmtDate));
        }
    }
}
