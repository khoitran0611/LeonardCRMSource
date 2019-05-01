using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Eli.Common;
using LeonardCRM.BusinessLayer;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.Web.Controllers
{
    /// <summary>
    /// The parent of page
    /// Extend later
    /// </summary>
    [Authorize]
    public abstract class BasicController : Controller
    {
        //CultureInfo currentCulture;
        private readonly Registry _registry;
        protected BasicController()
        {
            _registry = LoadRegistry();
            ViewBag.Title = _registry.DEFAULT_TITLE;
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

        #region Properties
        /// <summary>
        /// Getting the server URL, eg. http://abc.com or http://localhost:8080
        /// </summary>
        protected string ServerURL
        {
            get
            {
                var serverPort = long.Parse(System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
                var isSecure = (System.Web.HttpContext.Current.Request.ServerVariables["HTTPS"].ToLower() == "on");

                var url = new StringBuilder("http");

                if (isSecure)
                {
                    url.Append("s");
                }

                url.AppendFormat("://{0}", System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);

                if ((!isSecure && serverPort != 80) || (isSecure && serverPort != 443))
                {
                    url.AppendFormat(":{0}", serverPort.ToString());
                }

                return url.ToString();
            }
        }

        protected int GetModuleId
        {
            get
            {
                var url = Request.QueryString["p"];
                if (!String.IsNullOrEmpty(url))
                {
                    try
                    {
                        return int.Parse(url);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                return 0;
            }
        }

        protected Guid ActivationID
        {
            get
            {
                try
                {
                    return Guid.Parse(System.Web.HttpContext.Current.Request.QueryString[Constant.ACTIVEID]);
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
                    return int.Parse(System.Web.HttpContext.Current.Request.QueryString[Constant.LogId]);
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
                    return null != System.Web.HttpContext.Current.User && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
                }
                catch
                {
                    return false;
                }
            }
        }

        protected Eli_User CurrentUser
        {
            get
            {
                try
                {
                    if (null == System.Web.HttpContext.Current.User || !System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
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
                    return new SessionContext().GetUserData();
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
                    return new SessionContext().GetUserData().Id;
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
                    return (PageMode)Enum.Parse(typeof(PageMode), System.Web.HttpContext.Current.Request.QueryString[Constant.PAGEMODE]);
                }
                catch
                {
                    return PageMode.Edit;
                }
            }
        }

        public Registry SiteSettings
        {
            get {
                return _registry;
            }
        }

        private Registry LoadRegistry()
        {
            if (!ConfigValues.ENABLE_CACHE)
            {
                return new Registry();
            }
            var cache = new CacheManager();
            if (!cache.Exist(Constant.SiteSettings))
            {
                cache.Remove(Constant.SiteSettings);
                cache.Add(new Registry(), Constant.SiteSettings);
            }
            return cache.Get<Registry>(Constant.SiteSettings);
        }

        #endregion

        //protected override void OnPreInit(EventArgs e)
        //{
        //    if (String.IsNullOrEmpty(MasterPageFile))
        //        MasterPageFile = "~" + ConfigValues.SITE_ROOT + ConfigValues.SITE_MASTER_PAGE;
        //    base.OnPreInit(e);
        //}

        //protected override void InitializeCulture()
        //{
        //if (!IsPostBack)
        //{
        //    if (String.IsNullOrEmpty(SessionManager.PreferLanguage))
        //        SessionManager.PreferLanguage = new BaseUserControl("").CurrentCulture;

        //    var currentCulture = SessionManager.PreferLanguage;
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(currentCulture);
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);
        //}
        //}

        //protected void RequireRole(params Role[] roles)
        //{
        //    try
        //    {
        //        if (null != HttpContext.Current.User && HttpContext.Current.User.Identity.IsAuthenticated)
        //        {
        //            var user = BusinessLayer.HDUserBM.Instance.GetById(int.Parse(HttpContext.Current.User.Identity.Name));
        //            var prin = new UserPrinciple(user);
        //            if (!prin.IsInRole(roles))
        //                UriHelper.Redirect(ConfigValues.SITE_ROOT + Pages.DeniedPage);
        //        }
        //        else
        //        {
        //            FormsAuthentication.RedirectToLoginPage();
        //            Response.End();
        //        }
        //    }
        //    catch (ThreadAbortException)
        //    {
        //    }
        //    catch (Exception)
        //    {
        //        UriHelper.Redirect("", Pages.Login);
        //    }
        //}

        //private string RequestedPage
        //{
        //    get
        //    {
        //        var index = Page.Request.Path.IndexOf(".aspx");
        //        if (index > 0)
        //        {
        //            var str = Page.Request.Path.Substring(index);
        //            if (str.Length > 5)
        //            {
        //                Response.Redirect("~/");
        //                return "";
        //            }
        //        }
        //        return Page.Request.RawUrl;
        //    }
        //}

        //protected void RequireRole()
        //{
        //    try
        //    {
        //        if (null != HttpContext.Current.User && HttpContext.Current.User.Identity.IsAuthenticated
        //            && HttpContext.Current.Session[Constant.ss_UserId] != null 
        //            && !String.IsNullOrEmpty(HttpContext.Current.Session[Constant.ss_UserId].ToString()))
        //        {
        //            var cache = new CacheManager();
        //            var key = Constant.CurrentUserKey + "_" + HttpContext.Current.Session[Constant.ss_UserId];
        //            HDUser cachedUser = null;
        //            if (cache.Exist(key))
        //                cachedUser = cache.Get<HDUser>(key);
        //            if (cachedUser == null)
        //                cachedUser = HDUserBM.Instance.GetByIdWithRoles((int)HttpContext.Current.Session[Constant.ss_UserId]);
        //            if (cachedUser != null)
        //            {
        //                var group = cachedUser.Groups.SingleOrDefault();
        //                var navs = NavigationBM.Instance.GetNavigationsByCondition(n => n.GroupId == group.Flags);
        //                var requestPage = RequestedPage;
        //                if (requestPage.IndexOf("&") >= 0)
        //                {
        //                    requestPage = requestPage.Remove(requestPage.IndexOf("&"));
        //                }
        //                if (requestPage.IndexOf("?") >= 0)
        //                {
        //                    requestPage = requestPage.Remove(requestPage.IndexOf("?"));
        //                }
        //                var vwNavigations = navs as IList<vwNavigation> ?? navs.ToList();
        //                if (vwNavigations.All(n => n.Link.ToLower() != requestPage.ToLower()) && !vwNavigations.Any(n => RequestedPage.ToLower().Contains(n.Link.ToLower())))
        //                    UriHelper.Redirect("", Pages.DeniedPage);

        //            }
        //            else
        //            {
        //                //UriHelper.Redirect("", Pages.Login, "ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
        //                UriHelper.Redirect("", Pages.Login);
        //                Response.End();
        //            }
        //        }
        //        else
        //        {
        //            //UriHelper.Redirect("", Pages.Login, "ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
        //            UriHelper.Redirect("", Pages.Login);
        //            Response.End();
        //        }
        //    }
        //    catch (ThreadAbortException)
        //    {
        //    }
        //    catch (Exception)
        //    {
        //        UriHelper.Redirect("", Pages.Login);
        //    }
        //}

        //protected void RequirePermission(Permission permission)
        //{
        //    try
        //    {
        //        var HDUser = (HDUser)HttpContext.Current.User.Identity;
        //        //HDUser.Permissions = HDUserBM.GetPermissions(HDUser);
        //        var prin = new UserPrinciple(HDUser);
        //        if (!prin.HasPermission(permission))
        //        {
        //            Response.Redirect("~/Deny.aspx");
        //        }
        //    }
        //    catch { }
        //}

        //public void AddMetaTag(string text, int index, string tagName)
        //{
        //    var metaDescription = new HtmlMeta();
        //    metaDescription.Name = tagName;
        //    metaDescription.Content = text;
        //    this.Page.Header.Controls.AddAt(index, metaDescription);
        //}

        //protected void MakeTitle(string title)
        //{
        //    Title = title;
        //}

        //protected override void OnLoad(EventArgs e)
        //{
        //    if (String.IsNullOrEmpty(MasterPageFile) || (!String.IsNullOrEmpty(MasterPageFile) && MasterPageFile.ToLower() != "admin.master"))
        //    {
        //        // Create the <link> element for the CSS file
        //        var stylesheet = new HtmlLink {Href = ThemeFile("theme.css")};
        //        stylesheet.Attributes.Add("rel", "stylesheet");
        //        stylesheet.Attributes.Add("type", "text/css");

        //        // Add it to the <head> element of the page
        //        if (Header != null)
        //            Header.Controls.Add(stylesheet);
        //    }
        //    base.OnLoad(e);
        //}

        //        #region Theme Functions
        //        // XML THEME FILE (TEST)
        //        private XmlDocument LoadTheme(string themefile)
        //        {
        //            if (themefile == null || String.IsNullOrEmpty(themefile))
        //            {
        //                var ctrl = new BaseUserControl("");
        //                themefile = ctrl.SiteSettings.DEFAULT_THEME;
        //                ctrl = null;
        //                if (themefile == null || String.IsNullOrEmpty(themefile))
        //                {
        //                    themefile = "standard.xml";
        //                }
        //            }

        //            XmlDocument doc = null;
        //#if !DEBUG
        //            doc = (XmlDocument)System.Web.HttpContext.Current.Cache[themefile];
        //#endif
        //            if (doc == null)
        //            {
        //                doc = new XmlDocument();
        //                string filePath = String.Format("{0}themes/{1}", ConfigValues.SITE_ROOT, themefile);
        //                doc.Load(System.Web.HttpContext.Current.Server.MapPath(filePath));
        //#if !DEBUG
        //                System.Web.HttpContext.Current.Cache[themefile] = doc;
        //#endif
        //            }
        //            return doc;
        //        }

        //        private string ThemeDir
        //        {
        //            get
        //            {
        //                XmlDocument doc = LoadTheme(null);
        //                return doc.DocumentElement != null ? String.Format("{0}themes/{1}/", ConfigValues.SITE_ROOT, doc.DocumentElement.Attributes["dir"].Value) : "";
        //            }
        //        }

        //        private string ThemeFile(string filename)
        //        {
        //            return ThemeDir + filename;
        //        }

        //        #endregion
    }
}
