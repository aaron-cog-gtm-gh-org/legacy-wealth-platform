using System;
using System.Configuration;

namespace PV.Web
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblErr.Text = "An error has occurred. Please contact Support (ext. "
                + ConfigurationManager.AppSettings["SupportExt"] + ").";
        }
    }
}
