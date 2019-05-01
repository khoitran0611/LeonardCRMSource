using System.Web.Http;
using LeonardCRM.BusinessLayer.DataControllers;

namespace LeonardCRM.BusinessLayer.Security
{
    public class RequireAuthorization
    {
        [RequireAuthorize]
        public class AuthController : BaseApiController
        {
            [AllowAnonymous]
            [HttpPost]
            public AuthResult Login(/*LoginModel model*/)
            {
                //string cookieToken = "", formToken = "";
                //if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                //{
                //    HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(model.UserName), null);
                //    AntiForgery.GetTokens(null, out cookieToken, out formToken);
                //    // obtain the new "logged in" anti-forgery cookie from here
                //    HttpContext.Current.Response.Cookies[AntiForgeryConfig.CookieName].Value = cookieToken;
                //    return new AuthResult() { Result = true, Form = formToken };
                //}
                //else
                //{
                //    return new AuthResult { Result = false, Form = formToken };
                //}
                return new AuthResult();
            }
        }
    }

    public class AuthResult
    {
        public bool Result { get; set; }
        public string Form { get; set; }
    }
}
