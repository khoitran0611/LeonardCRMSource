using Eli.Common;
using LeonardCRM.BusinessLayer.HttpModules;

namespace LeonardCRM.BusinessLayer.Feature
{
    public sealed  class AccountFeature : Feature
    {
        public LoginStatus Login(string username, string password, bool isRemember, bool needEncryptPass)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var am = new AuthenticationModule();
                var loginStatus = am.AuthenticateUser(username, password, isRemember, needEncryptPass);
                //if (loginStatus == LoginStatus.Success)
                //{
                //    //var cookie = new HttpCookie(ConfigValues.SESSION_ID)
                //    //    {
                //    //        Expires = DateTime.Now.AddDays(1),
                //    //        Value = HttpContext.Current.Session.SessionID
                //    //    };
                //    //HttpContext.Current.Response.Cookies.Add(cookie);
                //    //FormsAuthentication.RedirectFromLoginPage(HttpContext.Current.Session[Constant.ss_UserId].ToString(), isRemember);
                //}
                return loginStatus;
            }
            return LoginStatus.NoAccount;
        }

        //public LoginStatus Login(string username, string password, bool isRemember, bool isReturn)
        //{
        //    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        //    {
        //        var am = new AuthenticationModule();
        //        var loginStatus = am.AuthenticateUser(username, password, isRemember);
        //        if (loginStatus == LoginStatus.Success)
        //        {
        //            if (isReturn)
        //                if (HttpContext.Current.Request.UrlReferrer != null)
        //                    FormsAuthentication.RedirectFromLoginPage(HttpContext.Current.Request.UrlReferrer.ToString(), isRemember);
        //        }
        //        return loginStatus;
        //    }
        //    return LoginStatus.NoAccount; 
        //}
    }
}
