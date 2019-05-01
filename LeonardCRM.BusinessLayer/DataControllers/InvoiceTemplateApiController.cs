using System;
using System.Collections.Generic;
using System.Web.Http;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eli.Common;


namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class InvoiceTemplateApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public virtual SalesInvTemplate GetInvTemplateById([FromUri] int invTemplateId)
        {
            var entity = new SalesInvTemplate();
            try
            {
                entity = SalesInvTemplateBM.Instance.GetById(invTemplateId);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
            }
            return entity;
        }

        [HttpPost]
        public virtual ResultObj DeleteInvTemplate([FromBody] JArray jsonArray)
        {
            try
            {
                int status = 0;
                var entities = JsonConvert.DeserializeObject<List<SalesInvTemplate>>(jsonArray.ToString());

                foreach (var entity in entities)
                {
                    var invoice = SalesInvoiceBM.Instance.First(inv => inv.InvTemplateId == entity.Id);
                    if (invoice != null)
                    {
                        return new ResultObj(ResultCodes.ValidationError, GetText("INVTEMPLATE", "FKEY_CONFLICT"),0);
                    }
                }
                status = SalesInvTemplateBM.Instance.Delete(entities);
                if (status > 0)
                {
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0);
                }
               
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"),0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
                return new ResultObj(ResultCodes.UnkownError, exception.ToString(),0);
            }
        }
     
        [HttpPost]
        public virtual ResultObj SaveInvTemplate([FromBody] JObject jsonObject,[FromUri] int moduleId)
        {
            int status = 0;
            string msg = string.Empty;
            try
            {
                var invTemplate = JsonConvert.DeserializeObject<SalesInvTemplate>(jsonObject.ToString());
                SetAuditFields(invTemplate, invTemplate.Id);
                invTemplate.IsActive = true;
                msg = ValidateObject(invTemplate, moduleId);
                if (string.IsNullOrEmpty(msg))
                {
                    if (invTemplate.Id == 0)
                    {
                        status = SalesInvTemplateBM.Instance.Insert(invTemplate);
                    }
                    else
                    {
                        status = SalesInvTemplateBM.Instance.Update(invTemplate);
                    }

                    if (status > 0)
                    {
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),0);
                    }
                    else
                    {
                        return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"),0);
                    }
                }
                else
                {
                    return new ResultObj(ResultCodes.ValidationError, msg,0);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
                return  new ResultObj(ResultCodes.UnkownError,exception.Message,0);
            }
        }


        private string ValidateObject(SalesInvTemplate InvTemplate, int moduleId)
        {
            string msg = string.Empty;
            var validator = new ObjectValidator(moduleId);
            var result = validator.ValidateObject(InvTemplate);
            float cost = 0;
            if (result.Length > 0)
            {
                msg += result;
            }
           
            return msg;
        }
    }
}
