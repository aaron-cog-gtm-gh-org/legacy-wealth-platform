using System;
using System.Configuration;
using System.IO;
using System.Web;
using log4net;
using PV.Core;

namespace PV.Web
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Global));

        // archive root is relative to the site folder; resolves to repo\archive
        public static string ArchiveRoot
        {
            get
            {
                string cfg = ConfigurationManager.AppSettings["ArchiveRoot"];
                return Path.GetFullPath(Path.Combine(
                    HttpContext.Current != null
                        ? HttpContext.Current.Server.MapPath("~")
                        : AppDomain.CurrentDomain.BaseDirectory,
                    cfg));
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            Log.Info("Portfolio Viewer v3.2.1 (build 2018.11) starting");
            ArchiveIndex.Reload(ArchiveRoot);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null) Log.Error("Unhandled exception", ex);
        }
    }
}
