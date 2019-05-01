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
    public class ContractTemplateApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public virtual SalesContractTemplate GetContractTemplateById([FromUri] int templateId)
        {
            var entity = new SalesContractTemplate();
            try
            {
                entity = SalesContractTemplateBM.Instance.GetContractTmplById(templateId);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message,exception);
            }
            return entity;
        }

        [HttpPost]
        public virtual ResultObj DeleteContractTemplate([FromBody] JArray jsonArray)
        {
            try
            {
                int status = 0;
                var entities = JsonConvert.DeserializeObject<List<SalesContractTemplate>>(jsonArray.ToString());

                status = SalesContractTemplateBM.Instance.Delete(entities);
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
        public virtual ResultObj SaveContractTemplate([FromBody] JObject jsonObject, [FromUri] int moduleId)
        {
            int status = 0;
            string msg = string.Empty;
            try
            {
                var contractTemplate = JsonConvert.DeserializeObject<SalesContractTemplate>(jsonObject.ToString());
                SetAuditFields(contractTemplate, contractTemplate.Id);
                contractTemplate.IsActive = true;
                msg = ValidateObject(contractTemplate, moduleId);
                if (string.IsNullOrEmpty(msg))
                {
                    contractTemplate.TemplateContent = contractTemplate.TemplateContent.Replace("\t", "").Replace("\n", "");
                    status = SalesContractTemplateBM.Instance.UpdateContractTemplate(contractTemplate);                   

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
        
        private string ValidateObject(SalesContractTemplate InvTemplate, int moduleId)
        {
            string msg = string.Empty;
            var validator = new ObjectValidator(moduleId);
            var result = validator.ValidateObject(InvTemplate);
            if (result.Length > 0)
            {
                msg += result;
            }
           
            return msg;
        }
    }
}
