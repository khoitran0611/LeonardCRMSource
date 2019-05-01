using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using Eli.Common;
using Elinext.DataLib;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.ObjectHelper;

namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewCustomDA : EF5RepositoryBase<LeonardUSAEntities, Eli_ViewCustom>
    {
        private static volatile ViewCustomDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewCustomDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ViewCustomDA();
                    }
                }

                return _instance;
            }
        }

        public ViewCustomDA()
            : base(Settings.ConnectionString)
        {
        }

        public List<Eli_ViewCustom> GetCustomViews(string customView, int currentUserId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var res = context.Eli_ViewCustom.Where(p => p.Code == customView
                                                            && (p.DefaultView || p.CreatedBy == currentUserId)).ToList();
                foreach (var view in res)
                {
                    view.SelectClause = view.WhereClause = view.FromClause = view.DefaultFilterStr = "";
                    var viewId = view.Id;
                    view.ColumnsDisplay = context.vwCustomViewColumns.Where(x => x.ViewCustomId == viewId)
                        .Select(p => new ColumnDisplay
                        {
                            Id = p.Id,
                            Sortable = p.Sortable ?? false,
                            SortOrder = p.SortOrder,
                            AllowGroup = p.AllowGroup ?? false,
                            Visible = p.Visible,
                            ColumnName = p.ColumnAlias,
                            ColumnHeader = p.DisplayText,
                            DataTypeName = p.TypeName
                        })
                        .ToList();
                }
                return res;
            }
        }

        public IList<object> GetViewCustom(int viewId, int pageIndex, int pageSize, out int totalRow
            , IList<FilterObj> filerArray
            , string sortExpression
            , string groupName
            , out string groupJson
            , int userId = 0
            , int roleId = 0)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                //Convert filer array to filer string 
                var filterStr = GetFilterString(filerArray);

                totalRow = 0;
                groupJson = "";

                //Check Has View
                var countView = context.Eli_ViewCustom.Any(r => r.Id == viewId);
                if (countView)
                {
                    TypeBuilder builder = DynamicObjHelper.CreateTypeBuilder("MyCustomDynamicAssembly", "MyCustomView",
                        "MyCustomType");

                    var columns =
                        context.vwCustomViewColumns.AsNoTracking()
                            .Where(records => records.ViewCustomId == viewId)
                            .ToList();
                    builder = columns.Aggregate(builder, DynamicObjHelper.DefineObject);

                    DynamicObjHelper.CreateAutoImplementedProperty(builder, "Selected", typeof(bool));

                    var resultType = builder.CreateType();
                    var viewParam = new SqlParameter
                    {
                        ParameterName = "ViewId",
                        Value = viewId
                    };
                    var userIdParam = new SqlParameter
                    {
                        ParameterName = "userId",
                        Value = userId
                    };
                    var roleIdParam = new SqlParameter
                    {
                        ParameterName = "roleId",
                        Value = roleId
                    };
                    var pageIndexParam = new SqlParameter
                    {
                        ParameterName = "pageIndex",
                        Value = pageIndex
                    };
                    var pageSizeParam = new SqlParameter
                    {
                        ParameterName = "pageSize",
                        Value = pageSize
                    };
                    var sortParam = new SqlParameter
                    {
                        ParameterName = "sortExpression",
                        Value = sortExpression
                    };

                    var filterParam = new SqlParameter
                    {
                        ParameterName = "filterStr",
                        Value = filterStr
                    };

                    var totalRowParam = new SqlParameter
                    {
                        ParameterName = "totalRow",
                        Value = totalRow,
                        Direction = ParameterDirection.Output
                    };

                    var groupParam = new SqlParameter
                    {
                        ParameterName = "columnGroup",
                        Value = groupName
                    };

                    var groupJsonRowParam = new SqlParameter
                    {
                        ParameterName = "groupJsonStr",
                        Value = groupJson,
                        Direction = ParameterDirection.Output,
                        Size = 4000
                    };

                    var queryResult = context.Database.SqlQuery(
                        resultType,
                        "sp_GetViewCustom @ViewId, @userId, @roleId, @pageIndex, @pageSize, @sortExpression, @filterStr"
                        + ", @totalRow out, @columnGroup, @groupJsonStr out"
                        , viewParam, userIdParam, roleIdParam, pageIndexParam, pageSizeParam, sortParam, filterParam
                        , totalRowParam, groupParam, groupJsonRowParam);

                    var entities = queryResult.Cast<object>().ToList();
                    totalRow = (int)totalRowParam.Value;
                    groupJson = !string.IsNullOrEmpty(groupName) && entities.Count > 0
                        ? groupJsonRowParam.Value.ToString()
                        : "[]";

                    return entities;
                }
                return new List<object>();
            }
        }

        public bool CreateCustomView(ViewCustomCreatedModel viewCustom, int currentUserId)
        {
            var newViewCustom = new Eli_ViewCustom
            {
                Code = viewCustom.ViewCode,
                ViewName = viewCustom.ViewName,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedBy = currentUserId,
                ModifiedBy = currentUserId
            };

            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var view = SingleLoadWithReferences(p => p.Id == viewCustom.ViewIdCurrent,
                    "Eli_ViewCustomColumns", "Eli_ViewCustomConditions");

                if (view != null)
                {
                    newViewCustom.SelectClause = view.SelectClause;
                    newViewCustom.FromClause = view.FromClause;
                    newViewCustom.DefaultFilterStr = view.DefaultFilterStr;
                    newViewCustom.WhereClause = view.WhereClause;
                    newViewCustom.MasterViewId = view.Id;
                    newViewCustom.IsPublic = false;
                    newViewCustom.OrderBy = string.IsNullOrWhiteSpace(viewCustom.Condition.OrderBy) ? view.OrderBy : viewCustom.Condition.OrderBy;
                    newViewCustom.GroupBy = string.IsNullOrWhiteSpace(viewCustom.Condition.GroupBy) ? view.GroupBy : viewCustom.Condition.GroupBy;
                    newViewCustom.PageSize = viewCustom.Condition.PageSize == 0 ? view.PageSize : viewCustom.Condition.PageSize;

                    context.Eli_ViewCustom.Add(newViewCustom);

                    //New view added
                    if (context.SaveChanges() > 0)
                    {
                        newViewCustom.Eli_ViewCustomColumns = new List<Eli_ViewCustomColumns>();

                        if (view.Eli_ViewCustomColumns != null)
                        {
                            //Copy columms from parent view
                            foreach (var viewCustomColumns in view.Eli_ViewCustomColumns)
                            {
                                viewCustomColumns.ViewCustomId = newViewCustom.Id;
                                viewCustomColumns.Eli_ViewCustom = null;

                                newViewCustom.Eli_ViewCustomColumns.Add(viewCustomColumns);
                            }

                            //Copy condition from parent view
                            foreach (var parentCondition in view.Eli_ViewCustomConditions)
                            {
                                var viewCustomConditions = new Eli_ViewCustomConditions
                                {
                                    ViewId = newViewCustom.Id,
                                    ColumnName = parentCondition.ColumnName,
                                    Operator = parentCondition.Operator,
                                    FilterValue = parentCondition.FilterValue,
                                    FieldId = parentCondition.FieldId,
                                    IsAND = parentCondition.IsAND,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CreatedBy = currentUserId,
                                    ModifiedBy = currentUserId
                                };

                                newViewCustom.Eli_ViewCustomConditions.Add(viewCustomConditions);
                            }

                            if (viewCustom.Condition.ListFilter != null
                                && viewCustom.Condition.ListFilter.Any())
                            {
                                //Generate condition from filter values 
                                foreach (var filter in viewCustom.Condition.ListFilter)
                                {
                                    var customViewColumn =
                                        context.vwCustomViewColumns.FirstOrDefault(p => p.ColumnAlias == filter.Key);

                                    var viewCustomConditions = new Eli_ViewCustomConditions
                                    {
                                        ViewId = newViewCustom.Id,
                                        ColumnName = filter.Key,
                                        Operator = filter.Operator,
                                        FilterValue = filter.Value,
                                        FieldId = customViewColumn != null ? customViewColumn.FieldId : 0,
                                        IsAND = true,
                                        CreatedDate = DateTime.Now,
                                        ModifiedDate = DateTime.Now,
                                        CreatedBy = currentUserId,
                                        ModifiedBy = currentUserId
                                    };

                                    newViewCustom.Eli_ViewCustomConditions.Add(viewCustomConditions);
                                }

                                var operatorInWhere = !string.IsNullOrWhiteSpace(newViewCustom.WhereClause)
                                    ? " AND "
                                    : "";

                                newViewCustom.WhereClause += operatorInWhere + GetFilterString(viewCustom.Condition.ListFilter);
                            }

                            context.Eli_ViewCustom.Attach(newViewCustom);
                            context.Entry(newViewCustom).State = System.Data.Entity.EntityState.Modified;
                            return context.SaveChanges() > 0;
                        }
                    }
                }
                return false;
            }
        }

        public bool SaveCustomView(ViewCustomUpdatedModel viewUpdated, int currentUserId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (viewUpdated.Condition.ListFilter != null
                    && viewUpdated.Condition.ListFilter.Any())
                {
                    foreach (var filter in viewUpdated.Condition.ListFilter)
                    {
                        var customViewColumn =
                            context.vwCustomViewColumns.FirstOrDefault(p => p.ColumnAlias == filter.Key);

                        var viewCustomConditions = new Eli_ViewCustomConditions
                        {
                            ViewId = viewUpdated.ViewCustom.Id,
                            ColumnName = filter.Key,
                            Operator = filter.Operator,
                            FilterValue = filter.Value,
                            FieldId = customViewColumn != null ? customViewColumn.FieldId : 0,
                            IsAND = true,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            CreatedBy = currentUserId,
                            ModifiedBy = currentUserId
                        };

                        context.Eli_ViewCustomConditions.Add(viewCustomConditions);
                    }

                    var operatorInWhere = !string.IsNullOrWhiteSpace(viewUpdated.ViewCustom.WhereClause)
                        ? " AND "
                        : "";
                    viewUpdated.ViewCustom.WhereClause += operatorInWhere + GetFilterString(viewUpdated.Condition.ListFilter);
                }

                viewUpdated.ViewCustom.OrderBy = string.IsNullOrWhiteSpace(viewUpdated.Condition.OrderBy)
                    ? viewUpdated.ViewCustom.OrderBy
                    : viewUpdated.Condition.OrderBy;

                viewUpdated.ViewCustom.GroupBy = string.IsNullOrWhiteSpace(viewUpdated.Condition.GroupBy)
                   ? viewUpdated.ViewCustom.GroupBy
                   : viewUpdated.Condition.GroupBy;

                viewUpdated.ViewCustom.PageSize = viewUpdated.Condition.PageSize == 0
                    ? viewUpdated.ViewCustom.PageSize
                    : viewUpdated.Condition.PageSize;

                context.Eli_ViewCustom.Attach(viewUpdated.ViewCustom);
                context.Entry(viewUpdated.ViewCustom).State = System.Data.Entity.EntityState.Modified;
                return context.SaveChanges() > 0;
            }
        }

        public bool EditCustomView(Eli_ViewCustom view, int currentUserId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var currnetView = context.Eli_ViewCustom.Include("Eli_ViewCustomColumns")
                                                        .Include("Eli_ViewCustomConditions").Single(v => v.Id == view.Id);

                //update view name
                currnetView.ViewName = view.ViewName;

                //update order
                currnetView.OrderBy = view.OrderBy;

                //update group by
                currnetView.GroupBy = view.GroupBy;

                //remove all old conditions
                var conditions = currnetView.Eli_ViewCustomConditions.ToList();
                foreach (var condition in conditions)
                {                    
                    context.Entry(condition).State = System.Data.Entity.EntityState.Deleted;
                }                

                //updating columns
                var columns = currnetView.Eli_ViewCustomColumns.ToList();
                foreach (var column in columns)
                {
                    var tempCol = view.Columns.SingleOrDefault(x => x.Id == column.Id);
                    if (tempCol != null)
                    {
                        column.SortOrder = view.Columns.IndexOf(tempCol);
                        column.Visible = tempCol.Visible;
                        column.DefaultWidth = tempCol.DefaultWidth;
                        context.Entry(column).State = System.Data.Entity.EntityState.Modified;
                    }                  
                }

                //adding conditions                
                if (view.Eli_ViewCustomConditions != null && view.Eli_ViewCustomConditions.Any())
                {
                    foreach (var filter in view.Eli_ViewCustomConditions)
                    {
                        var viewCustomConditions = new Eli_ViewCustomConditions
                        {
                            ViewId = view.Id,
                            ColumnName = filter.ColumnName,
                            Operator = filter.Operator,
                            FilterValue = filter.FilterValue,
                            FilterValue1 = filter.FilterValue1,
                            FieldId = filter.FieldId,
                            IsAND = filter.IsAND,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            CreatedBy = currentUserId,
                            ModifiedBy = currentUserId
                        };
                        currnetView.Eli_ViewCustomConditions.Add(viewCustomConditions);
                    }

                    //create where clause
                    currnetView.WhereClause = GetFullFilterString(view.Eli_ViewCustomConditions.ToArray(), context.vwCustomViewColumns.Where(v => v.ViewCustomId == view.Id).ToList());
                }
                else
                {
                    currnetView.WhereClause = string.Empty;
                }

                //update current view
                currnetView.ModifiedBy = currentUserId;
                currnetView.ModifiedDate = DateTime.Now;
                return context.SaveChanges() > 0;
            }
        }

        private string GetFilterString(IEnumerable<FilterObj> listFilter)
        {
            var filterArray = new List<string>();

            foreach (var filter in listFilter)
            {
                switch (filter.Operator)
                {
                    case "=":
                        {
                            if(filter.DataType == "Number")
                                filterArray.Add(string.Format("{0} {1} {2}", filter.Key, filter.Operator, filter.Value));
                            else if(filter.DataType == "Date")
                                filterArray.Add(string.Format("convert(date,{0}) {1} convert(date,'{2}')", filter.Key, filter.Operator, filter.Value));
                        } break;

                    case "Like":
                        {
                            filterArray.Add(string.Format("{0} {1} N'%{2}%'", filter.Key, filter.Operator, filter.Value));
                        } break;
                    case "StartsWith":
                        {
                            filterArray.Add(string.Format("{0} Like N'{1}%'", filter.Key, filter.Value));
                        } break;
                }
            }

            return string.Join(" AND ", filterArray);
        }

        private string GetFullFilterString(Eli_ViewCustomConditions[] listFilter, List<vwCustomViewColumn> columns)
        {
            var andConditions = GetFilter(listFilter.Where(c => c.IsAND), columns).ToArray();
            var orConditions = GetFilter(listFilter.Where(c => !c.IsAND), columns).ToArray();
            var a = andConditions.Length > 0 ? ("(" + string.Join(" AND ", andConditions) + ")") : "";
            var b = orConditions.Length > 0 ? ("(" + string.Join(" OR ", orConditions) + ")") : "";
            return a + (!string.IsNullOrWhiteSpace(b) ? (" OR " + b) : "");
        }

        private IEnumerable<string> GetFilter(IEnumerable<Eli_ViewCustomConditions> conditions, List<vwCustomViewColumn> columns)
        {
            var filterArray = new List<string>();
            foreach (var column in conditions)
            {
                var operatorStr = column.Operator;
                var filterValue = column.FilterValue;
                var filterValue1 = column.FilterValue1;
                var currentColumn = columns.Single(c => c.ColumnAlias == column.ColumnName);
                string value;
                if (operatorStr == SearchOperators.Within.ToString())
                {
                    var days = int.Parse(filterValue);
                    value = string.Format(days < 0
                        ? "  ({0}   between convert(date,DATEADD(DAY, {1}, GETDATE())) AND convert(date,getdate()))  "
                        : "  ({0}  between convert(date,getdate()) AND convert(date,DATEADD(DAY, {1}, GETDATE())))  "
                        , column.ColumnName, days);
                }
                else if (operatorStr == SearchOperators.Between.ToString())
                {
                    if (currentColumn.DataType == (int)DataTypes.Date)
                    {
                        value = string.Format("  ({0} {1} convert(date,'{2}') AND convert(date,'{3}'))  ", column.ColumnName, operatorStr, filterValue, filterValue1);
                    }
                    else
                    {
                        value = string.Format("  ({0}  {1} {2} AND {3})", column.ColumnName, operatorStr, filterValue, filterValue1);
                    }
                }
                else if (currentColumn.DataType == (int)DataTypes.Decimal || currentColumn.DataType == (int)DataTypes.Integer || currentColumn.DataType == (int)DataTypes.List || currentColumn.DataType == (int)DataTypes.Currency)
                {
                    if (currentColumn.DataType == (int)DataTypes.List)
                    {
                        if (operatorStr.ToLower().Equals("="))
                        {
                            value = string.Format("  ({0} LIKE N'{1}%') ", column.ColumnName, filterValue);
                        }
                        else if (operatorStr.ToLower().Equals("<>"))
                        {
                            value = string.Format("  ({0} NOT LIKE N'{1}%') ", column.ColumnName, filterValue);
                        }
                        else//other type
                        {
                            value = string.Format(" ({0} LIKE N'%{1}%') ", column.ColumnName, filterValue);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (currentColumn.DataType == (int)DataTypes.Integer)
                            {
                                var i = int.Parse(filterValue);
                            }
                            else
                            {
                                var i = decimal.Parse(filterValue);
                            }
                        }
                        catch
                        {
                            filterValue = "0";
                        }
                        value = string.Format("  ({0}  {1} {2})", column.ColumnName, operatorStr, filterValue);
                    }
                }
                else
                {
                    if (currentColumn.DataType == (int)DataTypes.CheckBox)
                    {
                        value = string.Format(" ({0}   {1} {2})", column.ColumnName, operatorStr, bool.Parse(filterValue) ? "1" : "0");
                    }
                    else if ((currentColumn.DataType == (int)DataTypes.Date) && !operatorStr.ToLower().Equals("like"))
                    {
                        value = string.Format("  (convert(date,{0})  {1} convert(date,'{2}'))  ", column.ColumnName, operatorStr, filterValue);
                    }
                    else if (operatorStr == SearchOperators.StartsWith.ToString())
                    {
                        value = string.Format("  ({0} like N'{1}%')  ", column.ColumnName, filterValue);
                    }
                    else 
                        value = operatorStr.ToLower().Equals("like") ? string.Format("  ({0}  {1} N'%{2}%')  ", column.ColumnName, operatorStr, filterValue) : string.Format("  ({0} {1} '{2}')  ", column.ColumnName, operatorStr, filterValue);
                }
                filterArray.Add(value);
            }
            return filterArray;
        }

        public Eli_ViewCustom GetViewObjById(int viewId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                //get view
                var res = context.Eli_ViewCustom.Include("Eli_ViewCustomConditions").SingleOrDefault(x => x.Id == viewId);

                //get the related columns
                res.Columns = context.vwCustomViewColumns.Where(x => x.ViewCustomId == viewId && (x.IsDefault == null || x.IsDefault.Value == false)).OrderBy(x => x.SortOrder).ToList();

                return res;
            }
        }

        //delete the view by Id
        public int DeleteById(int viewId, int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                //get view
                var view = context.Eli_ViewCustom.SingleOrDefault(x => x.Id == viewId && !x.DefaultView);

                if (view != null)
                {
                    //not belong to this user
                    if (view.CreatedBy != userId) return -1;

                    context.Entry(view).State = System.Data.Entity.EntityState.Deleted;
                }

                return context.SaveChanges();
            }
        }
    }
}
