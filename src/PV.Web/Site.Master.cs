using System;
using System.Web.Security;

namespace PV.Web
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                pnlUser.Visible = false;
            }
            else
            {
                lblUser.Text = Session["UserName"].ToString();
            }
            string role = Session["Role"] as string ?? "";
            phOps.Visible = role == "OPS" || role == "ADMIN";
            phAdmin.Visible = role == "ADMIN";
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
