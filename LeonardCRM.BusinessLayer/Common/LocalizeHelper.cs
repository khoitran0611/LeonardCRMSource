using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Eli.Common;

namespace LeonardCRM.BusinessLayer.Common
{
    public class LocalizeHelper
    {
        private static volatile LocalizeHelper _instance;
        private static readonly object SyncRoot = new Object();
        public static LocalizeHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new LocalizeHelper();
                    }
                }

                return _instance;
            }
        }

        private Localizer _mLocalizer;
        private Localizer _mDefaultLocale;
        private readonly CacheManager _cache;

        private readonly string _defaultLanguage;

        private static XDocument _doc;
        private static XDocument _defaultDoc;

        public static void ClearCacheForEdit()
        {
            _doc = null;
            _defaultDoc = null;
        }

        private LocalizeHelper()
        {
            _defaultLanguage = Settings.DefaultLanguage;
            _cache = new CacheManager();
            InitLocalizeHelper();
        }

        private void InitLocalizeHelper()
        {
            LoadLocalizer(DefaultLanguage);

            // If not using default language load that too
            if (DefaultLanguage.ToLower() != "default.xml")
            {
                LoadDefaultLanguage();
            }
        }

        public string DefaultLanguage
        {
            get { return !string.IsNullOrWhiteSpace(_defaultLanguage) ? _defaultLanguage: "default.xml"; }
        }

        private void LoadLocalizer(string filename)
        {
            try
            {
                if (_mLocalizer == null && _cache.Exist("Localizer." + filename))
                {
                    _mLocalizer = _cache.Get<Localizer>("Localizer." + filename);
                }

                if (_mLocalizer == null)
                {
                    _mLocalizer =
                        new Localizer(
                            HttpContext.Current.Server.MapPath(String.Format("{0}languages/{1}", ConfigValues.SITE_ROOT,
                                filename)));

                    //cache exists but can't get data, then remove it and re-insert
                    _cache.Remove("Localizer." + filename);
                    _cache.Add(_mLocalizer, "Localizer." + filename); 
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("Load Language error: " + ex.Message, ex);
            }
        }

        private void LoadDefaultLanguage()
        {
            try
            {
                if (_mDefaultLocale == null && _cache.Exist("DefaultLocale"))
                {
                    _mDefaultLocale = _cache.Get<Localizer>("DefaultLocale");
                }

                if (_mDefaultLocale == null)
                {
                    _mDefaultLocale =
                        new Localizer(HttpContext.Current.Server.MapPath(String.Format("{0}languages/default.xml",
                            ConfigValues.SITE_ROOT)));

                    _cache.Remove("DefaultLocale");
                    _cache.Add(_mDefaultLocale, "DefaultLocale");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("Load Default Language error:" + ex.Message, ex);
            }
        }

        public string GetText(string page, string textKey)
        {
            try
            {
                var str = _mLocalizer.GetText(page, textKey);

                // If the resource doesn't exist, try to use the default resource
                if (string.IsNullOrEmpty(str) && _mDefaultLocale != null)
                {
                    str = _mDefaultLocale.GetText(page, textKey);
                    if (!string.IsNullOrEmpty(str)) str = '[' + str + ']';
                }

                //Resource is missing
                if (string.IsNullOrEmpty(str))
                {
                    LogHelper.Log(String.Format("Missing Translation For {1}.{0}", textKey.ToUpper(), page.ToUpper()));
                    return String.Format("[{1}.{0}]", textKey.ToUpper(), page.ToUpper());
                }

                //Support html tag
                str = str.Replace("[", "<").Replace("]", ">");
                return str;
            }
            catch (Exception ex)
            {
                LogHelper.Log("GetText error:" + ex.Message, ex);
                return string.Empty;
            }
        }

        public Hashtable GetAllCachedResources()
        {
            if (!_cache.Exist(Constant.ResourceStrings))
            {
                _cache.Add(GetResourceStrings(DefaultLanguage), Constant.ResourceStrings);
            }
            return _cache.Get<Hashtable>(Constant.ResourceStrings);
        }

        public Hashtable GetResourceStrings(string defaultLanguage)
        {
            var resources = new Hashtable();
            var resourcePages = GetResourcePages(defaultLanguage);
            foreach (var resourcePage in resourcePages)
            {
                var resourceStrings = GetResourceStringsByPage(resourcePage.Name, defaultLanguage);
                var resourceElements = new Hashtable();
                foreach (var resourceString in resourceStrings)
                {
                    resourceElements.Add(resourceString.Name, resourceString.Value);
                }
                resources.Add(resourcePage.Name, resourceElements);
            }
            return resources;
        }

        public IEnumerable<Resource> GetResourceStringsByPage(string page, string languageFile)
        {
            if (_doc == null)
            {
                _doc = XDocument.Load(HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage,
                        languageFile)));
            }

            if (_defaultDoc == null)
            {
                _defaultDoc =
                    XDocument.Load(HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage,
                        "default.xml")));
            }

            var elements =
                _doc.Elements("Resources")
                    .Elements("page")
                    .Where(x => x.Attribute("name").Value == page)
                    .Elements("Resource");

            var defElements =
                _defaultDoc.Elements("Resources")
                    .Elements("page")
                    .Where(x => x.Attribute("name").Value == page)
                    .Elements("Resource");

            var list = from el in elements
                join defel in defElements on
                    el.Attribute("tag").Value equals (string) defel.Attribute("tag")
                select new Resource
                {
                    Name = el.Attribute("tag").Value,
                    Default = defel.Value,
                    Value = el.Value
                };

            return list.OrderBy(r => r.Name).ToList();
        }

        public IEnumerable<Resource> GetResourcePages(string languageFile)
        {
            if (_doc == null)
            {
                _doc =XDocument.Load(HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, languageFile)));
            }

            var elements = _doc.Elements("Resources").Elements("page");
            var list = from el in elements
                select new Resource
                {
                    Name = el.Attribute("name").Value,
                };
            return list.OrderBy(r => r.Name).ToList();
        }

        /// <summary>
        /// Refresh resource cache so that resource is up-to-date
        /// </summary>
        public void RefreshResourceCache()
        {
            _cache.Remove("Localizer." + DefaultLanguage);
            _cache.Remove("DefaultLocale");
            _cache.Remove(Constant.ResourceStrings);
            _doc = XDocument.Load(HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, DefaultLanguage)));
            _defaultDoc = XDocument.Load(HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, "default.xml")));
            if(_mLocalizer != null)
                _mLocalizer.LoadFile(HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, DefaultLanguage)));
        }
    }
}
