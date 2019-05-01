using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Eli.Common;
using Newtonsoft.Json;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class ViewCustomApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpGet]
        public ViewCustomDisplayModel PagingData(int viewId
            , int pageIndex
            , int pageSize
            , string sortExpression
            , string filterStr
            , string groupName
            , bool isFilterExpiring = false)
        {
            try
            {
                var filterArray = JsonConvert.DeserializeObject<List<FilterObj>>(filterStr);

                int totalRow;
                string groupJson;

                var customViewModel = new ViewCustomDisplayModel
                {
                    Data = ViewCustomBM.Instance.GetViewCustom(viewId, pageIndex,
                        pageSize,
                        out totalRow,
                        filterArray,
                        sortExpression ?? "",
                        groupName ?? "",
                        out groupJson).ToList(),
                    Total = totalRow,
                    ListGroup = JsonConvert.DeserializeObject<List<GroupItem>>(groupJson)
                };

                return customViewModel;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj CreateCustomView(ViewCustomCreatedModel viewCustom)
        {
            if (ViewCustomBM.Instance.CreateCustomView(viewCustom, CurrentUserID))
            {
                return new ResultObj(ResultCodes.Success,
                    GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), 0);
            }
            return new ResultObj(ResultCodes.SavingFailed,
                GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
        }

        [HttpPost]
        public ResultObj SaveCustomView(ViewCustomUpdatedModel viewCustom)
        {
            SetAuditFields(viewCustom, viewCustom.ViewCustom.Id);

            if (ViewCustomBM.Instance.SaveCustomView(viewCustom, CurrentUserID))
            {
                return new ResultObj(ResultCodes.Success,
                    GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), viewCustom.ViewCustom.Id);
            }
            return new ResultObj(ResultCodes.SavingFailed,
                GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
        }

        [HttpPost]
        public virtual ResultObj SaveView([FromBody] Eli_ViewCustom view)
        {
            SetObject(view);
            SetAuditFields(view, view.Id);
            var status = ViewCustomBM.Instance.SaveView(view, CurrentUserID);
            if (status)
            {
                return new ResultObj(ResultCodes.Success,
                    GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), view.Id);
            }
            return new ResultObj(ResultCodes.SavingFailed,
                GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
        }

        [HttpGet]
        public virtual Eli_ViewCustom GetViewObjById(int viewId)
        {
            var entity = viewId > 0
                  ? ViewCustomBM.Instance.GetViewObjById(viewId)
                  : new Eli_ViewCustom();

            //get the order columns
            var orderColumns = entity.OrderBy != null &&  entity.OrderBy.Trim() != "" ? entity.OrderBy.Split(',') : null;
            if (orderColumns != null)
            {
                foreach (var col in orderColumns)
                {
                    if (col.Trim() != "")
                    {
                        if (entity.OrderColumns == null) entity.OrderColumns = new List<OrderByItem>();
                        var item = col.Trim().Split(' ');
                        entity.OrderColumns.Add(new OrderByItem() { Column = item[0], Direction = item[1].ToLower() });
                    }
                }
            }
            else
            {
                entity.OrderColumns = new List<OrderByItem>() { new OrderByItem() { Column = "", Direction = "" } };
            }

            if (!string.IsNullOrEmpty(entity.GroupBy))
            {
                entity.GroupColumns = entity.GroupBy.Trim().Split(',').Select(x => new GroupItem() { GroupName = x }).ToList();
            }
            else
            {
                entity.GroupColumns = new List<GroupItem>() { new GroupItem() { GroupName = "" } };

            }

            return entity;
        }

        [HttpGet]
        public virtual ResultObj DeleteCustomView(int viewId)
        {
            var status = ViewCustomBM.Instance.DeleteById(viewId, this.CurrentUserID);
            if (status > 0)
            {
                return new ResultObj(ResultCodes.Success,
                    GetText("COMMON", "DELETE_SUCCESS"), viewId);
            }

            if (status == -1)
            {
                return new ResultObj(ResultCodes.SavingFailed,
               GetText("CUSTOM_VIEW", "NOT_OWNER_ERROR_MESSAGE"), 0);
            }
            else
            {
                return new ResultObj(ResultCodes.SavingFailed,
                GetText("COMMON", "DELETE_ERROR"), 0);
            }
        }

        private void SetObject(Eli_ViewCustom view)
        {
            if (view != null)
            {
                if (view.OrderColumns != null && view.OrderColumns.Count > 0)
                    view.OrderBy = string.Join(" ", view.OrderColumns[0].Column, view.OrderColumns[0].Direction);

                if (view.GroupColumns != null && view.GroupColumns.Count > 0)
                    view.GroupBy = view.GroupColumns[0].GroupName;
            }
        }
    }
}
