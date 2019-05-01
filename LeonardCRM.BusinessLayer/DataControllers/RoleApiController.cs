using System;
using System.Collections.Generic;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class RoleApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("ROLES", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public IList<Eli_Roles> GetAllRoles()
        {
            return RolesBM.Instance.GetAll();
        }
        [HttpGet]
        public IList<Eli_Roles> GetRoles()
        {
            return RolesBM.Instance.GetAllRoles(CurrentUserRole.Id);
        }
        [HttpGet]
        public Eli_Roles GetRoleById(int id)
        {
            try
            {
                var role = RolesBM.Instance.GetRolebyRoleId(id);
                role.ParentArray = new int[] { };
                if (role.Id > 0 && !string.IsNullOrEmpty(role.Parent))
                {
                    var tempArray = role.Parent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if(tempArray != null && tempArray.Any())
                    {
                        role.ParentArray = tempArray.Select(x => int.Parse(x)).ToArray();
                    }                    
                }
                return role;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Eli_Roles();
            }
        }

        [HttpPost]
        public ResultObj DeleteRoles([FromBody] JArray jsonObject)
        {
            try
            {
                var entities = JsonConvert.DeserializeObject<IList<Eli_Roles>>(jsonObject.ToString());
                if (entities.Count > 0)
                {
                    int status = RolesBM.Instance.DeleteRoles(entities);
                    if (status > 0)
                    {
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0);
                    }
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"),0);
                }
                return new ResultObj(ResultCodes.Success, GetText("NO_ITEM"),0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"),0);
            }
        }
        [HttpPost]
        public ResultObj SaveRoles([FromBody] JObject jsonObject)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Eli_Roles>(jsonObject.ToString());
                model = SetRole(model);
                string msg = ValidateFillForm(model);

                if (string.IsNullOrEmpty(msg))
                {
                    int status = RolesBM.Instance.SaveRole(model);
                    if (status > 0)
                        return new ResultObj(ResultCodes.Success,
                            GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),0);
                    return new ResultObj(ResultCodes.SavingFailed,
                        GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"),0);
                }
                return new ResultObj(ResultCodes.ValidationError, msg,0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"),0);
            }
        }

        private Eli_Roles SetRole(Eli_Roles entity)
        {
            if (entity.ParentArray != null && entity.ParentArray.Any())
            {
                entity.Parent = string.Join(",", entity.ParentArray);
            }
            else
            {
                entity.Parent = "";
            }

            SetAuditFields(entity, entity.Id);

            if (entity.Eli_RolesPermissions != null && entity.Eli_RolesPermissions.Count > 0)
            {
                foreach (var permission in entity.Eli_RolesPermissions)
                {
                    SetAuditFields(permission, permission.Id);
                    foreach (var vwFieldsRolesField in permission.EntityFields)
                    {
                        SetAuditFields(vwFieldsRolesField, vwFieldsRolesField.Id ?? 0);    
                    }
                }
            }
            return entity;
        }

        private string ValidateFillForm(Eli_Roles entity)
        {
            string msg = new ObjectValidator(entity.ModuleId).ValidateObject(entity);
            if (entity.Id > 0)
            {
                var role = RolesBM.Instance.Single(r => r.Name.ToLower() == entity.Name.ToLower() &&
                                                        r.Id != entity.Id);
                msg += role != null ? GetText("EXIST_NAME") : string.Empty;
            }
            else
            {
                if (entity.Name.ToLower() == "administrator")
                {
                    msg += GetText("SPECIAL_NAME");
                }
                else
                {
                    var role = RolesBM.Instance.Single(r => r.Name.ToLower() == entity.Name.ToLower());
                    msg += role != null ? GetText("EXIST_NAME") : string.Empty;
                }
            }
            return msg;
        }

        public Select2HierarchicalData GetRoleUserHierachy()
        {
            return RolesBM.Instance.GetUsersHierachy(CurrentUserRole.Id, CurrentUserID);
        }

        [HttpGet]
        public int GetCurrentUserRole()
        {
            return CurrentUserRole != null ? CurrentUserRole.Id : 0;
        }
    }
}
