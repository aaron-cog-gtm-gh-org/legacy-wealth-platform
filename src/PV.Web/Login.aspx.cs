using System;
using System.Configuration;
using System.Data;
using System.Web.Security;
using PV.Data;

namespace PV.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblErr.Text = "";
            DataRow u = UserData.GetUserByName(txtUser.Text);
            if (u == null ||
                !string.Equals(u["PWD_HASH"].ToString(),
                    UserData.HashPassword(txtPwd.Text),
                    StringComparison.OrdinalIgnoreCase))
            {
                lblErr.Text = "Invalid user ID or password.";
                return;
            }
            if (u["ACTIVE_FLG"].ToString() != "Y")
            {
                lblErr.Text = "Your account has been disabled. Please contact Support (ext. "
                    + ConfigurationManager.AppSettings["SupportExt"] + ").";
                return;
            }

            Session["UserId"] = (int)u["USER_ID"];
            Session["UserName"] = u["USERNAME"].ToString();
            Session["Role"] = u["ROLE_CD"].ToString();
            UserData.SetLastLogin((int)u["USER_ID"]);

            FormsAuthentication.SetAuthCookie(u["USERNAME"].ToString(), false);
            Response.Redirect("ClientSearch.aspx");
        }
    }
}
