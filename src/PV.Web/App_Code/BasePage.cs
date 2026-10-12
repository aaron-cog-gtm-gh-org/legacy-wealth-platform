using System;
using System.Configuration;
using System.Data;
using log4net;
using PV.Data;

namespace PV.Web
{
    // RJM 2013-05: every page inherits this; set RequiredRoles on the page to
    // gate it. Session holds UserName / Role / UserId after login.
    // AT 2026-10: on the intranet build there is no login page - first hit on a
    // session signs on as the DefaultUser from web.config and stamps the session.
    public class BasePage : System.Web.UI.Page
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BasePage));

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
                SignOnDefaultUser();
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

        // same session stamp the old login page used to do
        private void SignOnDefaultUser()
        {
            string name = ConfigurationManager.AppSettings["DefaultUser"];
            DataRow u = UserData.GetUserByName(name);
            if (u == null || u["ACTIVE_FLG"].ToString() != "Y")
                throw new ApplicationException(
                    "DefaultUser '" + name + "' not found or inactive");

            Session["UserId"] = (int)u["USER_ID"];
            Session["UserName"] = u["USERNAME"].ToString();
            Session["Role"] = u["ROLE_CD"].ToString();
            UserData.SetLastLogin((int)u["USER_ID"]);
            Log.Info("intranet sign-on as " + u["USERNAME"]);
        }
    }
}
