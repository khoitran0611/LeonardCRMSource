using System;
using System.Collections.Generic;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Web.Http;
using Eli.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class MailTemplateApiController :BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public virtual IList<Eli_MailTemplates> GetAllMailTemplate()
        {
            try
            {
                var entities = MailTemplateBM.Instance.GetAll();
                return entities;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
                return null;
            }
        }
        
        [HttpGet]
        public virtual Eli_MailTemplates GetMailTemplateById(int Id)
        {
            try
            {
                var mailtemplate = MailTemplateBM.Instance.GetById(Id);
                return mailtemplate;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
                return null;
            }
            
        }

        [HttpPost]
        public virtual ResultObj Save([FromBody] JObject jsonObject, [FromUri] int moduleId )
        {
            try
            {
                int status = 0;
                var mailTemplate = JsonConvert.DeserializeObject<Eli_MailTemplates>(jsonObject.ToString());
                string msg = Validate(mailTemplate, moduleId);
                if (string.IsNullOrEmpty(msg))
                {
                    status = MailTemplateBM.Instance.Update(mailTemplate);
                    if (status > 0)
                    {
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),0);
                    }
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"),0);
                }
                return new ResultObj(ResultCodes.ValidationError, msg,0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
                return  new ResultObj(ResultCodes.UnkownError,exception.Message,0);
            }
        }

        private string Validate(Eli_MailTemplates mailTemplate, int moduleId)
        {
            string msg = string.Empty;
            var validator = new ObjectValidator(moduleId);
            var result = validator.ValidateObject(mailTemplate);
            if (result.Length > 0)
            {
                msg += result;
            }
           
            return msg;
        }
    }
}
