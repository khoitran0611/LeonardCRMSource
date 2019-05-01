using Eli.Common;
using System;
using System.Web;
using System.Globalization;
using System.Threading;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.Security
{
    public class UserSecurity
    {
        public static void StoreInformationOnCookies(Eli_User user)
        {
            HttpResponse response = HttpContext.Current.Response;
            var encryptedUserDetails = EncryptUser(user);
            var userCookie = new HttpCookie(ConfigValues.AUTHEN_COOKIE_KEY, encryptedUserDetails);
            var curentInfo = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            userCookie.Expires = DateTime.Now.AddDays(ConfigValues.AUTHEN_COOKIE_EXPIRE_TIME);
            response.Cookies.Add(userCookie);
            if (curentInfo != null) Thread.CurrentThread.CurrentCulture = curentInfo;
        }

        public static void RemoveUserOnCookies()
        {
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;
            var vncookie = response.Cookies[ConfigValues.AUTHEN_COOKIE_KEY];
            if (vncookie == null) return;
            var curentInfo = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            vncookie.Expires = DateTime.Now.AddMinutes(-1);
            response.Cookies.Add(vncookie);
            if (curentInfo != null) Thread.CurrentThread.CurrentCulture = curentInfo;
        }

        public static string EncryptUser(Eli_User user)
        {
            //string plainText = XmlHelper.GetXmlString(user);
            //return SecurityHelper.Encrypt(plainText);
            return "";
        }

        public static Eli_User DecryptUser(string encryptedText)
        {
            //string plainText = SecurityHelper.Decrypt(encryptedText);
            //return XmlHelper.GetUserFromString(plainText);
            return new Eli_User();
        }

        public static void StoreInformationOnCookies(string username, string password)
        {
            var cookie = new HttpCookie(ConfigValues.AUTHEN_COOKIE_KEY);
            cookie["ud"] = SecurityHelper.Encrypt(username + "|" + password);
            cookie.Expires = DateTime.Now.AddDays(360);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
