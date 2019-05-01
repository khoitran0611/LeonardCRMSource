using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Eli.Common;
using LeonardCRM.BusinessLayer;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;

namespace LeonardCRM.Web.Controllers
{
    public class HomeController : BasicController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }
        public ActionResult Index()
        {
            var roleId = CurrentUserRole.Id;
            if (roleId == (int)UserRoles.Customer || roleId == (int)UserRoles.DeliveryStaff)
                return RedirectPermanent("/login");
            return View();
        }

        public ActionResult DynamicGrid(int viewId, int moduleId, int? id)
        {
            if (viewId < 1) return PartialView("_BlankPartial");
            IList<vwFieldNameDataType> columns;
            var customView = false;
            var roleId = CurrentUserRole.Id;
            if (HttpContext.Session != null && HttpContext.Session["ColumnsVisibleChanged"] != null)
            {
                var viewCols = HttpContext.Session["ColumnsVisibleChanged"] as IList<vwViewColumn>;
                var fieldIds = viewCols.Where(r => r.Selected).Select(r => r.FieldId).ToList();
                var vId = (int) HttpContext.Session["viewId"];
                var mId = (int) HttpContext.Session["moduleId"];

                if (viewId != vId || moduleId != mId)
                {
                    columns = ViewBM.Instance.GenView(viewId, moduleId, roleId);
                    HttpContext.Session["ColumnsVisibleChanged"] = null;
                    HttpContext.Session["viewId"] = null;
                    HttpContext.Session["moduleId"] = null;
                }
                else
                {
                    customView = true;
                    columns = ViewBM.Instance.GenView(viewId, moduleId,roleId, fieldIds);
                    foreach (var item in columns)
                    {
                        var col = viewCols.Single(r => r.FieldId == item.FieldId);
                        item.SortOrder = col.SortOrder;
                        item.Width = col.Width ?? "100px";
                    }
                }
            }
            else
            {
                columns = ViewBM.Instance.GenView(viewId, moduleId, roleId);
            }
            if (columns.Count <= 0) return PartialView("_BlankPartial");
            columns = columns.OrderBy(r => r.SortOrder).ToList();
            id = id ?? 0;
            int width = 96;
            int count = 0;
            if (columns.Count(r => r.ColumnName == "Id" && r.Display) > 0)
            {
                width -= 8;
                ViewData["IdWidth"] = "50px";
                count++;
            }
            if (columns.Count(r => r.IsImage && r.Display) > 0)
            {
                width -= 7;
                ViewData["ImageWidth"] = "50px";
                count++;
            }

            DataBind(viewId, id.Value, width, columns, count, customView);
            ViewBag.HasReadPermission = HasPermisson(Permission.Read, moduleId);
            return PartialView("Grid/DynamicGrid", columns);
        }
        [HttpGet]
        public ActionResult SubView(int viewId, int moduleId, int id)
        {
            var roleId = CurrentUserRole.Id;
            string childview = "{" + viewId + "}";
            var currentView = ViewBM.Instance.Single(r => r.Id == viewId);
            if (currentView != null && currentView.MasterViewId.HasValue)
            {
                childview = "{" + currentView.MasterViewId.Value + "}";
            }
            var view = ViewBM.Instance.Single(r => r.ParentId.Contains(childview));
            if (viewId < 1 || view == null) return PartialView("_BlankPartial");
            var columns = ViewBM.Instance.GenSubView(view.Id, view.ModuleId, roleId);
            if (columns.Count <= 0) return PartialView("_BlankPartial");

            int width = 100;
            int count = 0;
            width -= 4;//column load child view
            if (columns.Count(r => r.ColumnName == "Id" && r.Display) > 0)
            {
                width -= 9;
                ViewData["IdWidth"] = "9%";
                count++;
            }
            if (columns.Count(r => r.IsImage && r.Display) > 0)
            {
                width -= 7;
                ViewData["ImageWidth"] = "7%";
                count++;
            }
            DataBind(viewId, id, width, columns, count,false);
            return PartialView("Grid/SubView", columns);
        }

        [HttpGet]
        public ActionResult RelatedView(int viewId, int id)
        {
            var roleId = CurrentUserRole.Id;
            //string childview = "{" + viewId + "}";
            //var currentView = ViewBM.Instance.Single(r => r.Id == viewId);
            //if (currentView != null && currentView.MasterViewId.HasValue)
            //{
            //    childview = "{" + currentView.MasterViewId.Value + "}";
            //}
            var view = ViewBM.Instance.Single(r => r.Id == viewId);
            if (viewId < 1 || view == null) return PartialView("_BlankPartial");
            var columns = ViewBM.Instance.GenSubView(view.Id, view.ModuleId, roleId);
            if (columns.Count <= 0) return PartialView("_BlankPartial");

            int width = 100;
            int count = 0;
            width -= 4;//column load child view
            if (columns.Count(r => r.ColumnName == "Id" && r.Display) > 0)
            {
                width -= 9;
                ViewData["IdWidth"] = "9%";
                count++;
            }
            if (columns.Count(r => r.IsImage && r.Display) > 0)
            {
                width -= 7;
                ViewData["ImageWidth"] = "7%";
                count++;
            }
            DataBind(viewId, id, width, columns, count, false);
            return PartialView("Grid/SubView", columns);
        }

        public ActionResult Navigation()
        {
            var modules = ModuleBM.Instance.GetAllModulesWithDefaultView(CurrentUserRole.Id).ToList();
            var dashboard = modules.Where(m => m.Dashboard).ToList();
            if (dashboard.Count == 2 && dashboard[0].Name == "dashboard" && dashboard[1].Name == "views"
                && !HasPermisson(Permission.Read, dashboard[1].Id))
                modules = modules.Where(m => m.Parent == 0 && !m.Dashboard).ToList();

            else if (dashboard.SingleOrDefault(x => x.Name == "views") != null && !HasPermisson(Permission.Read, dashboard.Single(x => x.Name == "views").Id))
                modules = modules.Where(m => m.Name != "views").ToList();

            return PartialView(modules.Where(m => m.Parent == 0).ToList());
        }

        public ActionResult _ToolBar()
        {
            var role = CurrentUserRole;
            var rolePermission = new Eli_RolesPermissions();
            if (role != null)
                rolePermission = RolesPermissionsBM.Instance.Single(r => r.RoleId == role.Id && r.ModuleId == GetModuleId);

            return PartialView(rolePermission);
        }

        [AllowAnonymous]
        public ActionResult Deny()
        {
            ViewBag.Title = GetText("REQUEST_DENY","TITLE");
            ViewBag.Msg = GetText("REQUEST_DENY", "INFO_TEXT");
            return View();
        }
        [AllowAnonymous]
        public ActionResult Error()
        {
            ViewBag.Title = GetText("PAGE_ERROR", "TITLE");
            ViewBag.Msg = GetText("PAGE_ERROR", "MESSAGE");
            return View();
        }
        [AllowAnonymous]
        public ActionResult NotFound()
        {
            ViewBag.Title = GetText("PAGE_NOT_FOUND", "TITLE");
            ViewBag.Msg = GetText("PAGE_NOT_FOUND", "MESSAGE");
            return View();
        }

        private void DataBind(int viewId, int id, int width, IList<vwFieldNameDataType> columns, int count,bool customView)
        {
            var rowWidth = (float)width / (columns.Count(r => r.Display) - count);
            ViewData["ColWidth"] = string.Format("{0}%", rowWidth);
            var view = ViewBM.Instance.SingleLoadWithReferences(r => r.Id == viewId, "Eli_ViewOrderBy");
            string hashTableKey = string.Format("{0}{1}{2}", columns[0].Name, viewId, id);
            string defaultTable = id > 0 ? string.Format("datalist.{0}", hashTableKey) : "datalist";
            string itemKey = string.Format("{0}{1}", columns[0].Name, id);
            var groupByObj = ViewGroupByBM.Instance.Single(r => r.ViewId == viewId);
            string groupByCloumn = groupByObj != null ? groupByObj.ColumnName : string.Empty;
            IList<ColumnExport> entities = columns.Where(r => r.Display).OrderBy(r=> r.SortOrder)
                                                  .Select(column => new ColumnExport
            {
                ModuleName = column.Name,
                ColumnName = column.ColumnName,
                LabelDisplay = column.LabelDisplay,
                Display = column.Display,
                IsInteger = column.IsInteger,
                IsDate = column.IsDate,
                IsDecimal = column.IsDecimal,
                IsText = column.IsText,
                IsList = column.IsList,
                IsTextArea = column.IsTextArea,
                IsTime = column.IsTime,
                IsEmail = column.IsEmail,
                IsUrl = column.IsUrl,
                IsMultiSelecttBox = column.IsMultiSelecttBox,
                IsCheckBox = column.IsCheckBox,
                IsImage = column.IsImage,
                IsDateTime = column.IsDateTime,
                IsCurrency = column.IsCurrency,
                Id = column.Id,
                Width = column.Width,
                ModuleId = column.ModuleId,
                ViewId = column.ViewId,
                FieldId = column.FieldId,
                SortOrder = column.SortOrder,
                Sortable = column.Sortable ?? true
            }).ToList();
            ViewBag.IsShareView = view.Shared.ToString();
            ViewBag.ColumnNames = JsonConvert.SerializeObject(entities).Replace("'","\\'"); //ConvertStringArrayToString(viewcolumnNames);
            ViewBag.HashTableKey = defaultTable;
            ViewBag.HashTableLength = string.Format("{0}.length", defaultTable);
            ViewBag.ViewId = columns[0].ViewId;
            ViewBag.ModuleId = columns[0].ModuleId;
            ViewBag.ModuleName = columns[0].Name;
            ViewBag.Item = itemKey;
            ViewBag.ID = id;
            ViewBag.ItemSelected = string.Format("{0}.Selected", itemKey);
            ViewBag.GroupByColumn = string.Format("{0}", groupByCloumn);
            ViewBag.PageSize = view.PageSize ?? SiteSettings.ITEMS_PER_PAGE;
            ViewBag.DataReOg = GetText("COMMON", "DATA_REORGANIZATION");
            if (view.Eli_ViewOrderBy.Count > 0)
            {
                ViewBag.SortExpression = string.Format("{0} {1}", view.Eli_ViewOrderBy.Single().ColumnName,
                    view.Eli_ViewOrderBy.Single().OrderDirection);
            }
            else
            {
                ViewBag.SortExpression = "Id Asc";
            }
        }
    }
}
