using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.BusinessLayer.Common;
using System.Web.Http;
using Eli.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class ListNamesApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public Eli_ListNames GetListNameById([FromUri]int id)
        {
            try
            {
                var entity = ListNameBM.Instance.GetById(id);

                if (!string.IsNullOrWhiteSpace(entity.Module))
                {
                    entity.Module = entity.Module.Substring(0, entity.Module.Length - 1);
                    int n;
                    entity.ModuleIds = entity.Module.Split(',')
                        .Select(s => int.TryParse(s, out n) ? n : 0).ToArray();
                }

                entity.Eli_ListValues =
                    ListValueBM.Instance.Find(l => l.ListNameId == entity.Id).OrderBy(ll => ll.ListOrder).ToList();
                return entity;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return null;
            }
        }
        [HttpGet]
        public IList<Eli_Modules> GetAllModule([FromUri] int moduleId)
        {
            try
            {
                var entities = ModuleBM.Instance.Find(m => m.IsActive && m.IsPublished  && m.NeedPickList ).ToList();
                return entities;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return null;
            }
        }

        [HttpPost]
        public ResultObj SaveListName([FromBody] Eli_ListNames listName, [FromUri] int moduleId)
        {
            try
            {
                listName.Module = listName.ModuleIds != null && listName.ModuleIds.Any()
                      ? String.Join(",", listName.ModuleIds) + ","
                      : string.Empty;

                SetAuditFields(listName, listName.Id);
                foreach (var list in listName.Eli_ListValues)
                {
                    if (listName.Id == 0)
                    {
                        list.Active = true;
                    }
                    list.ListNameId = listName.Id;
                    SetAuditFields(list,list.Id);
                }

                var msg = ValidateObject(listName, moduleId);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = listName.Id == 0
                                 ? ListNameBM.Instance.AddListName(listName)
                                 : ListNameBM.Instance.UpdateListName(listName);
                    if (status > 0)
                    {
                        if (listName.ModuleIds != null)
                            foreach (var id in listName.ModuleIds)
                            {
                                new CacheManager().Remove("PickList_" + id);    
                            }
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),0);
                    }
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"),0);
                }
                return new ResultObj(ResultCodes.ValidationError, msg,0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.Message,0);
            }
        }

        [HttpPost]
        public ResultObj DeleteListName([FromBody]JArray jsonArray)
        {
            try
            {
                int status = 0;
                var entities = JsonConvert.DeserializeObject<List<Eli_ListNames>>(jsonArray.ToString());
                if (entities != null)
                {
                    foreach (var entity in entities)
                    {
                        var listname = ListNameBM.Instance.GetById(entity.Id);
                        if (listname != null)
                        {
                            listname.Active = false;
                        }
                        status= ListNameBM.Instance.Update(listname);
                    }
                }
                if (status > 0)
                {
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"),0);
                }

                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"),0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.ToString(),0);
            }
        }

        [HttpGet]
        public IList<Eli_ListNames> GetListNameByModuleId(int id)
        {
            try
            {
                var modules = id + ",";

                return ListNameBM.Instance.Find(r => r.Module.Contains(modules) && r.Active);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<Eli_ListNames>();
            }
        }
        [HttpGet]
        public IList<vwListNameValue> GetListValueByModuleListName([FromUri]int moduleId, [FromUri]string listName)
        {
            try
            {
                return ListNameBM.Instance.GetListNameValuesByModuleId(moduleId,listName);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<vwListNameValue>();
            }
        }
        [HttpGet]
        public Hashtable GetListValueByFieldIds([FromUri]int masterFieldId, [FromUri]int childFieldId)
        {
            try
            {
                return ListValueBM.Instance.GetListValueByFieldIds(masterFieldId, childFieldId);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message,ex);
                return null;
            }
        }

        private string ValidateObject(Eli_ListNames listName, int moduleid)
        {
            string msg = string.Empty;
            var validator = new ObjectValidator(moduleid);
            var result = validator.ValidateObject(listName);
            if (listName.Id == 0)
            {
                var entities = ListNameBM.Instance.Find(l => l.Module == listName.Module).ToList();
                foreach (var entity in entities)
                {
                    if (entity.ListName.Equals(listName.ListName))
                    {
                        msg += GetText("PICKLIST", "LISTNAME_DULICATED");
                        break;
                    }
                }
            }

            if (result.Length > 0)
            {
                msg += result;
            }

            if (listName.Eli_ListValues != null && listName.Eli_ListValues.Count > 0)
            {
                foreach (var listvalue in listName.Eli_ListValues)
                {
                    if (string.IsNullOrEmpty(listvalue.Description))
                    {
                        msg += GetText("PICKLIST", "LISTVALUE_DESC_REQUIRED");
                    }

                    int lookupid;
                    bool flag = int.TryParse(listvalue.LookupId.ToString(), out lookupid);
                    if (!flag)
                    {
                        msg += GetText("PICKLIST", "INVALID_LOOKUPID");
                    }
                }
            }

            return msg;
        }
    }
}
