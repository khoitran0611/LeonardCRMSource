using System;
using System.Collections.Generic;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class PicklistDependencyApiController:BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("PICKLIST_DEPENDENCY", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public Eli_ListDependency GetObjById(int id)
        {
            try
            {
                return PicklistDependencyBM.Instance.GetObjById(id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj SaveObject([FromBody] Eli_ListDependency model,int id)
        {
            try
            {
                model = SetObject(model);
                var msg = ValidateObject(model, id);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = PicklistDependencyBM.Instance.SaveObject(model);
                    if (status > 0)
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), model.Id);
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), model.Id);
                }
                return new ResultObj(ResultCodes.ValidationError, msg, model.Id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public ResultObj DeleteObjects([FromBody] IList<Eli_ListDependency> entities)
        {
            try
            {
                int status = PicklistDependencyBM.Instance.Delete(entities);
                if (status > 0)
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"), 0);
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }

        }

        [HttpGet]
        public IList<vwPicklistDependency> GetPicklistDependenciesByModuleId(int id)
        {
            try
            {
                return PicklistDependencyBM.Instance.GetPicklistDependenciesByModuleId(id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return null;
            }
        }

        private Eli_ListDependency SetObject(Eli_ListDependency model)
        {
            SetAuditFields(model,model.Id);
            foreach (var item in model.Eli_ListDependencyDetail)
            {
                SetAuditFields(item, item.Id);
            }
            return model;
        }

        private string ValidateObject(Eli_ListDependency model, int moduleId)
        {
            var msg = new ObjectValidator(moduleId).ValidateObject(model);
            if (model.Eli_ListDependencyDetail.Count == 0)
            {
                msg += GetText("SELECTED_SOURCE_VALUE_MSG") + "<br>";
            }
            if (model.Id == 0 && PicklistDependencyBM.Instance.Count(r=> r.MasterFieldId == model.MasterFieldId && r.ChildFieldId == model.ChildFieldId) > 0)
            {
                msg += GetText("EXIST_PICKLIST_MSG") + "<br>";
            }
            return msg;
        }
    }
}
