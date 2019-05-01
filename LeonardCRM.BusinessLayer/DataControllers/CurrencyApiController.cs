using System;
using System.Collections;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Eli.Common;
using System.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class CurrencyApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public Hashtable LoadData()
        {
            var hashTable = new Hashtable();
            var currencies = CurrencyBM.Instance.GetAll();
            var currencyNames = CurrencyNameBM.Instance.GetAll();
            hashTable.Add("Currency", currencies);
            hashTable.Add("CurrencyName", currencyNames);
            return hashTable;
        }

        [HttpGet]
        public virtual IList<Eli_CurrencyNames> GetAllCurrencyName()
        {
            try
            {
                var entities = CurrencyNameBM.Instance.GetAll();
                return entities;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
            }
            return null;
        }

        [HttpGet]
        public virtual IList<Eli_Currency> GetAll()
        {
            try
            {
                var entities = CurrencyBM.Instance.GetAll();
                return entities;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
            }
            return null;
        }

        [HttpGet]
        public Eli_Currency GetCurrencyById(int id)
        {
            try
            {
                return CurrencyBM.Instance.GetById(id) ?? new Eli_Currency { IsActive = true };
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new Eli_Currency();
            }
        }

        [HttpPost]
        public ResultObj Save([FromBody] JObject jsonObject, int moduleid)
        {
            try
            {
                var currency = JsonConvert.DeserializeObject<Eli_Currency>(jsonObject.ToString());
                SetAuditFields(currency, currency.Id);
                var msg = ValidateObject(currency, moduleid);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = 0;
                    if (currency.Id == 0)
                    {
                        status = CurrencyBM.Instance.Insert(currency);
                    }
                    status = CurrencyBM.Instance.Update(currency);

                    if (status > 0)
                    {
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), currency.Id);
                    }

                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), currency.Id);
                }

                return new ResultObj(ResultCodes.ValidationError, msg, 0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        private string ValidateObject(Eli_Currency currency, int moduleId)
        {
            string msg = string.Empty;
            var validator = new ObjectValidator(moduleId);
            var result = validator.ValidateObject(currency);
            if (result.Length > 0)
            {
                msg += result;
            }
            if (currency.Id == 0)
            {
                var currencies = CurrencyBM.Instance.GetAll();
                var entity = currencies.FirstOrDefault(c => c.Name == currency.Name);
                if (entity != null)
                {
                    msg += GetText("CURRENCY", "DUPLICATE_CURRENCY");
                }
            }
            return msg;
        }
    }
}
