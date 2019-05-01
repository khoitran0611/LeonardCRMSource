using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Eli.Common;
using LeonardCRM.BusinessLayer;
using LeonardCRM.Web.Controllers;
using Newtonsoft.Json;

namespace LeonardCRM.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {

            //working for entity framework
            Settings.ConnectionString = ConfigurationManager.ConnectionStrings["LeonardCRM"].Name;
            Settings.DefaultLanguage = RegistryBM.Instance.Single(r=>r.Name == "DEFAULT_LANGUAGE").Value;

            log4net.Config.XmlConfigurator.Configure();

            // Get the Hierarchy object that organizes the loggers
            var hier = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
            if (hier != null)
            {
                //get ADONetAppender
                var adoAppender =
                  (log4net.Appender.AdoNetAppender)hier.GetLogger("LeonardCRM",
                    hier.LoggerFactory).GetAppender("AdoNetAppender_SqlServer");
                if (adoAppender != null)
                {
                    adoAppender.ConnectionString = ConfigurationManager.ConnectionStrings["log4netConStr"].ConnectionString;
                    adoAppender.ActivateOptions(); //refresh settings of appender
                }
            }

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Response.Clear();

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                //string action;

                switch (httpException.GetHttpCode())
                {
                    //case 404:
                    // page not found
                    //    action = "HttpError404";
                    //    break;
                    case 500:
                        // server error
                        //action = "HttpError500";
                        log4net.GlobalContext.Properties["Url"] = Request.RawUrl;
                        var user = new SessionContext().GetUserData();
                        if (user != null)
                            log4net.GlobalContext.Properties["User"] = user.LoginName;
                        LogHelper.Log(exception.Message, exception);
                        break;
                    //default:
                    //    action = "General";
                    //    break;
                }

                // clear error on server
                //Server.ClearError();
                //UriHelper.Redirect(String.Format("~/Error/{0}/?message={1}", action, exception.Message));
            }
            else
            {
                log4net.GlobalContext.Properties["Url"] = Request.RawUrl;
                var user = new SessionContext().GetUserData();
                if (user != null)
                    log4net.GlobalContext.Properties["User"] = user.LoginName;
                LogHelper.Log(exception.Message, exception);
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }
       
    }
}