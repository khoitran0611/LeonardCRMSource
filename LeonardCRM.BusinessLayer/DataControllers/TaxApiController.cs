using System;
using System.Collections.Generic;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class TaxApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("TAX", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public IList<Eli_Tax> GetAll()
        {
            try
            {
                return TaxBM.Instance.GetAll();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpGet]
        public Eli_Tax GetTaxByTaxId(int id)
        {
            try
            {
                var model = TaxBM.Instance.GetById(id);
                return model ?? new Eli_Tax();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj SaveTax([FromBody] JObject jsonObject,int id)//id is ModuleId
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Eli_Tax>(jsonObject.ToString());
                model = SetTax(model);
                string msg = ValidateTax(model, id);

                if (string.IsNullOrEmpty(msg))
                {
                    var status = model.Id > 0 ? TaxBM.Instance.Update(model) : TaxBM.Instance.Insert(model);
                    if (status > 0)
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), model.Id);
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), model.Id);
                }
                return new ResultObj(ResultCodes.ValidationError,msg, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        private Eli_Tax SetTax(Eli_Tax model)
        {
            SetAuditFields(model,model.Id);
            return model;
        }

        private string ValidateTax(Eli_Tax model, int moduleId)
        {
            string msg = new ObjectValidator(moduleId).ValidateObject(model);
            if (model.Id > 0)
            {
                if (TaxBM.Instance.Count(r => r.TaxName.Equals(model.TaxName) && r.Id != model.Id) > 0)
                {
                    msg += string.Format("{0} <br>", GetText("EXIST_NAME"));
                }
            }
            else
            {
                if (TaxBM.Instance.Count(r => r.TaxName.Equals(model.TaxName)) > 0)
                {
                    msg += string.Format("{0} <br>", GetText("EXIST_NAME"));
                }
            }
            return msg;
        }
    }
}
