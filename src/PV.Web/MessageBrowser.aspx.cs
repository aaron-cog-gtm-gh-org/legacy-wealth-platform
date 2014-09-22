using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using PV.Core;

namespace PV.Web
{
    // DW 2014-09: raw archive viewer for ops. Files are only served if their
    // full path sits under the archive root.
    public partial class MessageBrowser : BasePage
    {
        public MessageBrowser()
        {
            RequiredRoles = new[] { "OPS", "ADMIN" };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Request["type"] == "535") ddlType.SelectedValue = "MT535";
            else if (Request["type"] == "54x") ddlType.SelectedValue = "MT54x";

            if (Request["file"] != null)
            {
                ShowFile(Request["file"]);
                return;
            }
            if (Request["date"] != null) txtDate.Text = Request["date"];
            if (Request["date"] != null || Request["type"] != null) DoList();
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            DoList();
        }

        private void DoList()
        {
            var idx = ArchiveIndex.Current;
            if (idx == null) { lblMsg.Text = "Archive not loaded."; return; }

            DateTime? d = null;
            if (txtDate.Text.Trim() != "")
            {
                DateTime dt;
                if (!WebUtil.TryParseUkDate(txtDate.Text, out dt))
                {
                    lblMsg.Text = "Enter date as dd/MM/yyyy.";
                    return;
                }
                d = dt;
            }
            string safe = txtSafe.Text.Trim().TrimStart('0');

            var rows = new List<object>();
            foreach (ArchiveIndex.ArchiveFileEntry f in idx.Files)
            {
                if (ddlType.SelectedValue != "" && f.MsgType != ddlType.SelectedValue) continue;
                if (d.HasValue && f.Date != d.Value) continue;
                if (safe != "" && (f.SafeCode == null || f.SafeCode.TrimStart('0') != safe)) continue;
                rows.Add(new
                {
                    FileName = f.FileName,
                    MsgType = f.MsgType,
                    DateTxt = WebUtil.FmtDt(f.Date),
                    Size = f.Size
                });
            }
            gv.DataSource = rows;
            gv.DataBind();
            lblMsg.Text = rows.Count == 0 ? "No files match." : "";
            Session["MsgBrowserList"] = rows;
        }

        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv.PageIndex = e.NewPageIndex;
            gv.DataSource = Session["MsgBrowserList"];
            gv.DataBind();
        }

        private void ShowFile(string name)
        {
            var idx = ArchiveIndex.Current;
            if (idx == null) { lblMsg.Text = "Archive not loaded."; return; }

            // find the file in the index; never build a path from user input
            string full = null;
            foreach (ArchiveIndex.ArchiveFileEntry f in idx.Files)
                if (f.FileName == name) { full = f.FullPath; break; }

            string root = Path.GetFullPath(Global.ArchiveRoot);
            if (full == null ||
                !Path.GetFullPath(full).StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                lblMsg.Text = "File not found.";
                return;
            }
            lblPath.Text = full;
            litRaw.Text = Server.HtmlEncode(File.ReadAllText(full));
        }
    }
}
