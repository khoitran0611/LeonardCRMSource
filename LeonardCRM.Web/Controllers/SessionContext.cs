using System;
using System.Web;
using System.Web.Security;
using Eli.Common;
using LeonardCRM.BusinessLayer.Security;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;

namespace LeonardCRM.Web.Controllers
{
    public class SessionContext
    {
        public void SetAuthenticationToken(string name, string password, bool isPersistant, Eli_User userData)
        {
            #region Clear some data so it's enough to save in cookie(4KB)
            userData.LastLogin = null;
            userData.ModifiedBy = null;
            userData.ModifiedDate = null;
            userData.ActivationCode = null;
            userData.Eli_Roles.Description = null;
            userData.Password = "";
            userData.SalesCustomerUsers = null;
            userData.SalesOrdersUsers = null;
            #endregion

            var data = JsonConvert.SerializeObject(userData, Formatting.None, new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects, NullValueHandling = NullValueHandling.Ignore });

            if (isPersistant)
                UserSecurity.StoreInformationOnCookies(name, password);
            else
                UserSecurity.RemoveUserOnCookies();

            var ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddDays(ConfigValues.AUTHEN_COOKIE_EXPIRE_TIME), isPersistant, data);

            var cookieData = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
            {
                HttpOnly = true,
            };
            HttpContext.Current.Session[Constant.ss_UserId] = userData.Id;
            HttpContext.Current.Response.Cookies.Add(cookie);
            cookie = new HttpCookie("user", userData.Id.ToString());
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public Eli_User GetUserData()
        {
            Eli_User userData = null;
            try
            {
                var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    userData = JsonConvert.DeserializeObject(ticket.UserData, typeof(Eli_User)) as Eli_User;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
            }

            return userData;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}