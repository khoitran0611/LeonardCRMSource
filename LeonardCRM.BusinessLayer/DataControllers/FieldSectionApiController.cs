using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class FieldSectionApiController:BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("LAYOUT_EDITOR", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public IList<Eli_FieldsSection> GetFieldSectionByModuleId(int id)
        {
            try
            {
                return FieldSectionBM.Instance.GetFieldSectionByModuleId(id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj SaveObjs([FromBody] IList<Eli_FieldsSection> models, int id,[FromUri]int moduleId)
        {
            try
            {
                models = SetObjs(models, moduleId);
                var msg = ValidateObjs(models, id);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = FieldSectionBM.Instance.SaveObjs(models, moduleId);
                    if (status > 0)
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), status);
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), status);
                }
                return new ResultObj(ResultCodes.ValidationError, msg, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        private IList<Eli_FieldsSection> SetObjs(IList<Eli_FieldsSection> models,int moduleId)
        {
            models = models.Where(r => r.ModuleId == moduleId).ToList();
            byte i = 1;
            foreach (var model in models)
            {
                byte j = 1;
                foreach (var item in model.Eli_FieldsSectionDetail.Where(r=> r.LeftSide))
                {
                    item.SortOrder = j;
                    item.SectionId = model.Id;
                    SetAuditFields(item, item.Id);
                    j++;
                }
                j = 1;
                foreach (var item in model.Eli_FieldsSectionDetail.Where(r => r.LeftSide == false))
                {
                    item.SortOrder = j;
                    item.SectionId = model.Id;
                    SetAuditFields(item, item.Id);
                    j++;
                }
                model.SortOrder = i;
                SetAuditFields(model, model.Id);
                i++;
            }
            return models;
        }

        private string ValidateObjs(IEnumerable<Eli_FieldsSection> models, int moduleId)
        {
            var msg = string.Empty;
            foreach (var model in models)
            {
                var result = new ObjectValidator(moduleId).ValidateObject(model);
                if (!string.IsNullOrEmpty(result))
                {
                    msg += string.Format(GetText("NUMBER_MSG"), model.SortOrder) + "<br/>";
                    msg += string.Format("{0}",result);
                }
            }
            return msg;
        }
    }
}
