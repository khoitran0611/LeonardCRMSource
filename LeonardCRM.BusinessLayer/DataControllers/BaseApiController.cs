using System;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Globalization;
using System.Threading;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Security;
using System.Xml;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    /// <summary>
    /// This class use for all WebApiControllers
    /// Cache user control 
    /// 
    /// </summary>
    [Authorize]
    public class BaseApiController : ApiController
    {
        private readonly Registry _registry;

        protected BaseApiController()
        {
            _registry = LoadRegistry();
        }
        public bool HasPermisson(Permission permission, int moduleId)
        {
            if (!IsLoggedIn || CurrentUserRole == null)
                return false;

            var rolePermissions =
                RolesPermissionsBM.Instance.Single(r => r.RoleId == CurrentUserRole.Id && r.ModuleId == moduleId);
            if (rolePermissions == null)
                return false;

            if (permission == Permission.Read)
                return rolePermissions.AllowRead;
            if (permission == Permission.CreateEdit)
                return rolePermissions.AllowEdit;
            if (permission == Permission.Delete)
                return rolePermissions.AllowDelete;

            return false;
        }

        #region Input Checking functions such as username, password, email...

        /// <summary>
        /// Checking if the filename is an image file(gif - jpg - png - jpeg - bmp - tif) or not
        /// </summary>
        /// <param name="fileName">The filename with extension to check</param>
        /// <returns></returns>
        public bool CheckImageExtension(string fileName)
        {
            //get extension
            string extension = Path.GetExtension(fileName).ToLower() + "|";
            //we just support some extensions  gif - jpg - png - jpeg - bmp - tif
            string exts = ".gif|.jpg|.png|.jpeg|.bmp|.tif|";
            if (!exts.Contains(extension))
                return false;
            return true;
        }

        /// <summary>
        /// Check if username is valid. A valid username is min 5chars, max 50chars of alphanumeric(case insensitive) and 2 symbols '-_'
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        protected bool IsUsernameValid(string username)
        {
            var reg = new Regex(@"^(?=.*[a-zA-Z])[a-zA-Z0-9-_]{5,50}$");
            return reg.IsMatch(username);
        }

        #endregion

        /// <summary>
        /// Checking if an input string exists in a denied list of strings or not
        /// </summary>
        /// <param name="inputString">The input string to check</param>
        /// <param name="denyStrings">The denied list of strings</param>
        /// <returns></returns>
        protected bool IsStringExistInDenyList(string inputString, string denyStrings)
        {
            inputString = inputString.Trim().RemoveDirtySymbols("");
            var denyArr = denyStrings.Split(',');
            return denyArr.Any(s => s.Trim().ToLower() == inputString.ToLower());
        }

        protected string GetFormartedDate(object date, bool withTime)
        {
            try
            {
                var culture = CultureInfo.CreateSpecificCulture(SessionManager.PreferLanguage);
                Thread.CurrentThread.CurrentCulture = culture;
                var s = DateTime.Parse(date.ToString(), culture);
                return s.ToString(SiteSettings.DATE_FORMAT.Replace("'", "\"'\"") + (withTime ? String.Format(" {0}", SiteSettings.TIME_FORMAT) : ""));
            }
            catch
            {
                return String.Empty;
            }
        }

        #region Properties
        /// <summary>
        /// Getting the server URL, eg. http://abc.com or http://localhost:8080
        /// </summary>
        protected string ServerURL
        {
            get
            {
                var serverPort = long.Parse(HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
                var isSecure = (HttpContext.Current.Request.ServerVariables["HTTPS"].ToLower() == "on");

                var url = new StringBuilder("http");

                if (isSecure)
                {
                    url.Append("s");
                }

                url.AppendFormat("://{0}", HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);

                if ((!isSecure && serverPort != 80) || (isSecure && serverPort != 443))
                {
                    url.AppendFormat(":{0}", serverPort.ToString());
                }

                return url.ToString();
            }
        }

        protected Guid ActivationID
        {
            get
            {
                try
                {
                    return Guid.Parse(HttpContext.Current.Request.QueryString[Constant.ACTIVEID]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected int LogId
        {
            get
            {
                try
                {
                    return int.Parse(HttpContext.Current.Request.QueryString[Constant.LogId]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public string CurrentCulture
        {
            get
            {
                var cache = new CacheManager();
                if (!cache.Exist(Constant.CurrentLanguage))
                {
                    var languages = ControlHelper.Languages();
                    var current = languages.SingleOrDefault(l => l.FileName == SiteSettings.DEFAULT_LANGUAGE);
                    if (current != null)
                    {
                        cache.Add(current.Code, Constant.CurrentLanguage);
                        return current.Code;
                    }
                }
                return cache.Get<string>(Constant.CurrentLanguage);

            }
            set { new CacheManager().Remove(Constant.CurrentLanguage); }
        }

        protected bool IsLoggedIn
        {
            get
            {
                try
                {
                    return null != HttpContext.Current.User && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
                }
                catch
                {
                    return false;
                }
            }
        }

        protected Eli_User GetUserData()
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
            }

            return userData;
        }

        protected Eli_User CurrentUser
        {
            get
            {
                try
                {
                    if (null == HttpContext.Current.User || !HttpContext.Current.User.Identity.IsAuthenticated)
                        return null;

                    //if (System.Web.HttpContext.Current.Session[Constant.ss_UserId] != null && !String.IsNullOrEmpty(System.Web.HttpContext.Current.Session[Constant.ss_UserId].ToString()))
                    //{
                    //    var cache = new CacheManager();
                    //    var key = Constant.CurrentUserKey + "_" + CurrentUserID;
                    //    if (cache.Exist(key))
                    //    {
                    //        var cachedUser = cache.Get<Eli_User>(key);
                    //        if (cachedUser != null)
                    //            return cachedUser;
                    //        cache.Remove(key);
                    //    }
                    //    var user =
                    //        HDUserBM.Instance.GetByIdWithRoles(CurrentUserID);
                    //    if (user != null)
                    //    {
                    //        cache.Add(user, key);
                    //        return user;
                    //    }
                    //    return null;
                    //}
                    //return null;
                    return GetUserData();
                }
                catch
                {
                    return null;
                }
            }
        }

        public Eli_Roles CurrentUserRole
        {
            get { return CurrentUser != null ? CurrentUser.Eli_Roles : null; }
        }

        public int CurrentUserID
        {
            get
            {
                try
                {
                    return GetUserData().Id;
                }
                catch
                {
                    return 0;
                }
            }
        }

        protected PageMode CurrentPageMode
        {
            get
            {
                try
                {
                    return (PageMode)Enum.Parse(typeof(PageMode), HttpContext.Current.Request.QueryString[Constant.PAGEMODE]);
                }
                catch
                {
                    return PageMode.Edit;
                }
            }
        }

        public Registry SiteSettings
        {
            get { return _registry; }
            set { new CacheManager().Remove(Constant.SiteSettings); }
        }

        private Registry LoadRegistry()
        {
            var cache = new CacheManager();
            if (!cache.Exist(Constant.SiteSettings))
            {
                cache.Remove(Constant.SiteSettings);
                cache.Add(new Registry(), Constant.SiteSettings);
            }
            return cache.Get<Registry>(Constant.SiteSettings);
        }

        #endregion

        #region Theme Functions

        // XML THEME FILE (TEST)
        private XmlDocument LoadTheme(string themefile)
        {
            if (themefile == null || String.IsNullOrEmpty(themefile))
            {
                themefile = SiteSettings.DEFAULT_THEME;

                if (themefile == null || String.IsNullOrEmpty(themefile))
                {
                    themefile = "standard.xml";
                }
            }

            XmlDocument doc = null;
            if (ConfigValues.ENABLE_CACHE)
                doc = (XmlDocument)HttpContext.Current.Cache[themefile];

            if (doc == null)
            {
                doc = new XmlDocument();
                var filePath = String.Format("{0}themes/{1}", ConfigValues.SITE_ROOT, themefile);
                doc.Load(HttpContext.Current.Server.MapPath(filePath));
                if (ConfigValues.ENABLE_CACHE)
                    HttpContext.Current.Cache[themefile] = doc;
            }
            return doc;
        }

        /// <summary>
        /// Get a value from the currently configured forum theme.
        /// </summary>
        /// <param name="page">Page to look under</param>
        /// <param name="tag">Theme item</param>
        /// <returns>Converted Theme information</returns>
        protected string GetThemeContents(string page, string tag)
        {
            return GetThemeContents(page, tag, String.Format("[{0}.{1}]", page.ToUpper(), tag.ToUpper()), false);
        }

        /// <summary>
        /// Get a value from the currently configured theme.
        /// </summary>
        /// <param name="page">Page to look under</param>
        /// <param name="tag">Theme item</param>
        /// <param name="defaultValue">Value to return if the theme item doesn't exist</param>
        /// <param name="dontLogMissing">True if you don't want a log created if it doesn't exist</param>
        /// <returns>Converted Theme information or Default Value if it doesn't exist</returns>
        private string GetThemeContents(string page, string tag, string defaultValue, bool dontLogMissing)
        {
            //var doc = LoadTheme(null);

            //if (doc.DocumentElement != null)
            //{
            //    string themeDir = doc.DocumentElement.Attributes["dir"].Value;
            //    string langCode = LoadTranslation().ToUpper();
            //    string select = string.Format("//page[@name='{0}']/Resource[@tag='{1}' and @language='{2}']",
            //                                  page.ToUpper(), tag.ToUpper(), langCode);
            //    var node = doc.SelectSingleNode(@select);
            //    if (node == null)
            //    {
            //        @select = string.Format("//page[@name='{0}']/Resource[@tag='{1}']", page.ToUpper(), tag.ToUpper());
            //        node = doc.SelectSingleNode(@select);
            //    }
            //    if (node == null)
            //    {
            //        if (!dontLogMissing)
            //            LogHelper.Log(String.Format("Missing Theme Item: {0}.{1}", page.ToUpper(), tag.ToUpper()));
            //        return defaultValue;
            //    }

            //    string contents = node.InnerText;
            //    contents = contents.Replace("~", String.Format("{0}themes/{1}", ConfigValues.SITE_ROOT, themeDir));
            //    return contents;
            //}
            return "";
        }

        protected string ThemeDir
        {
            get
            {
                var doc = LoadTheme(null);
                if (doc.DocumentElement != null)
                    return String.Format("{0}themes/{1}/", ConfigValues.SITE_ROOT,
                                         doc.DocumentElement.Attributes["dir"].Value);
                return "";
            }
        }

        public string GetUrlToResource(string resourceName)
        {
            return string.Format("{1}resource.ashx?r={0}", resourceName, ConfigValues.SITE_ROOT);
        }

        private string ThemeFile(string filename)
        {
            return ThemeDir + filename;
        }

        #endregion

        #region Audit commands

        /// <summary>
        /// Set audit fields such as CreatedDate, CreatedBy...
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcKeyValue"></param>
        /// <returns></returns>
        protected void SetAuditFields(object src, int srcKeyValue)
        {
            SetDefaultAudit(src, srcKeyValue);
        }
        protected void SetAuditFields(object src, long srcKeyValue)
        {
            SetDefaultAudit(src, srcKeyValue);
        }

        private void SetDefaultAudit(object src, int srcKeyValue)
        {
            if (srcKeyValue <= 0)
            {
                TrySetProperty(src, "CreatedBy", CurrentUserID);
                TrySetProperty(src, "CreatedDate", DateTime.Now);
            }
            TrySetProperty(src, "ModifiedBy", CurrentUserID);
            TrySetProperty(src, "ModifiedDate", DateTime.Now);
        }
        private void SetDefaultAudit(object src, long srcKeyValue)
        {
            if (srcKeyValue <= 0)
            {
                TrySetProperty(src, "CreatedBy", CurrentUserID);
                TrySetProperty(src, "CreatedDate", DateTime.Now);
            }
            TrySetProperty(src, "ModifiedBy", CurrentUserID);
            TrySetProperty(src, "ModifiedDate", DateTime.Now);
        }

        private void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
        }

        #endregion

        public sealed class CustomStatus
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}