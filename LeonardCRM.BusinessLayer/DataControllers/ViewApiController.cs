using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.ViewRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ConfigValues = Eli.Common.ConfigValues;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class ViewApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("VIEW", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpPost]
        public virtual PageInfo GetView([FromBody] JObject jsonObject)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<PageInfo>(jsonObject.ToString());
                if (HasPermisson(Permission.Read, model.ModuleId))
                    return CheckPageInfo(model, null);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new PageInfo();
            }
        }

        [HttpGet]
        public virtual Eli_Views GetViewObjById(int id)
        {
            var entity = id > 0
                ? ViewBM.Instance.SingleLoadWithReferences(v => v.Id == id, "Eli_ViewColumns", "Eli_ViewConditions",
                    "Eli_ViewOrderBy", "Eli_ViewGroupBy")
                : new Eli_Views();

            if (entity == null || (id > 0 && CurrentUser.Id != entity.CreatedBy && entity.Shared == false)) return null;
            var viewColumns = EntityFieldBM.Instance.GetViewColumnsByModule(entity.ModuleId, true, CurrentUserRole.Id); //Full Columns From
            var i = 0;
            if (id > 0)
            {
                var colIds = entity.Eli_ViewColumns.Select(r => r.FieldId).ToList();
                var viewCols = viewColumns.Where(r => !colIds.Contains(r.FieldId)).ToList();
                i = entity.Eli_ViewColumns.Count;
                // Add the missing columns
                foreach (var item in viewCols)
                {
                    var viewCol = new Eli_ViewColumns
                    {
                        ColumnName = item.ColumnName,
                        LabelDisplay = item.LabelDisplay,
                        ViewId = entity.Id,
                        FieldId = item.FieldId,
                        Visible = false,
                        Width = "100px",
                        AllowGroup = item.AllowGroup.HasValue && item.AllowGroup.Value
                    };
                    entity.Eli_ViewColumns.Add(viewCol);

                }
                // Remove Id Column
                entity.Eli_ViewColumns = entity.Eli_ViewColumns.Where(r => viewColumns.Any(vc => vc.FieldId == r.FieldId))
                                               .OrderBy(r => r.SortOrder).ToList();
                // Update Column name & Label Display
                foreach (var item in entity.Eli_ViewColumns.Where(r => colIds.Contains(r.FieldId)))
                {
                    var col = viewColumns.Single(r => r.FieldId == item.FieldId);
                    item.ColumnName = col.ColumnName;
                    item.LabelDisplay = col.LabelDisplay;
                    item.SortOrder = i++;
                    item.AllowGroup = col.AllowGroup.HasValue && col.AllowGroup.Value;
                }
                if (!string.IsNullOrEmpty(entity.ParentId))
                {
                    entity.Parent = ConvertStringtoArray(entity.ParentId);
                }

                var tempArray = !string.IsNullOrEmpty(entity.UserRole) ? entity.UserRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
                if (tempArray != null && tempArray.Any())
                {
                    entity.UserRoleArray = tempArray.Select(x => int.Parse(x)).ToArray();
                }
            }
            else
            {
                entity.UserRoleArray = new int[] { };
                entity.Shared = true;
                foreach (var item in viewColumns)
                {
                    var viewCol = new Eli_ViewColumns
                    {
                        ColumnName = item.ColumnName,
                        ViewId = entity.Id,
                        FieldId = item.FieldId,
                        SortOrder = i++,
                        LabelDisplay = item.LabelDisplay,
                        Visible = false,
                        Width = "100px",
                        AllowGroup = item.AllowGroup.HasValue && item.AllowGroup.Value
                    };
                    entity.Eli_ViewColumns.Add(viewCol);
                }
            }

            var moduleList = ModuleBM.Instance.GetAllModulesWithDefaultView(CurrentUserRole.Id);
            //.Where(r => r.Dashboard == false && r.Parent == 0);
            var viewList = id > 0 ? ViewBM.Instance.GetViewList(entity.ModuleId) : new List<Eli_Views>();
            entity.NameValues = new Hashtable {
            { "viewlist", viewList },
            { "modules", moduleList },
            { "viewColumns", viewColumns } };
            return entity;
        }

        [HttpPost]
        public virtual ResultObj SaveView([FromBody] Eli_Views model)
        {
            try
            {
                model = SetView(model, true);
                string msg = ValidateFillForm(model);

                if (string.IsNullOrEmpty(msg))
                {
                    int status = ViewBM.Instance.SaveView(model);
                    if (status > 0)
                        return new ResultObj(ResultCodes.Success,
                            GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), 0);
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
        [HttpPost]
        public ResultObj SavePersonalView([FromBody] Eli_Views model, [FromUri] bool isCreateNewView)
        {
            try
            {
                var entity = ViewBM.Instance.SingleLoadWithReferences(v => v.Id == model.Id, "Eli_ViewColumns", "Eli_ViewConditions", "Eli_ViewGroupBy", "Eli_ViewOrderBy");
                SetPersonalView(model, entity);
                var fieldSource = EntityFieldBM.Instance.GetAllFieldsByModule(model.ModuleId);
                if (isCreateNewView)
                {
                    // create new view
                    // merge codition
                    foreach (var viewConds in model.Eli_ViewConditions)
                    {
                        var col = fieldSource.Single(r => r.FieldName == viewConds.ColumnName);
                        if ((col.IsDate || col.IsDateTime || col.IsList || col.IsInteger || col.IsDecimal) && string.IsNullOrEmpty(viewConds.Operator))
                        {
                            viewConds.Operator = "=";
                        }
                        else if (col.IsMultiSelecttBox && string.IsNullOrEmpty(viewConds.Operator))
                        {
                            viewConds.Operator = "like";
                        }
                        else if (string.IsNullOrEmpty(viewConds.Operator))
                        {
                            viewConds.Operator = SearchOperators.StartsWith.ToString();
                        }
                        viewConds.FieldId = col.FieldId;
                        viewConds.ViewId = entity.Id;
                        SetAuditFields(viewConds, viewConds.Id);
                        entity.Eli_ViewConditions.Add(viewConds);
                    }
                    entity.Id = 0;
                    SetView(entity, true);
                    entity.MasterViewId = model.Id;
                    entity.Shared = false;
                    entity.ViewName = model.ViewName;
                    var msg = ValidateFilterView(entity);
                    if (string.IsNullOrEmpty(msg))
                    {
                        var status = ViewBM.Instance.SaveView(entity);
                        if (status > 0)
                            return new ResultObj(ResultCodes.Success,
                                GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), entity.Id);
                        return new ResultObj(ResultCodes.SavingFailed,
                            GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), entity.Id);
                    }
                    return new ResultObj(ResultCodes.ValidationError, msg, entity.Id);
                }
                else
                {
                    // update view
                    // merge codition

                    foreach (var viewConds in model.Eli_ViewConditions)
                    {
                        var col = fieldSource.Single(r => r.FieldName == viewConds.ColumnName);
                        if ((col.IsDate || col.IsDateTime || col.IsList || col.IsInteger || col.IsDecimal) && string.IsNullOrEmpty(viewConds.Operator))
                        {
                            viewConds.Operator = "=";
                        }
                        else if (col.IsMultiSelecttBox && string.IsNullOrEmpty(viewConds.Operator))
                        {
                            viewConds.Operator = "like";
                        }
                        else if (string.IsNullOrEmpty(viewConds.Operator))
                        {
                            viewConds.Operator = SearchOperators.StartsWith.ToString();
                        }
                        SetAuditFields(viewConds, viewConds.Id);
                        viewConds.ViewId = entity.Id;
                        viewConds.FieldId = col.FieldId;
                        entity.Eli_ViewConditions.Add(viewConds);
                    }
                    SetView(entity, false);
                    entity.Shared = false;
                    var msg = ValidateFilterView(entity);
                    if (string.IsNullOrEmpty(msg))
                    {
                        var status = ViewBM.Instance.SaveView(entity);
                        if (status > 0)
                            return new ResultObj(ResultCodes.Success,
                                GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), entity.Id);
                        return new ResultObj(ResultCodes.SavingFailed,
                            GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), entity.Id);
                    }
                    return new ResultObj(ResultCodes.ValidationError, msg, entity.Id);
                }

            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), model.Id);
            }
        }
        [HttpGet]
        public virtual IList<Eli_Views> GetViewByModuleId(int id)
        {
            return ViewBM.Instance.GetViewList(id);
        }

        [HttpGet]
        public IList<vwViewMenu> GetViewsWithTotal(int id)
        {
            return ViewBM.Instance.GetViewsWithTotal(id, CurrentUserRole.Id, CurrentUserID);
        }

        [HttpPost]
        public virtual ResultObj DeleteViews([FromBody] JArray jsonObject)
        {
            try
            {
                var ids = JsonConvert.DeserializeObject<IList<Eli_Views>>(jsonObject.ToString());

                if (ids.Count == 0)
                    return new ResultObj(ResultCodes.ValidationError,
                        GetText("REQUIRED_ONE_VIEW"), 0);

                string denyNames;
                string idArray = string.Join(",", ids.Select(r => r.Id).ToArray());
                int status = ViewBM.Instance.DeleteViews(idArray, out denyNames);
                string msg = string.Empty;
                string[] nameArray = denyNames.Split(',');
                if (nameArray.Length > 0 && nameArray[0] != string.Empty)
                {
                    msg = nameArray.Aggregate(msg,
                        (current, name) =>
                            current + string.Format(GetText("VIEW_IS_USING") + "<br>", name));
                }

                if (status > 0)
                {
                    msg += GetText("COMMON", "DELETE_SUCCESS");
                    return new ResultObj(ResultCodes.Success, msg, 0);
                }
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public virtual ResultObj ExportCsv([FromBody] JObject jsonObject)
        {
            try
            {
                var pageInfo = JsonConvert.DeserializeObject<PageInfo>(jsonObject.ToString());
                var key = pageInfo.Models["module"];
                var jArray = pageInfo.Models[key].ToString();
                var json = "{'" + key + "': " + jArray.Replace("\r\n", "") + "}";
                WriteCSV(pageInfo, json);
                return new ResultObj(ResultCodes.Success, string.Format("{0}temp/{1}.csv", ConfigValues.UPLOAD_DIRECTORY, pageInfo.ModuleName), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public virtual ResultObj ExportAllDataCsv([FromBody] PageInfo pageInfo)
        {
            try
            {
                Eli_TempViews entity = ViewBM.Instance.GetTempView(pageInfo.ViewId, CurrentUserID);
                pageInfo.PageSize = 999999999;
                pageInfo.PageIndex = 1;
                pageInfo.DefaultOrderBy = true;
                pageInfo = CheckPageInfo(pageInfo, entity);

                var key = pageInfo.Models["module"];
                var jArray = JsonConvert.SerializeObject(pageInfo.Models[key]);
                var json = "{'" + key + "': " + jArray.Replace("\r\n", "") + "}";
                WriteCSV(pageInfo, json);

                return new ResultObj(ResultCodes.Success, string.Format("{0}temp/{1}.csv", ConfigValues.UPLOAD_DIRECTORY, pageInfo.ModuleName), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public virtual PageInfo AdvanceSearch([FromBody] JArray jsonObject, [FromUri] int id, [FromUri] bool defaultOrder, [FromUri] int pageSize, [FromUri] int pageIndex, [FromUri] string groupColumn, [FromUri]string sortExpression)
        {
            try
            {
                var view = ViewBM.Instance.SingleLoadWithReferences(v => v.Id == id, "Eli_ViewColumns",
                                                                          "Eli_ViewConditions",
                                                                          "Eli_ViewOrderBy");
                //Update ColumnName to comapre groupby column
                var viewColumns = EntityFieldBM.Instance.GetViewColumnsByModule(view.ModuleId, true, CurrentUserRole.Id); //Full Columns From
                view.Eli_ViewColumns = view.Eli_ViewColumns.Select(v =>
                {
                    var column = viewColumns.SingleOrDefault(c => c.FieldId == v.FieldId);
                    if (column != null)
                        v.ColumnName = column.ColumnName;
                    return v;
                }).ToList();

                if (view == null || !HasPermisson(Permission.Read, view.ModuleId)) return new PageInfo();

                if (jsonObject.Count > 0)
                {
                    foreach (var item in jsonObject)
                    {
                        if (item["FilterValue"] is JArray) item["FilterValue"] = "\"" + item["FilterValue"] + "\"";
                        if (item["FilterValue1"] is JArray) item["FilterValue1"] = "\"" + item["FilterValue1"] + "\"";
                    }
                }
                var conditions = JsonConvert.DeserializeObject<IList<Eli_ViewConditions>>(jsonObject.ToString());
                if (conditions.Count > 0)
                {
                    var fieldSource = EntityFieldBM.Instance.GetAllFieldsByModule(view.ModuleId);

                    foreach (var condition in conditions)
                    {
                        var col = fieldSource.Single(r => r.FieldName == condition.ColumnName);
                        if ((col.IsDate || col.IsDateTime || col.IsList || col.IsInteger || col.IsDecimal || col.IsCurrency) &&
                            string.IsNullOrEmpty(condition.Operator))//don't specify the operator
                        {
                            if (col.IsList && col.IsTextShow == true) condition.Operator = "like";
                            else condition.Operator = "=";
                        }
                        else if (col.IsMultiSelecttBox && //the column which refer with the other data (include the picklist and table)
                                string.IsNullOrEmpty(condition.Operator))//don't specify the operator
                        {
                            condition.Operator = "like";
                        }
                        else if (string.IsNullOrEmpty(condition.Operator))//don't specify the operator
                        {
                            condition.Operator = SearchOperators.StartsWith.ToString();
                        }

                        condition.FieldId = col.FieldId;
                        view.Eli_ViewConditions.Add(condition);
                    }
                }

                return SetAdvanceSearch(view, pageSize, pageIndex, groupColumn, 0, true, sortExpression);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new PageInfo();
            }
        }

        [HttpPost]
        public virtual PageInfo ServerFilterData([FromBody] JArray jsonJObject, [FromUri] int viewId, [FromUri] int moduleId, [FromUri] int id, [FromUri] int pageSize, [FromUri] int pageIndex, [FromUri] string groupColumn, [FromUri]string sortExpression)
        {
            try
            {
                var view = ViewBM.Instance.SingleLoadWithReferences(v => v.Id == viewId, "Eli_ViewColumns", "Eli_ViewConditions",
                           "Eli_ViewOrderBy");
                if (view == null || !HasPermisson(Permission.Read, view.ModuleId)) return new PageInfo();
                var conditions = JsonConvert.DeserializeObject<IList<Eli_ViewConditions>>(jsonJObject.ToString());
                if (conditions.Count > 0)
                {
                    var fieldSource = EntityFieldBM.Instance.GetAllFieldsByModule(moduleId);
                    foreach (var condition in conditions)
                    {
                        var col = fieldSource.Single(r => r.FieldName == condition.ColumnName);
                        if ((col.IsDate || col.IsDateTime || col.IsList || col.IsInteger || col.IsDecimal || col.IsCurrency) && string.IsNullOrEmpty(condition.Operator))
                        {
                            condition.Operator = "=";
                        }
                        else if (col.IsMultiSelecttBox && string.IsNullOrEmpty(condition.Operator))
                        {
                            condition.Operator = "like";
                        }
                        else if (string.IsNullOrEmpty(condition.Operator))
                        {
                            condition.Operator = SearchOperators.StartsWith.ToString();
                        }
                        condition.FieldId = col.FieldId;
                        view.Eli_ViewConditions.Add(condition);
                    }
                }
                return SetAdvanceSearch(view, pageSize, pageIndex, groupColumn, id, true, sortExpression);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public ResultObj SaveFilter([FromBody] JObject jsonObject)
        {
            try
            {
                var view = JsonConvert.DeserializeObject<Eli_Views>(jsonObject.ToString());
                var viewEntity = ViewBM.Instance.SingleLoadWithReferences(v => v.Id == view.Id, "Eli_ViewColumns", "Eli_ViewOrderBy");
                if (viewEntity != null)
                {
                    view.Eli_ViewColumns = new Collection<Eli_ViewColumns>();
                    foreach (var viewColumn in viewEntity.Eli_ViewColumns)
                    {
                        var column = new Eli_ViewColumns
                        {
                            FieldId = viewColumn.FieldId,
                            SortOrder = viewColumn.SortOrder
                        };
                        view.Eli_ViewColumns.Add(column);

                    }
                    view.Eli_ViewOrderBy = new Collection<Eli_ViewOrderBy>();
                    foreach (var viewColumn in viewEntity.Eli_ViewOrderBy)
                    {
                        var column = new Eli_ViewOrderBy
                        {
                            FieldId = viewColumn.FieldId,
                            ColumnName = viewColumn.ColumnName,
                            OrderDirection = viewColumn.OrderDirection
                        };
                        view.Eli_ViewOrderBy.Add(column);
                    }
                    view.MasterViewId = view.Id;
                    view.Id = 0;
                    view.ParentId = string.Empty;
                    view = SetView(view, false);
                    view.Shared = false;
                    view.IsActive = true;
                    string msg = ValidateFilterView(view);
                    if (string.IsNullOrEmpty(msg))
                    {
                        int status = ViewBM.Instance.SaveView(view);
                        if (status > 0)
                            return new ResultObj(ResultCodes.Success,
                                GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), 0);
                        return new ResultObj(ResultCodes.SavingFailed,
                            GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
                    }
                    return new ResultObj(ResultCodes.ValidationError, msg, 0);
                }
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError,
                    GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public int ColumnsVisibleChanged([FromBody] IList<vwViewColumn> models, [FromUri]int viewId, [FromUri] int moduleId)
        {
            try
            {
                //var fieldIds = models.Where(r => r.Selected).Select(r => r.FieldId).ToList();
                HttpContext.Current.Session["ColumnsVisibleChanged"] = models;
                HttpContext.Current.Session["viewId"] = viewId;
                HttpContext.Current.Session["moduleId"] = moduleId;

                //update tempview
                //get main view to generate sql script
                var currentView = ViewBM.Instance.SingleLoadWithReferences(v => v.Id == viewId, "Eli_ViewColumns", "Eli_ViewConditions",
                    "Eli_ViewOrderBy", "Eli_ViewGroupBy");
                var tempView = ViewBM.Instance.GetTempView(viewId, CurrentUserID);

                if (currentView != null && tempView != null)
                {
                    var fieldSelectedIds = models.Select(v => v.FieldId);
                    //reset visible
                    currentView.Eli_ViewColumns = currentView.Eli_ViewColumns.Select(v =>
                    {
                        v.Visible = fieldSelectedIds.Contains(v.FieldId);
                        return v;
                    }).ToList();
                    tempView.QueryScript = ViewBuilder.Instance.CreateSqlView(currentView);
                    ViewBM.Instance.SaveTempView(tempView);
                }

                return models.Count;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return 0;
            }
        }

        [HttpGet]
        public int GetDefaultViewByRoleAndModule([FromUri]int mId)
        {
            try
            {
                var modules = ModuleBM.Instance.GetAllModulesWithDefaultView(CurrentUserRole.Id).ToList();
                if(modules != null && modules.Any())
                {
                    var module = modules.SingleOrDefault(x => x.Id == mId);
                    if(module != null)
                    {
                        return module.DefaultViewId.GetValueOrDefault(0);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return 0;
            }
        }

        private void WriteCSV(PageInfo pageInfo, string json)
        {
            XmlNode xml = JsonConvert.DeserializeXmlNode("{records:{record:" + json + "}}");

            var xmldoc = new XmlDocument();
            //Create XmlDoc Object
            xmldoc.LoadXml(xml.InnerXml);
            //Create XML Steam 
            var xmlReader = new XmlNodeReader(xmldoc);
            var dataSet = new DataSet();
            //Load Dataset with Xml
            dataSet.ReadXml(xmlReader);
            //return single table inside of dataset
            var csv = ToCSV(dataSet.Tables[1], ",", pageInfo.ViewColumns);
            var encoding = new UTF8Encoding();
            var physicalPath = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP);
            var path = string.Format(physicalPath + "\\{0}.csv", pageInfo.ModuleName);
            using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            {
                streamWriter.Write(csv);
            }
        }

        private string ToCSV(DataTable table, string delimator, IList<ColumnExport> columns)
        {

            var result = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].Display)
                {
                    result.Append(string.Format("\"{0}\"", columns[i].LabelDisplay));
                    result.Append(i == columns.Count - 1 ? "\n" : delimator);
                }
            }
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i].Display)
                    {
                        if (columns[i].IsDate)
                        {
                            try
                            {
                                result.Append(string.Format("\"{0}\"", DateUtility.GetFormartedDate(DateTime.Parse(row[columns[i].ColumnName].ToString()), SiteSettings.DATE_FORMAT, false, string.Empty, CurrentCulture)));
                            }
                            catch
                            {
                                result.Append(string.Empty);
                            }
                        }
                        else if (columns[i].IsDateTime)
                        {
                            try
                            {
                                result.Append(string.Format("\"{0}\"", DateUtility.GetFormartedDate(DateTime.Parse(row[columns[i].ColumnName].ToString()), SiteSettings.DATE_FORMAT, true, SiteSettings.TIME_FORMAT, CurrentCulture)));
                            }
                            catch
                            {
                                result.Append(string.Empty);
                            }
                        }
                        else if (columns[i].IsTextArea)
                        {
                            string res = row[columns[i].ColumnName].ToString().Replace("<br>", "\n");
                            result.Append(string.Format("\"{0}\"", res));

                        }
                        else if (columns[i].IsList && row[columns[i].ColumnName].ToString().IndexOf("@@") > 0)
                        {
                            var text = row[columns[i].ColumnName].ToString();
                            result.Append(string.Format("\"{0}\"", text.Substring(0, text.IndexOf("@@"))));
                        }
                        else
                        {
                            result.Append(string.Format("\"{0}\"", row[columns[i].ColumnName]));
                        }
                        result.Append(i == columns.Count - 1 ? "\n" : delimator);
                    }
                }
            }
            return result.ToString().TrimEnd(new[] { '\r', '\n' });
        }

        private void ProcessData(PageInfo model, int totalRow, IList<object> entities, string moduleName)
        {

            model.TotalRow = totalRow;
            if (totalRow > 0 && entities.Count == 0)
            {
                string groupResult;
                model.PageIndex -= 1;
                entities = ViewBM.Instance.GetView(out moduleName, model.ViewId, model.ModuleId, model.Id, CurrentUserID,
                    CurrentUserRole.Id,
                    model.PageIndex, model.PageSize, out totalRow,
                    model.SortExpression, model.DefaultOrderBy, model.GroupColumn, out groupResult);
                model.GroupResult = groupResult;
            }
            if (totalRow < model.PageSize)
            {
                model.PageIndex = 1;
            }
            model.ModuleName = moduleName;
            string key = string.Format("{0}{1}{2}", moduleName, model.ViewId, model.Id);
            var hashtable = new Hashtable { { "module", key }, { key, entities } };
            model.Models = hashtable;
        }

        private PageInfo CheckPageInfo(PageInfo model, Eli_TempViews entity, Eli_Views view = null)
        {
            int totalRow;
            string moduleName;
            string groupResult;
            IList<object> entities;
            if (model.AdvanceSearch)
            {
                string viewScript;
                if (entity != null)
                {
                    viewScript = entity.QueryScript;
                }
                else
                {
                    entity = ViewBM.Instance.GetTempView(model.ViewId, CurrentUserID);
                    viewScript = entity.QueryScript;
                }

                //get columns visible from tempview
                List<int> fieldIdsSelected = new List<int>();
                if (HttpContext.Current.Session["ColumnsVisibleChanged"] != null)
                {
                    var viewCols = HttpContext.Current.Session["ColumnsVisibleChanged"] as IList<vwViewColumn>;
                    if (viewCols != null)
                    {
                        fieldIdsSelected = viewCols.Where(r => r.Selected).Select(r => r.FieldId).ToList();
                    }
                }
                //get colums visiable from group columns
                if (!string.IsNullOrWhiteSpace(model.GroupColumn) && view != null)
                {
                    var fieldGroup = view.Eli_ViewColumns.SingleOrDefault(c => c.ColumnName == model.GroupColumn);
                    if (fieldGroup != null)
                        fieldIdsSelected.Add(fieldGroup.FieldId);
                }

                entities = ViewBM.Instance.AdvanceSearch(out moduleName, model.ViewId, model.ModuleId, model.Id, viewScript,
                    CurrentUserID, CurrentUserRole.Id, model.PageIndex, model.PageSize, out totalRow,
                    model.SortExpression, model.DefaultOrderBy, model.GroupColumn, out groupResult, fieldIdsSelected);
            }
            else
            {
                entities = ViewBM.Instance.GetView(out moduleName, model.ViewId, model.ModuleId, model.Id, CurrentUserID,
                    CurrentUserRole.Id, model.PageIndex, model.PageSize, out totalRow, model.SortExpression,
                    model.DefaultOrderBy, model.GroupColumn, out groupResult);
            }
            model.GroupResult = groupResult;
            ProcessData(model, totalRow, entities, moduleName);
            return model;
        }

        private Eli_Views SetView(Eli_Views entity, bool notUpdateCustomView)
        {
            entity.Eli_ViewColumns = entity.Eli_ViewColumns.OrderBy(r => r.SortOrder).ToList();
            var colIds = entity.Eli_ViewColumns.Select(r => r.FieldId).ToList();
            var viewColumns = EntityFieldBM.Instance.GetViewColumnsByModule(entity.ModuleId, false, CurrentUserRole.Id);
            var missingColumns = viewColumns.Where(r => !colIds.Contains(r.FieldId));
            var flag = false;
            if (entity.Id > 0)
            {
                var idCol = viewColumns.Single(r => r.ColumnName == "Id");
                if (entity.Eli_ViewColumns.Any(r => r.FieldId == idCol.FieldId) == false)
                {
                    flag = true;
                    entity.Eli_ViewColumns.Add(new Eli_ViewColumns
                    {
                        ViewId = entity.Id,
                        FieldId = idCol.FieldId,
                        Visible = false,
                        Width = "100px"
                    });
                }
            }
            else
            {
                foreach (var item in missingColumns)
                {
                    entity.Eli_ViewColumns.Add(new Eli_ViewColumns
                    {
                        ViewId = entity.Id,
                        FieldId = item.FieldId,
                        Visible = false,
                        Width = "100px"
                    });
                }
                entity.Shared = true;
                entity.IsPublic = true;
            }

            Eli_Views view =
                ViewBM.Instance.Find(r => r.ModuleId == entity.ModuleId && r.Id != entity.Id)
                    .OrderByDescending(r => r.SortOrder)
                    .FirstOrDefault();
            entity.SortOrder = view != null && entity.SortOrder > 0 ? entity.SortOrder : 1;
            entity.QueryScript = ViewBuilder.Instance.CreateSqlView(entity);
            if (flag)
            {
                // Remove Id Column
                entity.Eli_ViewColumns = entity.Eli_ViewColumns.Take(entity.Eli_ViewColumns.Count - 1).ToList();
            }
            SetAuditFields(entity, entity.Id);
            int i = 1;
            foreach (Eli_ViewColumns item in entity.Eli_ViewColumns)
            {
                item.Id = entity.Id > 0 ? item.Id : 0;
                item.SortOrder = i++;
                SetAuditFields(item, item.Id);
            }
            foreach (Eli_ViewConditions item in entity.Eli_ViewConditions)
            {
                item.Id = entity.Id > 0 ? item.Id : 0;
                SetAuditFields(item, item.Id);
            }
            var orderBy = entity.Eli_ViewOrderBy.Where(r => r.FieldId != 0).ToList();
            entity.Eli_ViewOrderBy = orderBy;
            foreach (Eli_ViewOrderBy item in entity.Eli_ViewOrderBy)
            {
                item.Id = entity.Id > 0 ? item.Id : 0;
                SetAuditFields(item, item.Id);
            }
            var groupBy = entity.Eli_ViewGroupBy.Where(r => r.FieldId != 0).ToList();
            entity.Eli_ViewGroupBy = groupBy;
            foreach (Eli_ViewGroupBy item in entity.Eli_ViewGroupBy)
            {
                item.Id = entity.Id > 0 ? item.Id : 0;
                item.ColumnName = viewColumns.Single(r => r.FieldId == item.FieldId).ColumnName;
                SetAuditFields(item, item.Id);
            }


            entity.UserRole = entity.UserRoleArray != null && entity.UserRoleArray.Any() ? string.Join<int>(",", entity.UserRoleArray) : "";
            
               

            return entity;
        }

        private string ValidateFillForm(Eli_Views entity)
        {
            string msg = new ObjectValidator(entity.CurrentModule).ValidateObject(entity);

            //Check Name
            bool result;
            if (entity.Shared)
            {
                // Create New/Edit Public View
                result = entity.Id > 0
                ? ViewBM.Instance.Count(r => r.ViewName.Equals(entity.ViewName) && r.Id != entity.Id && r.Shared) > 0
                : ViewBM.Instance.Count(r => r.ViewName.Equals(entity.ViewName) && r.Shared) > 0;
            }
            else
            {
                // Edit Personal View
                result = entity.Id > 0
                ? ViewBM.Instance.Count(r => r.ViewName.Equals(entity.ViewName) && r.Id != entity.Id
                                             && r.CreatedBy == CurrentUserID && r.Shared == false
                                             && r.ModuleId == entity.ModuleId) > 0
                : ViewBM.Instance.Count(r => r.ViewName.Equals(entity.ViewName) && r.CreatedBy == CurrentUserID
                                             && r.Shared == false && r.ModuleId == entity.ModuleId) > 0;
            }

            if (result)
            {
                msg += GetText("EXIST_NAME") + "<br/>";
            }

            if (entity.Parent != null && entity.Parent.Length > 0)
            {
                IList<string> nameList;
                result = ViewBM.Instance.CheckSubviewBelongParent(entity, out nameList);
                if (nameList.Count > 0 && result)
                {
                    msg = nameList.Aggregate(msg, (current, name) => current + (string.Format(GetText("ONLY_ONE_CHILD_VIEW"), name) + "<br/>"));
                }
            }
            return msg;
        }

        private string ValidateFilterView(Eli_Views entity)
        {
            string msg = new ObjectValidator(entity.CurrentModule).ValidateObject(entity);
            //Check Name
            var result = entity.Id > 0
                ? ViewBM.Instance.Count(r => r.ViewName.Equals(entity.ViewName) && r.Id != entity.Id
                                             && r.CreatedBy == CurrentUserID && r.Shared == false
                                             && r.ModuleId == entity.ModuleId) > 0
                : ViewBM.Instance.Count(r => r.ViewName.Equals(entity.ViewName) && r.CreatedBy == CurrentUserID
                                             && r.Shared == false && r.ModuleId == entity.ModuleId) > 0;
            if (result)
            {
                msg += GetText("EXIST_NAME");
            }
            return msg;
        }

        private PageInfo SetAdvanceSearch(Eli_Views view, int pageSize, int pageIndex, string groupColumn, int id, bool advanceSearch, string sortExpression)
        {
            //update visibility of view if session that have any fields selected are exist
            var fieldIdsSelected = GetFieldIdsTempView();

            view.Eli_ViewColumns = view.Eli_ViewColumns.Select(v =>
            {
                v.Visible = (fieldIdsSelected != null ? fieldIdsSelected.Contains(v.FieldId) : v.Visible) ||
                            (!string.IsNullOrWhiteSpace(groupColumn) && !string.IsNullOrWhiteSpace(v.ColumnName)
                                ? ("," + groupColumn + ",").Contains(v.ColumnName)
                                : v.Visible);
                return v;
            }).ToList();




            var tempView = ViewBM.Instance.GetTempView(view.Id, CurrentUserID);
            if (tempView == null)
            {
                //Insert TempView
                tempView = new Eli_TempViews
                {
                    ViewName = view.ViewName,
                    IsPublic = view.IsPublic,
                    SortOrder = view.SortOrder,
                    ModuleId = view.ModuleId,
                    QueryScript = ViewBuilder.Instance.CreateSqlView(view),
                    ParentId = view.ParentId,
                    LoadChildView = view.LoadChildView,
                    DefaultView = view.DefaultView,
                    IsActive = view.IsActive,
                    ViewParentId = view.Id
                };
                SetAuditFields(tempView, tempView.Id);
                ViewBM.Instance.SaveTempView(tempView);
            }
            else
            {
                if (NeedToUpdateScript(view.Eli_ViewConditions, groupColumn))
                {
                    //Update TempView
                    tempView.QueryScript = ViewBuilder.Instance.CreateSqlView(view);
                    SetAuditFields(tempView, tempView.Id);
                    ViewBM.Instance.SaveTempView(tempView);
                }
            }

            var model = new PageInfo
            {
                Id = id,
                ViewId = view.Id,
                ModuleId = view.ModuleId,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleId = CurrentUserRole.Id,
                SortExpression = string.IsNullOrEmpty(sortExpression) ? "Id Asc" : sortExpression,
                AdvanceSearch = advanceSearch,
                DefaultOrderBy = string.IsNullOrEmpty(sortExpression),
                GroupColumn = groupColumn ?? string.Empty
            };
            return CheckPageInfo(model, tempView, view);
        }

        private int[] ConvertStringtoArray(string str)
        {
            string res = str.Replace("{", "").Replace("}", ",");
            res = res.Substring(0, res.Length - 1);
            return res.Split(',').Select(int.Parse).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">From Client</param>
        /// <param name="entity">From Database</param>
        private void SetPersonalView(Eli_Views model, Eli_Views entity)
        {

            // Update View Column
            foreach (var viewColumn in entity.Eli_ViewColumns)
            {
                viewColumn.SortOrder = 0;
                var col = model.Eli_ViewColumns.SingleOrDefault(r => r.FieldId == viewColumn.FieldId);

                if (col != null)
                {
                    viewColumn.Width = col.Width;
                    viewColumn.SortOrder = col.SortOrder;
                    SetAuditFields(viewColumn, viewColumn.Id);
                    viewColumn.Visible = true;
                }
                else
                {
                    viewColumn.Visible = false;
                }
            }

            // insert conditions
            if (entity.Eli_ViewConditions.Count > 0)
            {
                var newcondions =
                    model.Eli_ViewConditions.Where(r => !entity.Eli_ViewConditions.Any(cond => r.FieldId == cond.FieldId
                                                                                               && r.Operator == cond.Operator
                                                                                               && r.IsAND == cond.IsAND
                                                                                               &&
                                                                                               r.FilterValue == cond.FilterValue
                                                                                               &&
                                                                                               r.FilterValue1 ==
                                                                                               cond.FilterValue1));
                foreach (var item in newcondions)
                {
                    entity.Eli_ViewConditions.Add(item);
                }
            }


            // Update Order By
            if (model.Eli_ViewOrderBy.Count > 0)
            {
                var columnOrder = model.Eli_ViewOrderBy.Single();
                // exist group by
                if (entity.Eli_ViewOrderBy.Count > 0)
                {
                    foreach (var item in entity.Eli_ViewOrderBy)
                    {
                        item.ColumnName = columnOrder.ColumnName;
                        item.FieldId = columnOrder.FieldId;
                        item.OrderDirection = columnOrder.OrderDirection;
                    }
                }
                else
                {
                    entity.Eli_ViewOrderBy.Add(columnOrder);
                }
            }


            // Update Group By
            if (model.Eli_ViewGroupBy.Count > 0)
            {
                var columnGroup = model.Eli_ViewGroupBy.Single();
                // exist group by
                if (entity.Eli_ViewGroupBy.Count > 0)
                {
                    foreach (var item in entity.Eli_ViewGroupBy)
                    {
                        item.ColumnName = columnGroup.ColumnName;
                        item.FieldId = columnGroup.FieldId;
                    }
                }
                else
                {
                    entity.Eli_ViewGroupBy.Add(columnGroup);
                }
            }

            entity.SortOrder += 1;
            entity.ParentId = string.Empty;
            entity.IsActive = true;
            entity.DefaultView = false;
            entity.PageSize = model.PageSize;
        }

        private bool NeedToUpdateScript(ICollection<Eli_ViewConditions> conditions, string groupName)
        {
            var lastConditions = HttpContext.Current.Session["LastConditions"] as List<Eli_ViewConditions>;
            if (lastConditions == null && conditions.Count == 0)
                return true;
            if ((lastConditions == null && conditions.Count > 0) || (conditions.Count != lastConditions.Count))
            {
                HttpContext.Current.Session["LastConditions"] = conditions.ToList();
                return true;
            }
            var comparer = EqualityComparer<Eli_ViewConditions>.Default;
            var cons = conditions.ToList();
            if (cons.Where((t, i) => !comparer.Equals(t, lastConditions[i])).Any())
            {
                HttpContext.Current.Session["LastConditions"] = cons;
                return true;
            }


            var lastGroupName = HttpContext.Current.Session["GroupName"] as string;
            if (!string.IsNullOrWhiteSpace(groupName) || !string.IsNullOrWhiteSpace(lastGroupName))
            {
                HttpContext.Current.Session["GroupName"] = groupName;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the field ids that seleteced from selected visibility.
        /// This list stored on session
        /// </summary>
        /// <returns></returns>
        private List<int> GetFieldIdsTempView()
        {
            //merge session tempview and current view
            //get columns visible from tempview
            List<int> fieldIdsSelected = null;
            if (HttpContext.Current.Session["ColumnsVisibleChanged"] != null)
            {
                var viewCols = HttpContext.Current.Session["ColumnsVisibleChanged"] as IList<vwViewColumn>;
                if (viewCols != null)
                {
                    fieldIdsSelected = viewCols.Where(r => r.Selected).Select(r => r.FieldId).ToList();
                }
            }

            return fieldIdsSelected;
        }
    }
}