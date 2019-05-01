using System;
using System.Web;
using System.Web.Security;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.HttpModules
{
    public class AuthenticationModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            //application.AuthenticateRequest += application_AuthenticateRequest;
        }

        void application_AuthenticateRequest(object sender, EventArgs e)
        {
            //var culture = Thread.CurrentThread.CurrentCulture;
            //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            //var app = (HttpApplication)sender;
            //if (app.Context.User != null && app.Context.User.Identity.IsAuthenticated)
            //{
            //    var req = app.Request;
            //    if (req.Cookies.Count > 0 && req.Cookies[ConfigValues.AUTHEN_COOKIE_KEY] != null)
            //    {
            //        var cookie = req.Cookies[ConfigValues.AUTHEN_COOKIE_KEY];
            //        if (cookie != null)
            //        {
            //            string str = cookie.Value;
            //            var userInfo = UserSecurity.DecryptUser(str);
            //            var principal = new UserPrinciple(userInfo);
            //            app.Context.User = principal;
            //        }
            //    }
            //}
            //Thread.CurrentThread.CurrentCulture = culture;
        }

        public void Dispose()
        {
        }

        //public void Logout()
        //{
        //    if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        FormsAuthentication.SignOut();
        //    }
        //}

        public LoginStatus AuthenticateUser(string username, string password, bool persistLogin, bool needEncryptPass)
        {
            //var culture = Thread.CurrentThread.CurrentCulture;
            //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var userInformation = UserBM.Instance.CheckLogin(username, needEncryptPass ? FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5") : password);
            if (userInformation != null)
            {
                if (userInformation.Status == (int)UserStatus.Suspended)
                    return LoginStatus.BanStatus;
                if (userInformation.Status == (int)UserStatus.Not_Activated)
                    return LoginStatus.NotActivated;

                //userInformation.IsAuthenticated = true;
                //HttpContext.Current.User = new UserPrinciple(userInformation);
                //HttpContext.Current.Session[Constant.ss_UserId] = userInformation.Id;

                //if (persistLogin)
                //{
                //    UserSecurity.StoreInformationOnCookies(username, password);
                //}
                ////UserSecurity.StoreInformationOnCookies(userInformation);
                //FormsAuthentication.SetAuthCookie(username, persistLogin);

                LoginLogBM.Instance.Insert(new Eli_LoginLog
                    {
                        CreatedDate = DateTime.Now,
                        IpAddress = Utilities.GetClientIpAddress(),
                        UserId = userInformation.Id
                    });

                return LoginStatus.Success;
            }
            //Thread.CurrentThread.CurrentCulture = culture;

            //int num_login_fails = HDUserBM.UpdateLoginFail(username);
            //if (num_login_fails == -1)
            //    return LoginStatus.NoAccount;
            //else if (num_login_fails == 3)
            //    return LoginStatus.BanStatus;
            //else
            return LoginStatus.LoginFail;
        }

        public static LoginStatus AuthenticateUser(string username, string password, out string accessToken)
        {
            accessToken = "";
            var userInformation = UserBM.Instance.CheckLogin(username, FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5"));
            if (userInformation != null)
            {
                if (userInformation.Status == (int)UserStatus.Suspended)
                    return LoginStatus.BanStatus;
                if (userInformation.Status == (int)UserStatus.Not_Activated)
                    return LoginStatus.NotActivated;

                var loginInfo = LoginLogBM.Instance.GetLoggedInUser(userInformation.Id);

                if (loginInfo == null)
                {
                    accessToken = SecurityHelper.Encrypt(string.Format("{0}|{1}|{2}", userInformation.Id,
                                                                       userInformation.LoginName, userInformation.RoleId));
                    LoginLogBM.Instance.Insert(new Eli_OnlineUsers
                    {
                        LoggedTime = DateTime.Now,
                        AccessToken = accessToken,
                        UserID = userInformation.Id
                    });
                }
                else
                    accessToken = loginInfo.AccessToken;
                return LoginStatus.Success;
            }
            return LoginStatus.LoginFail;
        }

        public static LoginStatus AuthenticateUser(string token)
        {
            var decryptString = SecurityHelper.Decrypt(token);
            var userId = int.Parse(decryptString.Split('|')[0]);

            var status = UserBM.Instance.GoOnline(token, userId);
            return status > 0 ? LoginStatus.Success : LoginStatus.LoginFail;
        }

        public static LoginStatus AuthenticateExistUser(Eli_User userInformation, out string accessToken)
        {
            accessToken = "";
            if (userInformation.Status == (int)UserStatus.Suspended)
                return LoginStatus.BanStatus;
            if (userInformation.Status == (int)UserStatus.Not_Activated)
                return LoginStatus.NotActivated;

            var loginInfo = LoginLogBM.Instance.GetLoggedInUser(userInformation.Id);

            if (loginInfo == null)
            {
                accessToken = SecurityHelper.Encrypt(string.Format("{0}|{1}|{2}", userInformation.Id,
                                                                   userInformation.LoginName, userInformation.RoleId));
                LoginLogBM.Instance.Insert(new Eli_OnlineUsers
                {
                    LoggedTime = DateTime.Now,
                    AccessToken = accessToken,
                    UserID = userInformation.Id
                });
            }
            else
                accessToken = loginInfo.AccessToken;
            return LoginStatus.Success;
        }
    }
}
