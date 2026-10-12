using System;

namespace PV.Web
{
    // AT 2026-10: intranet landing page - straight to the client list
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("ClientSearch.aspx");
        }
    }
}
