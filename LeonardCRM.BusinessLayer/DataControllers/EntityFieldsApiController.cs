using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class EntityFieldsApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if(page == null)
                return LocalizeHelper.Instance.GetText("ENTITY_FIELD", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public IList<vwViewColumn> GetEntityFieldByModuleId(int id)
        {
            try
            {
                return EntityFieldBM.Instance.GetViewColumnsByModule(id, true, CurrentUserRole.Id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<vwViewColumn>();
            }
        }

        [HttpGet]
        public IList<Eli_EntityFields> GetManageFieldsByModuleId(int id)
        {
            try
            {
                var entities = EntityFieldBM.Instance.GetManageFieldsByModuleId(id);
                return entities;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<Eli_EntityFields>();
            }
        }

        [HttpPost]
        public ResultObj UpdateSortOrder([FromBody] JArray jsonObject)
        {
            try
            {
                var entities = JsonConvert.DeserializeObject<IList<Eli_EntityFields>>(jsonObject.ToString());
                int status = EntityFieldBM.Instance.Update(entities);
                if (status > 0)
                    return new ResultObj(ResultCodes.Success,
                        GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), 0);
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

        [HttpGet]
        public Eli_EntityFields GetEntityFieldById(int id)
        {
            try
            {
                return EntityFieldBM.Instance.GetById(id) ?? new Eli_EntityFields { Deletable = true };
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Eli_EntityFields() { Deletable = true };
            }
        }

        [HttpPost]
        public ResultObj SaveEntityField([FromBody] JObject jsonObject)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Eli_EntityFields>(jsonObject.ToString());
                model = SetModel(model);
                var msg = ValidateModel(model);
                if (string.IsNullOrEmpty(msg))
                {
                    model.FieldName = model.FieldName.RemoveDirtySymbols("_").RemoveDuplicatedSymbol("_");
                    int status = model.Id > 0 ? EntityFieldBM.Instance.Update(model)
                                              : EntityFieldBM.Instance.Insert(model);
                    if (status > 0)
                        return new ResultObj(ResultCodes.Success,
                            GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), model.Id);
                    return new ResultObj(ResultCodes.SavingFailed,
                    GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), model.Id);
                }
                return new ResultObj(ResultCodes.ValidationError, msg, model.Id);

            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public ResultObj DeleteEntityField([FromBody] JObject jsonObject)
        {
            try
            {
                var entity = JsonConvert.DeserializeObject<Eli_EntityFields>(jsonObject.ToString());
                var msg = ValidateDeleteEntityField(entity);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = EntityFieldBM.Instance.Delete(entity);
                    return status > 0 ? new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"), entity.Id)
                                      : new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), entity.Id);
                }
                return new ResultObj(ResultCodes.ValidationError,msg, entity.Id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpGet]
        public IList<Eli_EntityFields> GetFieldListType(int id)
        {
            try
            {
                var models = EntityFieldBM.Instance.Find(r => r.ModuleId == id && r.IsActive && r.DataTypeId == Constant.ListType &&
                                                               (r.ForeignKey.HasValue == false || r.ForeignKey.Value == false));
                return models;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return null;
            }
        }

        [HttpGet]
        public IList<Eli_EntityFields> GetAllCustFieldByModule(int id)
        {
            try
            {
                return EntityFieldBM.Instance.GetAllCustFieldByModule(id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return null;
            }
        }

        private string ValidateDeleteEntityField(Eli_EntityFields entity)
        {
            var msg = string.Empty;
            if (EntityFieldBM.Instance.CheckEntityFieldIsUsing(entity.Id))
            {
                msg += GetText( "FIELD_BE_USED_MSG");
            }
            return msg;
        }
        private Eli_EntityFields SetModel(Eli_EntityFields model)
        {
            if (model.Id < 1)
            {
                model.Deletable = true;
                model.SortOrder = EntityFieldBM.Instance.Find(r=> r.ModuleId == model.ModuleId).Max(r => r.SortOrder) + 1;
            }
            SetAuditFields(model, model.Id);
            return model;
        }

        private string ValidateModel(Eli_EntityFields model)
        {
            var msg = string.Empty;

            if (string.IsNullOrEmpty(model.FieldName) || !Regex.IsMatch(model.FieldName, @"^[a-zA-Z0-9_]+$"))
            {
                msg += GetText("FIELD_NAME_ERROR_MSG") + "<br/>";
            }
            if (string.IsNullOrEmpty(model.LabelDisplay))
            {
                msg += GetText("LABEL_DISPLAY_ERROR_MSG") + "<br/>";
            }
            if (model.DataTypeId < 1)
            {
                msg += GetText("DATA_TYPE_REQUIRED_MSG") + "<br/>";
            }
            if(!string.IsNullOrEmpty(model.RegularExpression) && !Utilities.IsRegexPattern(model.RegularExpression))
            {
                msg += GetText("REGULAR_EXPRESSION_ERROR_MSG") + "<br/>";
            }

            if (string.IsNullOrEmpty(msg))
            {
                var entities = EntityFieldBM.Instance.GetAll().Where(e => e.ModuleId == model.ModuleId).ToList();
                if (model.Id > 0)
                {
                    var entity = entities.FirstOrDefault(e => e.FieldName.ToLower().Equals(model.FieldName.ToLower()) && e.Id != model.Id);
                    if (entity != null)
                    {
                        msg += GetText("FIELD_NAME_EXIST") + "<br/>";
                    }
                }
                else
                {
                    var entity = entities.FirstOrDefault(e => e.FieldName.ToLower().Equals(model.FieldName.ToLower()));
                    if (entity != null)
                    {
                        msg += GetText("FIELD_NAME_EXIST") + "<br/>";
                    }
                }

            }
            return msg;
        }
    }
}
