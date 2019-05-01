using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Security;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class UserApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("USERS", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public Eli_User GetUserById(int id)
        {
            var entity = id > 0 ? UserBM.Instance.GetByIdWithRoles(id) : new Eli_User();
            if (entity.Id > 0)
            {
                entity.PasswordConfirm = entity.Password;
            }
            return entity;
        }

        [HttpPost]
        public ResultObj Delete([FromBody] JArray users)
        {
            var entities = JsonConvert.DeserializeObject<IList<Eli_User>>(users.ToString());
            //DO NOT allow to delete the administrator account
            var admin = entities.FirstOrDefault(e => e.Id == 1);
            if (admin != null)
                entities.Remove(admin);
            var res = 0;
            var outNames = string.Empty;
            if (entities.Count > 0)
            {
                var ids = String.Join(",", entities.Select(r => r.Id).ToArray());
                res = UserBM.Instance.Delete_Users(ids,out outNames);
            }


            ResultObj resultObj = res > 0 ? new ResultObj( ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0) 
                                    : new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "DELETE_ERROR"), 0);
            resultObj.Names = outNames;
            return resultObj;
        }

        [HttpPost]
        public ResultObj Save([FromBody] JObject user, [FromUri] int moduleId)
        {
            var obj = JsonConvert.DeserializeObject<Eli_User>(user.ToString());
            SetAuditFields(obj, obj.Id);
            if (moduleId == 0)
                moduleId = Constant.ModuleUser;
            var result = new ObjectValidator(moduleId).ValidateObject(obj);

            result += CheckUserFurther(obj, result);

            if (string.IsNullOrEmpty(result))
            {
                if (obj.Eli_Roles != null && obj.RoleId != obj.Eli_Roles.Id)
                    obj.Eli_Roles = RolesBM.Instance.GetById(obj.RoleId);
                var res = obj.Id <= 0 ? UserBM.Instance.InsertUser(obj) : UserBM.Instance.Update(obj);
                return res > 0 ? new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),0) : 
                    new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_ADMIN"),0);
            }
            return new ResultObj(ResultCodes.ValidationError, result,0);
        }

        private string CheckUserFurther(Eli_User obj, string currentResult)
        {
            var result = string.Empty;

            //checking username invalid
            //if (!string.IsNullOrEmpty(obj.LoginName) && !IsUsernameValid(obj.LoginName))
            //    result += GetText("USERNAME_INVALID") + "<br>";

            //Checking denied list
            //var denyArr = GetText("DENY_LIST_NAME");
            //if (!string.IsNullOrEmpty(obj.LoginName) && obj.Id <= 0 && IsStringExistInDenyList(obj.LoginName, denyArr))
            //    result += GetText("DENY_NAME") + "<br>";

            if(!string.IsNullOrEmpty(obj.Name) && obj.Name.Trim().Split(' ').Length < 2 && obj.RoleId != UserRoles.Administrator.GetHashCode())
            {
                result += GetText("USERNAME_CUSTOM_ERROR_MSG") + "<br>";
            }

            //Checking if the passwords match
            if (obj.Password != obj.PasswordConfirm)
                result += GetText("PASSWORD_MATCH") + "<br>";

            Eli_User us = null;
            if (obj.Id > 0)
            {
                us = UserBM.Instance.GetById(obj.Id);
            }
            if (us == null || us.Password != obj.Password)
            {
                //checking password invalid
                if (!string.IsNullOrEmpty(obj.Password) && !Utilities.IsPasswordValid(obj.Password))
                    result += GetText("PASSWORD_INVALID") + "<br>";
            }

            //check if username exists
            //if ((obj.Id <= 0 && UserBM.Instance.First(u => u.LoginName.ToLower() == obj.LoginName.ToLower()) != null)
            //    || (obj.Id > 0 && UserBM.Instance.First(u => u.LoginName.ToLower() == obj.LoginName.ToLower() && u.Id != obj.Id) != null))
            //    result += GetText("USERNAME_EXIST") + "<br>";

            //check if email exists
            if ((obj.Id <= 0 && UserBM.Instance.ExistEmail(obj.Email))
                || (obj.Id > 0 && UserBM.Instance.First(u => u.Email.ToLower() == obj.Email.ToLower() && u.Id != obj.Id) != null))
                result += GetText("EMAIL_EXIST") + "<br>";

            if (currentResult == "" && (result == "<br>" || result == "") && ((us != null && us.Password != obj.Password) || us == null))
            {
                obj.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(obj.Password, "md5");
            }
            return result != "<br>" ? result : "";
        }
        [HttpGet]
        public IList<vwUserRole> GetAllUserGroup()
        {
            try
            {
                return UserBM.Instance.GetAllUserGroup(CurrentUserRole.Id, CurrentUserID);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return new List<vwUserRole>();
            }
        }
        [HttpGet]
        public IList<vwUserRole> GetAllUser()
        {
            try
            {
                var models = UserBM.Instance.GetAllUserGroup();
                return models;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<vwUserRole>();
            }
        }

        [HttpGet]
        public bool CheckIfExistEmail([FromUri]string email)
        {
            try
            {
                return UserBM.Instance.ExistEmail(email);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }
    }
}
