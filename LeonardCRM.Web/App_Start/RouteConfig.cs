using System.Web.Mvc;
using System.Web.Routing;

namespace LeonardCRM.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Deny",
                url: "deny",
                defaults: new { controller = "Home", action = "Deny" }
            );
            routes.MapRoute(
                name: "Error",
                url: "error",
                defaults: new { controller = "Home", action = "Error" }
            );
            routes.MapRoute(
                name: "NotFound",
                url: "notfound",
                defaults: new { controller = "Home", action = "NotFound" }
            );
            routes.MapRoute(
               name: "DeliveryRequest",
               url: "public/delivery",
               defaults: new { controller = "Anonymous", action = "DeliveryRequestView"}
            );
            routes.MapRoute(
               name: "CustomerAcceptance",
               url: "public/customer-accpetance",
               defaults: new { controller = "Anonymous", action = "CustomerAcceptanceView" }
            );
            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Account", action = "Login" }
            );
            routes.MapRoute(
                name: "PasswordRecovery",
                url: "PasswordRecovery",
                defaults: new { controller = "Account", action = "PasswordRecovery" }
            );
            routes.MapRoute(
               name: "ResetPassword",
               url: "ResetPassword",
               defaults: new { controller = "Account", action = "ResetPassword" }
           );
            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new { controller = "Account", action = "Register" }
            );
            routes.MapRoute(
               name: "Activation",
               url: "Activation",
               defaults: new { controller = "Account", action = "Activation" }
           );
            routes.MapRoute(
                name: "Admin",
                url: "admin",
                defaults: new { controller = "Home", action = "Index" }
            );
            routes.MapRoute(
                name: "SurveyCustomer",
                url: "survey",
                defaults: new { controller = "Public", action = "Survey", token = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SurveyComplete",
                url: "surveycomlete",
                defaults: new { controller = "Public", action = "SurveyComplete", token = UrlParameter.Optional }
            );          
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Public", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Dynamic_Grid",
                url: "{controller}/DynamicGrid/{viewId}/{moduleId}/{id}",
                defaults: new { controller = "Home", action = "DynamicGrid", viewId = UrlParameter.Optional, moduleId = UrlParameter.Optional,id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Sub_View",
                url: "{controller}/SubView/{viewId}/{moduleId}/{id}",
                defaults: new { controller = "Home", action = "SubView", viewId = UrlParameter.Optional, moduleId = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Related_View",
                url: "{controller}/RelatedView/{viewId}/{id}",
                defaults: new { controller = "Home", action = "RelatedView", viewId = UrlParameter.Optional, moduleId = UrlParameter.Optional, id = UrlParameter.Optional }
            );          
        }
    }
}