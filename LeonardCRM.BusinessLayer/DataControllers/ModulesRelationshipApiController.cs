using System;
using System.Collections.Generic;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class ModulesRelationshipApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("RELATIONSHIP", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public IList<Eli_Modules> GetModules()
        {
            try
            {
                return ModulesRelationshipBM.Instance.GetModules();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<Eli_Modules>();
            }
        }

        [HttpGet]
        public IList<Eli_ModuleRelationship> GetRelationshipByModules([FromUri] int masterModuleId
            ,[FromUri] int childModuleId)
        {
            try
            {
                return ModulesRelationshipBM.Instance.GetRelationshipByModules(masterModuleId, childModuleId);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj CreateRelationship([FromUri] int masterModuleId, [FromBody] List<Eli_EntityFields> listEntities)
        {
            try
            {
                if (listEntities == null || listEntities.Count < 3)
                {
                    return new ResultObj(ResultCodes.ValidationError,
                        GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
                }

                var maxAlias = ModulesRelationshipBM.Instance.GetMaxAlias();
                var masterEntities = listEntities[0];
                var childEntities = listEntities[1];

                var modulesRelationship = new Eli_ModuleRelationship
                {
                    MasterFieldId = masterEntities.Id,
                    MasterDisplayColumn = listEntities[2].FieldName,
                    ChildFieldId = childEntities.Id,
                    Alias = "a" + (maxAlias + 1)
                };

                //Check relationship duplicated
                if (ModulesRelationshipBM.Instance.IsDuplicatedRelationship(modulesRelationship))
                {
                    return new ResultObj(ResultCodes.ValidationError,
                       GetText("RELATIONSHIP", "DUPLICATED_MSG"), 0);
                }

                //Add to Modules Relationship
                var status = ModulesRelationshipBM.Instance.Insert(modulesRelationship);
                if (status > 0)
                {
                    //Update ForeignKey and ListSql
                    childEntities = EntityFieldBM.Instance.GetById(childEntities.Id);
                    SetAuditFields(childEntities, childEntities.Id);

                    ModulesRelationshipBM.Instance.UpdateChildEntity(childEntities,
                        masterModuleId,
                        masterEntities.FieldName,
                        modulesRelationship.MasterDisplayColumn);

                    return new ResultObj(ResultCodes.Success,
                        GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), status);
                }
                return new ResultObj(ResultCodes.SavingFailed,
                    GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), status);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public ResultObj DeleteRelationship([FromBody]int id)
        {
            try
            {
                var relationship = ModulesRelationshipBM.Instance.GetById(id);
                var childEntities = EntityFieldBM.Instance.GetById(relationship.ChildFieldId);

                //Update foreign key
                childEntities.ForeignKey = false;
                SetAuditFields(childEntities, childEntities.Id);
                EntityFieldBM.Instance.Update(childEntities);

                var status = ModulesRelationshipBM.Instance.Delete(relationship);

                if (status > 0)
                {
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"), 0);
                }

                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), 0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.ToString(), 0);
            }
        }
    }
}
