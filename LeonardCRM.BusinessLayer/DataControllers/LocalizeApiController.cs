using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class LocalizeApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("EDIT_LANGUAGE", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [AllowAnonymous]
        [HttpGet]
        public Hashtable GetResources()
        {
            return LocalizeHelper.Instance.GetAllCachedResources();
        }


        [HttpGet]
        public Hashtable GetDefaultLanguages()
        {
            try
            {
                var fileData = ControlHelper.Languages().OrderBy(r => r.Theme).ToList();
                var currentFileLanguage = SiteSettings.DEFAULT_LANGUAGE;
                var res = GetPageResource(currentFileLanguage, string.Empty);
               
                var hashtable = new Hashtable
                {
                    {"languageFiles", fileData},
                    {"defaultLang", res["defaultLang"]},
                    {"pages", res["pages"]},
                    {"resString", res["resString"]}
                };
                return hashtable;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Hashtable();
            }
        }

        [HttpGet]
        public Hashtable LanguageChanged([FromUri] string fileName)
        {
            try
            {
                return GetPageResource(fileName, string.Empty);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Hashtable();
            }
        }

        [HttpGet]
        public Hashtable PageChanged([FromUri] string fileName, [FromUri] string pageName)
        {
            try
            {
                return GetPageResource(fileName, pageName);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Hashtable(); ;
            }
        }

        [HttpGet]
        public IList<ControlHelper.ThemeData> GetListLanguages()
        {
            try
            {
                return ControlHelper.Languages().OrderBy(r => r.Theme).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj CreateLanguage([FromBody] LanguageExt language)
        {
            try
            {
                LocalizeHelper.ClearCacheForEdit();

                var fileName = language.LanguageName.ToLower() + ".xml";

                var newLanguagePath = HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, fileName));
                if (File.Exists(newLanguagePath))
                {
                    return new ResultObj(ResultCodes.ValidationError,
                        GetText("LANGUAGE_EXIST"), 0);
                }

                var defaultLanguagePath = HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage
                    , SiteSettings.DefaultLanguageFileName));
                File.Copy(defaultLanguagePath, newLanguagePath);

                var doc = XDocument.Load(newLanguagePath);

                doc.Elements("Resources")
                    .Single(x => x.Attribute("language").Value == "English")
                    .SetAttributeValue("language", language.LanguageName);

                doc.Elements("Resources")
                   .Single(x => x.Attribute("code").Value == "en")
                   .SetAttributeValue("code", language.LanguageCode);

                doc.Elements("Resources")
                   .Single(x => x.Attribute("culture").Value == "en-US")
                   .SetAttributeValue("culture", language.CultureCode);

                doc.Save(newLanguagePath);

                //new CacheManager().Remove(Constant.ResourceStrings);

                return new ResultObj(ResultCodes.Success,
                    GetText("SAVE_SUCCESS")) { Names = fileName };
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public ResultObj CreateResource([FromBody] ResourceExt resource)
        {
            try
            {
                LocalizeHelper.ClearCacheForEdit();

                var listLanguages = ControlHelper.Languages().OrderBy(r => r.Theme).ToList();
                listLanguages.Add(new ControlHelper.ThemeData {FileName = SiteSettings.DefaultLanguageFileName});

                foreach (var language in listLanguages)
                {
                    var msg = CreateResource(language.FileName, resource.Name);

                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        return new ResultObj(ResultCodes.ValidationError, msg, 0);
                    }
                }

                //new CacheManager().Remove(Constant.ResourceStrings);
                return new ResultObj(ResultCodes.Success,
                    GetText("SAVE_SUCCESS"), 0) {Names = resource.Name};
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        private string CreateResource(string fileName, string resourceName)
        {
            var filePath =
                HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, fileName));
            var doc = XDocument.Load(filePath);

            var resource = new XElement("page", new XAttribute("name", resourceName));
            resource.SetValue("");

            var pages = doc.Elements("Resources").Elements("page").ToList();

            if (pages.All(p => (string)p.Attribute("name") != resourceName))
            {
                pages.Last().AddAfterSelf(resource);
            }
            else
            {
                return GetText("RESOURCE_EXIST");
            }

            doc.Save(filePath);

            return string.Empty;
        }

        [HttpPost]
        public ResultObj CreateTranslation([FromBody] TranslationExt translation)
        {
            try
            {  
                LocalizeHelper.ClearCacheForEdit();

                var listLanguages = ControlHelper.Languages().OrderBy(r => r.Theme).ToList();
                listLanguages.Add(new ControlHelper.ThemeData {FileName = SiteSettings.DefaultLanguageFileName});

                foreach (var language in listLanguages)
                {
                    var msg = CreateTranslation(language.FileName, translation);

                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        return new ResultObj(ResultCodes.ValidationError, msg, 0);
                    }
                }

                //new CacheManager().Remove(Constant.ResourceStrings);
                return new ResultObj(ResultCodes.Success,
                    GetText("SAVE_SUCCESS"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        private string CreateTranslation(string fileName, TranslationExt translation)
        {
            var translationValue = fileName.ToUpper() == translation.FileName.ToUpper()
                ? translation.Translation
                : translation.TranslationDefault;

            var filePath = HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, fileName));
            var doc = XDocument.Load(filePath);

            var resources = doc.Elements("Resources").Elements("page")
                .First(c => (string) c.Attribute("name") == translation.ResourceName);
            var listResources = resources.Elements("Resource").ToList();

            var newResource = new XElement("Resource", new XAttribute("tag", translation.Key));
            newResource.SetValue(translationValue);

            if (listResources.Any())
            {
                if (listResources.All(p => (string) p.Attribute("tag") != translation.Key))
                {
                    listResources.Last().AddAfterSelf(newResource);
                }
                else
                {
                    return GetText("TRANSLATION_EXIST");
                }
            }
            else
            {
                resources.Add(newResource);
            }

            doc.Save(filePath);

            return String.Empty;
        }

        [HttpPost]
        public ResultObj SaveLanguage([FromUri] string fileName, [FromUri] string pageName, [FromBody] JObject jsonObject)
        {
            try
            {
                var isChanged = false;
                var hashtable = JsonConvert.DeserializeObject<Hashtable>(jsonObject.ToString());
                var keys = hashtable.Keys.Cast<string>().ToArray();
                var path = HttpContext.Current.Server.MapPath(String.Format("~/{0}/{1}", Pages.FolderLanguage, fileName));
                var doc = XDocument.Load(path);
                foreach (string key in keys)
                {
                    string value = hashtable[key].ToString();
                    var ele = doc.Elements("Resources")
                        .Elements("page")
                        .Where(x => x.Attribute("name").Value == pageName)
                        .Elements("Resource")
                        .SingleOrDefault(x => x.Attribute("tag").Value == key);

                    if (ele == null || ele.Value.Equals(value))
                    {
                        continue;
                    }

                    ele.Value = value;
                    isChanged = true;
                }

                if (isChanged)
                {
                    doc.Save(path);
                    LocalizeHelper.Instance.RefreshResourceCache();
                    return new ResultObj(ResultCodes.Success,
                            GetText("SAVE_SUCCESS"));
                }

                return new ResultObj(ResultCodes.Success,
                            GetText("SAVE_SUCCESS_NOTHING"));
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"));
            }
        }

        [HttpGet]
        public ResultObj RefreshCache()
        {
            try
            {
                LocalizeHelper.Instance.RefreshResourceCache();
                return new ResultObj(ResultCodes.Success, "The resource has been refreshed");
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"));
            }
        }
        private Hashtable GetPageResource(string fileName, string pageName)
        {
            var defaultLanguageName = getdefaultlanguage();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = defaultLanguageName;
            }

            var res = LocalizeHelper.Instance.GetResourceStrings(fileName);
            var pages = res.Keys.Cast<string>().ToArray();
            Array.Sort(pages, StringComparer.InvariantCulture);

            return new Hashtable
            {
                {"pages", pages},
                {"resString", string.IsNullOrWhiteSpace(pageName) ? res[pages[0]] : res[pageName]},
                {"defaultLang", LocalizeHelper.Instance.GetResourceStrings(defaultLanguageName)}
            };
        }

        private string getdefaultlanguage()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "languages\\";
            string[] filesPath = Directory.GetFiles(path, "default.xml", SearchOption.TopDirectoryOnly);
            return Path.GetFileName(filesPath[0]);
        }
    }
}
