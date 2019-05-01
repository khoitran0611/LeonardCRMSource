using System;
using System.Collections;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ConfigValues = LeonardCRM.BusinessLayer.Common.Registry;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class RegistryApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("HOST_SETTING", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }
        [HttpGet]
        public ConfigValues GetRegistry()
        {
            return SiteSettings;
        }

        [HttpGet]
        public Hashtable GetSettings()
        {
            try
            {
                var hashtable = new Hashtable
                {
                    {"reg", new ConfigValues()},
                    {"languages", ControlHelper.Languages()},
                    {"themes", ControlHelper.Themes()},
                    {"currencies", CurrencyBM.Instance.Find(n=>n.IsActive)}
                };
                return hashtable;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Hashtable();
            }
        }
        [HttpPost]
        public virtual ResultObj SaveSettings([FromBody] JObject jsonObject)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<ConfigValues>(jsonObject.ToString());
                int status = config.SaveRegistry();

                if (status > 0)
                {
                    SiteSettings = null;
                    if(Settings.DefaultLanguage == string.Empty || Settings.DefaultLanguage != config.DEFAULT_LANGUAGE)
                        Settings.DefaultLanguage = config.DEFAULT_LANGUAGE;

                    return new ResultObj(ResultCodes.Success,
                        GetText("SAVE_SUCCESS"), 0);
                }

                return new ResultObj(ResultCodes.SavingFailed,
                    GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }
    }
}
