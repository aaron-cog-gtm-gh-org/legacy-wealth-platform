using System;

namespace PV.Web
{
    // RJM 2013-05: every page inherits this; set RequiredRoles on the page to
    // gate it. Session holds UserName / Role / UserId after login.
    public class BasePage : System.Web.UI.Page
    {
        public string[] RequiredRoles;

        protected string CurrentUser { get { return Session["UserName"] as string; } }
        protected string CurrentRole { get { return Session["Role"] as string ?? ""; } }
        protected int CurrentUserId
        {
            get { return Session["UserId"] == null ? 0 : (int)Session["UserId"]; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Session["UserName"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (RequiredRoles != null && RequiredRoles.Length > 0)
            {
                bool ok = false;
                foreach (string r in RequiredRoles)
                    if (r == CurrentRole) { ok = true; break; }
                if (!ok)
                    Response.Redirect("ClientSearch.aspx?err=1");
            }
        }
    }
}
