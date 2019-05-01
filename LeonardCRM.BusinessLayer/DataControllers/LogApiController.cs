using System;
using System.Collections.Generic;
using System.Web.Http;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Eli.Common;


namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class LogApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }
        [HttpGet]
        public virtual Eli_Log GetLogById([FromUri] int id)
        {
            try
            {
                var log = LogBM.Instance.GetById(id);
                return log;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return null;
            }
        }

        [HttpPost]
        public virtual ResultObj DeleteLog([FromBody]JArray jsonObject)
        {
            try
            {
                int status = 0;
                var entities = JsonConvert.DeserializeObject<IList<Eli_Log>>(jsonObject.ToString());
                status = LogBM.Instance.Delete(entities);
                if (status > 0)
                {
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0);
                }
                return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_ERROR"),0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.Message,0);
            }
        }

        [HttpGet]
        public virtual ResultObj ClearLog()
        {
            try
            {
                LogBM.Instance.ClearLog();
                return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.Message,0);
            }
        }
    }
}
