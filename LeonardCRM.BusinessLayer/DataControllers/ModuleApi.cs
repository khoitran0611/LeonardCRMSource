using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using LeonardCRM.BusinessLayer.Helper;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class ModuleApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("MODULES", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public IList<vwModule> GetModules()
        {
            var modules = ModuleBM.Instance.GetAllModulesWithDefaultView(CurrentUserRole.Id);
            var dashboard = modules.Where(m => m.Dashboard).ToList();
            if (dashboard.Count == 2 && dashboard[0].Name == "dashboard" && dashboard[1].Name == "views"
                && !HasPermisson(Permission.Read, dashboard[1].Id))
                return modules.Where(m => !m.Dashboard).ToList();
            if (dashboard.SingleOrDefault(x => x.Name == "views") != null && !HasPermisson(Permission.Read, dashboard.Single(x => x.Name == "views").Id))
                return modules.Where(m => m.Name != "views").ToList();
            return modules;
        }
        [HttpGet]
        public IList<vwModule> GetReportModules()
        {
            return ModuleBM.Instance.GetAllModulesWithDefaultView(1).Where(m => m.ReportModule).ToList();
        }
        [HttpGet]
        public int GetModuleId(string id)
        {
            var module = ModuleBM.Instance.Single(m => m.Name.ToLower() == id);
            return module != null ? module.Id : 0;
        }
        [HttpGet]
        public int GetViewIdByModule(string id)
        {
            var view = ViewBM.Instance.GetViewByModule(id);
            return view != null ? view.Id : 0;
        }
        [HttpGet]
        public object GetPickListByModule(int id)
        {
            return new
                {
                    PickList = ListNameBM.Instance.GetListNameValuesByModuleId(id),
                    ReferenceList = ListNameBM.Instance.GetReferenceListsByModule(id)
                };
        }
        [HttpPost]
        public object GetPickListByModules([FromBody] JArray ids, [FromUri] bool needRef)
        {
            var modules = JsonConvert.DeserializeObject<int[]>(ids.ToString());
            return new
            {
                PickList = ListNameBM.Instance.GetListNameValuesByModules(modules),
                ReferenceList = needRef ? ListNameBM.Instance.GetReferenceListsByModules(modules) : new GetReferenceListValues_Result[] { }
            };
        }
        [HttpGet]
        public IList<GetReferenceListValues_Result> GetReferenceListsByModule(int id)
        {
            return ListNameBM.Instance.GetReferenceListsByModule(id);
        }
        [HttpGet]
        public bool HasEditPermission(int id)
        {
            var role = CurrentUserRole;
            if (role != null)
                return RolesPermissionsBM.Instance.Single(r => r.RoleId == role.Id && r.ModuleId == id && r.AllowEdit) != null;
            return false;
        }

        [HttpGet]
        public IList<vwModuleHasEntityField> GetModuleForEntityFields()
        {
            try
            {
                return ModuleBM.Instance.GetModuleForEntityFields().Where(m=> CurrentUserRole.Id != (int)UserRoles.ContractManager || m.Id == Constant.ModuleCustomer).ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<vwModuleHasEntityField>();
            }
        }

        [HttpGet]
        public IList<Eli_Modules> GetAllModule()
        {
            try
            {
                return ModuleBM.Instance.GetModules();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public virtual ResultObj UpdateSortOrder([FromBody] JArray jsonObject)
        {
            try
            {
                var entities = JsonConvert.DeserializeObject<IList<Eli_Modules>>(jsonObject.ToString());
                int status = ModuleBM.Instance.Update(entities);
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
        public virtual Eli_Modules GetModuleByModuleId(int id)
        {
            try
            {
                return ModuleBM.Instance.GetById(id) ?? new Eli_Modules();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new Eli_Modules();
            }
        }

        [HttpPost]
        public virtual ResultObj UpdateModule([FromBody] JObject jsonObject, [FromUri]int moduleId)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Eli_Modules>(jsonObject.ToString());
                string msg = ValidateModule(model, moduleId);
                if (string.IsNullOrEmpty(msg))
                {
                    int status = ModuleBM.Instance.Update(model);
                    if (status > 0)
                    {
                        var cache = new CacheManager();
                        cache.Remove(Constant.ModulesCacheKey + CurrentUserRole.Id);
                        return new ResultObj(ResultCodes.Success,
                            GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), 0);
                    }

                    return new ResultObj(ResultCodes.SavingFailed,
                        GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
                }
                return new ResultObj(ResultCodes.ValidationError, msg, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpGet]
        public string GetFrontEndKey()
        {
            return HttpUtility.UrlEncode(SecurityHelper.Encrypt(CurrentUserID + "/" + CurrentUserRole.Id));
        }

        private string ValidateModule(Eli_Modules model, int moduleId)
        {
            return new ObjectValidator(moduleId).ValidateObject(model);
        }
    }
}
