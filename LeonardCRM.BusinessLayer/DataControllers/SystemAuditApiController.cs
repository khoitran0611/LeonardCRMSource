using System;
using System.Collections;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SystemAuditApiController:BaseApiController
    {
        [HttpPost]
        public virtual PageInfo GetByModuleIdnRecordId([FromBody] JObject jsonObject)
        {
            try
            {
                var pageInfo = JsonConvert.DeserializeObject<PageInfo>(jsonObject.ToString());
                pageInfo.Models.Add("sysAudit", SysAuditBM.Instance.GetByModuleIdnRecordId(pageInfo));
                return pageInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new PageInfo();
            }
        }

        [HttpPost]
        public virtual PageInfo ServerFilter([FromBody] JObject jsonObject, [FromUri] int moduleId, [FromUri] int recordId,[FromUri] int pageIndex)
        {
            try
            {
                var sysAudit = JsonConvert.DeserializeObject<Eli_SysAudit>(jsonObject.ToString());
                var pageInfo = new PageInfo
                {
                    ModuleId = moduleId, 
                    Id = recordId, 
                    Models = new Hashtable(),
                    PageIndex = pageIndex,
                    PageSize = SiteSettings.ITEMS_PER_PAGE
                };
                pageInfo.Models.Add("sysAudit", SysAuditBM.Instance.ServerFilter(pageInfo, sysAudit));
                return pageInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new PageInfo();
            }
        }
    }
}
