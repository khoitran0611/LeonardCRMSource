using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.ObjectHelper;
using Elinext.DataLib;
using System.Data;

namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewDA : EF5RepositoryBase<LeonardUSAEntities, Eli_Views>
    {
        private static volatile ViewDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ViewDA();
                    }
                }

                return _instance;
            }
        }
        private ViewDA() : base(Settings.ConnectionString) { }

        public IList<object> GetView(out string moduleName, int viewId, int moduleId, int id, int userId, int roleId, int pageIndex, int pageSize, out int totalRow, string sortExpression, bool defaultOderBy, string groupColumn, out string groupResult)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                totalRow = 0;
                moduleName = groupResult = string.Empty;

                //Check Has View
                int countView = _context.Eli_Views.Count(r => r.Id == viewId && r.ModuleId == moduleId);
                if (countView > 0)
                {
                    moduleName = _context.Eli_Modules.AsNoTracking().Single(r => r.Id == moduleId).Name;
                    TypeBuilder builder = DynamicObjHelper.CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

                    var columns = _context.vwFieldNameDataTypes.AsNoTracking()
                                          .Where(records => records.ViewId == viewId)
                                          .ToList();
                    builder = columns.Aggregate(builder, DynamicObjHelper.DefineObject);

                    DynamicObjHelper.CreateAutoImplementedProperty(builder, "Selected", typeof(bool));

                    var resultType = builder.CreateType();
                    var moduleParam = new SqlParameter
                    {
                        ParameterName = "ModuleId",
                        Value = moduleId
                    };
                    var viewParam = new SqlParameter
                    {
                        ParameterName = "ViewId",
                        Value = viewId
                    };
                    var idParam = new SqlParameter
                    {
                        ParameterName = "Id",
                        Value = id
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
                        ParameterName = "sortDirection",
                        Value = sortExpression
                    };
                    var totalRowParam = new SqlParameter
                    {
                        ParameterName = "totalRow",
                        Value = totalRow,
                        Direction = ParameterDirection.Output
                    };
                    var oderByParam = new SqlParameter
                    {
                        ParameterName = "defaultOderBy",
                        Value = defaultOderBy

                    };
                    var groupColumnParam = new SqlParameter
                    {
                        ParameterName = "columnGroup",
                        Value = !string.IsNullOrEmpty(groupColumn) && id == 0 ? groupColumn : string.Empty
                    };
                    var groupResultParam = new SqlParameter
                    {
                        ParameterName = "groupJsonStr",
                        Value = groupResult,
                        Direction = ParameterDirection.Output,
                        Size = 4000
                    };
                    var queryResult = _context.Database.SqlQuery(
                        resultType, "sp_GetView @ModuleId, @ViewId, @Id, @userId, @roleId, @pageIndex, @pageSize, @sortDirection, @totalRow out,@defaultOderBy,@columnGroup,@groupJsonStr out", moduleParam, viewParam, idParam, userIdParam, roleIdParam, pageIndexParam, pageSizeParam, sortParam, totalRowParam, oderByParam, groupColumnParam, groupResultParam);

                    var entities = queryResult.Cast<object>().ToList();

                    totalRow = (int)totalRowParam.Value;
                    groupResult = !string.IsNullOrEmpty(groupColumn) && entities.Count > 0
                        ? groupResultParam.Value.ToString()
                        : "[]";
                    return entities;
                }
                return new List<object>();
            }
        }

        public IList<object> AdvanceSearch(out string moduleName, int viewId, int moduleId, int id, string script, int userId, int roleId, int pageIndex, int pageSize, out int totalRow, string sortExpression, bool defaultOderBy, string groupColumn, out string groupResult, List<int> fieldIdsSelected = null)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                totalRow = 0;
                moduleName = groupResult = string.Empty;
                //Check Has View
                int countView = context.Eli_Views.Count(r => r.Id == viewId && r.ModuleId == moduleId);
                if (countView > 0)
                {
                    moduleName = context.Eli_Modules.AsNoTracking().Single(r => r.Id == moduleId).Name;
                    TypeBuilder builder = DynamicObjHelper.CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

                    var columns =
                        context.vwFieldNameDataTypes.AsNoTracking()
                            .Where(
                                records =>
                                    records.ViewId == viewId)
                            .Distinct()
                            .ToList();

                    if (!fieldIdsSelected.IsNullOrEmpty())
                    {
                        columns =
                            columns.Where(
                                c =>
                                    c.Visible ||
                                    fieldIdsSelected.Contains(c.FieldId) ||
                                    Constant.RequireFields.Contains(c.ColumnName.ToLower())).ToList();
                    }
                    else
                    {
                        columns =
                            columns.Where(
                                c =>
                                    c.Visible ||
                                    Constant.RequireFields.Contains(c.ColumnName.ToLower())).ToList();
                    }
                    
                    builder = columns.Aggregate(builder, DynamicObjHelper.DefineObject);

                    DynamicObjHelper.CreateAutoImplementedProperty(builder, "Selected", typeof(bool));

                    Type resultType = builder.CreateType();
                    var moduleParam = new SqlParameter
                    {
                        ParameterName = "ModuleId",
                        Value = moduleId
                    };
                    var viewParam = new SqlParameter
                    {
                        ParameterName = "ViewId",
                        Value = viewId
                    };
                    var idParam = new SqlParameter
                    {
                        ParameterName = "Id",
                        Value = id
                    };
                    var scriptIdParam = new SqlParameter
                    {
                        ParameterName = "sqlScripts",
                        Value = script
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
                        ParameterName = "sortDirection",
                        Value = sortExpression
                    };
                    var totalRowParam = new SqlParameter
                    {
                        ParameterName = "totalRow",
                        Value = totalRow,
                        Direction = ParameterDirection.Output
                    };
                    var oderByParam = new SqlParameter
                    {
                        ParameterName = "defaultOderBy",
                        Value = defaultOderBy

                    };
                    var groupColumnParam = new SqlParameter
                    {
                        ParameterName = "columnGroup",
                        Value = !string.IsNullOrEmpty(groupColumn) && id == 0 ? groupColumn : string.Empty
                    };
                    var groupResultParam = new SqlParameter
                    {
                        ParameterName = "groupJsonStr",
                        Value = groupResult,
                        Direction = ParameterDirection.Output,
                        Size = 4000
                    };
                    var queryResult = context.Database.SqlQuery(
                        resultType, "sp_AdvanceSearch @ModuleId,@ViewId, @Id, @sqlScripts, @userId, @roleId, @pageIndex, @pageSize, @sortDirection, @totalRow out,@defaultOderBy,@columnGroup,@groupJsonStr out", moduleParam, viewParam, idParam, scriptIdParam, userIdParam, roleIdParam, pageIndexParam, pageSizeParam, sortParam, totalRowParam, oderByParam, groupColumnParam, groupResultParam);

                    var entities = queryResult.Cast<object>().ToList();
                    totalRow = (int)totalRowParam.Value;
                    groupResult = !string.IsNullOrEmpty(groupColumn) && entities.Count > 0
                        ? groupResultParam.Value.ToString()
                        : "[]";
                    return entities;
                }
                return new List<object>();
            }
        }

        public IList<vwFieldNameDataType> GenView(int viewId, int moduleId,int roleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var columns = context.vwFieldNameDataTypes.Where(r => r.ViewId == viewId && r.ModuleId == moduleId
                                                                            && (r.RoleId == roleId || r.RoleId == 0) && r.Visible && r.Show.HasValue && r.Show.Value)
                                      .OrderBy(r => r.ModuleId).ThenBy(r => r.RoleId).ThenBy(r => r.SortOrder).ToList();
                return columns;
            }
        }
        public IList<vwFieldNameDataType> GenView(int viewId, int moduleId,int roleId, IList<int> fieldIds)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var columns = context.vwFieldNameDataTypes.Where(r => r.ViewId == viewId && r.ModuleId == moduleId
                                                                      && r.RoleId == roleId 
                                                                      && r.Show.HasValue && r.Show.Value && fieldIds.Contains(r.FieldId))
                                      .OrderBy(r => r.ModuleId).ThenBy(r => r.RoleId).ThenBy(r => r.SortOrder).ToList();
                return columns;
            }
        }
        public IList<vwViewMenu> GetViewsWithTotal(int moduleId, int roleId, int userId)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var roleExp = "," + roleId + ",";

                var views = _context.vwViewMenus.Where(v => v.ModuleId == moduleId && v.Shared && (v.UserRole == null || v.UserRole == "" || ("," + v.UserRole + ",").Contains(roleExp)));
                views = views.Concat(_context.vwViewMenus.Where(v => v.ModuleId == moduleId && !v.Shared && v.CreatedBy == userId));
                foreach (var vwViewMenu in views)
                {
                    var param = new ObjectParameter("return", typeof(int));
                    _context.sp_GetTotalForView(vwViewMenu.ViewId, roleId, userId, param);
                    vwViewMenu.Total = (int)param.Value;
                }
                return views.OrderBy(v=>v.SortOrder).ToList();
            }
        }
        public Eli_Views GetDefaultViewByModule(string moduleName)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var query = from m in context.Eli_Modules
                            join v in context.Eli_Views on m.Id equals v.ModuleId
                            where m.Name.ToLower() == moduleName && v.DefaultView
                            select v;
                return query.SingleOrDefault();
            }
        }
        public IList<Eli_Views> GetViewList(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var parentModules = context.Eli_Modules.Single(r => r.Id == moduleId);
                return context.Eli_Views.AsNoTracking().Where(r => r.ModuleId == parentModules.RelatedTo && r.Shared).ToList();
            }
        }
        public Eli_TempViews GetTempView(int viewId, int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_TempViews.AsNoTracking().SingleOrDefault(r => r.CreatedBy == userId && r.ViewParentId == viewId);
            }
        }
        public int SaveTempView(Eli_TempViews entity)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (entity.Id > 0)
                {
                    context.Eli_TempViews.Attach((entity));
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.Eli_TempViews.Add(entity);
                }
                return context.SaveChanges();
            }
        }
        public int ResetSqlTempView(int viewId, int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var tempView = context.Eli_TempViews.AsNoTracking().SingleOrDefault(r => r.CreatedBy == userId && r.ViewParentId == viewId);
                if (tempView != null)
                {
                    var view = context.Eli_Views.Single(v => v.Id == viewId);
                    if (view != null)
                    {
                        tempView.QueryScript = view.QueryScript;
                        return context.SaveChanges();
                    }
                }

                return 0;
            }
        }
        public int SaveView(Eli_Views entity)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var selectCols = entity.Eli_ViewColumns.ToList();
                var conditionCols = entity.Eli_ViewConditions.ToList();
                var orderByCols = entity.Eli_ViewOrderBy.ToList();
                var groupByCols = entity.Eli_ViewGroupBy.ToList();

                entity.Eli_ViewColumns.Clear();
                entity.Eli_ViewConditions.Clear();
                entity.Eli_ViewOrderBy.Clear();
                entity.Eli_ViewGroupBy.Clear();

                // Update View Columns
                foreach (var item in selectCols.Where(r=> r.Id > 0))
                {
                    context.Eli_ViewColumns.Attach(item);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                //update view conditions     
                if (entity.Id > 0)
                {
                    var conditionIds = conditionCols.Where(x => x.Id > 0).Select(x => x.Id);

                    //clear all if have no any conditions, else only clear some specified conditions
                    var conds = conditionIds.Any() ? context.Eli_ViewConditions.Where(r => r.ViewId == entity.Id && !conditionIds.Contains(r.Id)).ToList() :
                                                     context.Eli_ViewConditions.Where(r => r.ViewId == entity.Id).ToList();
                    foreach (var item in conds)
                    {
                        context.Eli_ViewConditions.Remove(item);
                    }
                }

                if (conditionCols.Any()) //add or update
                {
                    foreach (var item in conditionCols)
                    {
                        if (item.Id > 0)
                        {
                            context.Eli_ViewConditions.Attach(item);
                            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                        else if (item.Id == 0)
                        {                            
                            item.ViewId = entity.Id;
                            context.Eli_ViewConditions.Add(item);
                        }
                    }
                }

                


                //update group column
                if (groupByCols.Count > 0)
                {
                    // Update View Group By
                    foreach (var item in groupByCols.Where(r => r.Id > 0))
                    {
                        context.Eli_ViewGroupBy.Attach(item);
                        context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else if(entity.Id > 0)
                {
                    var groups = context.Eli_ViewGroupBy.Where(r => r.ViewId == entity.Id);
                    foreach (var item in groups)
                    {
                        context.Eli_ViewGroupBy.Remove(item);
                    }
                }


                //update sort column
                if (orderByCols.Count > 0)
                {
                    // Update View Order By
                    foreach (var item in orderByCols.Where(r => r.Id > 0))
                    {
                        context.Eli_ViewOrderBy.Attach(item);
                        context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else if(entity.Id > 0)
                {
                    var orderBy = context.Eli_ViewOrderBy.Where(r => r.ViewId == entity.Id);
                    foreach (var item in orderBy)
                    {
                        context.Eli_ViewOrderBy.Remove(item);
                    }
                }


                context.Eli_Views.Attach(entity);
                foreach (var item in selectCols.Where(r => r.Id == 0))
                {
                    entity.Eli_ViewColumns.Add(item);
                }
                foreach (var item in orderByCols.Where(r => r.Id == 0))
                {
                    entity.Eli_ViewOrderBy.Add(item);
                }
                foreach (var item in groupByCols.Where(r => r.Id == 0))
                {
                    entity.Eli_ViewGroupBy.Add(item);
                }

                //Set Default View
                if (entity.DefaultView)
                {
                    var views = context.Eli_Views.Where(r => r.DefaultView && r.ModuleId == entity.ModuleId && r.Id != entity.Id).ToList();
                    foreach (var view in views)
                    {
                        view.DefaultView = false;
                    }
                }
                if (entity.Id > 0)
                {
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.Eli_Views.Add(entity);    
                }
                return context.SaveChanges();
            }
        }
        public int DeleteViews(string idArray, out string denyNames)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var outParam = new ObjectParameter("NameArray", typeof(string));
                int status = context.sp_DeleteViews(idArray, outParam);
                denyNames = outParam.Value.ToString();
                return status;
            }
        }
        public bool CheckSubviewBelongParent(Eli_Views entity,out IList<string> nameList)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                nameList= new List<string>();
                var module = context.Eli_Modules.Single(r => r.Id == entity.ModuleId);
                var views = context.Eli_Views.Where(r => r.ModuleId == module.Id && r.Id != entity.Id 
                                                        && !string.IsNullOrEmpty(r.ParentId));// Get only subview 
                foreach (var item in views)
                {
                    foreach (var viewId in entity.Parent)
                    {
                        if (item.ParentId.Contains(string.Format("{{{0}}}",viewId)))
                        {
                            var view = context.Eli_Views.Single(r => r.Id == viewId);
                            nameList.Add(view.ViewName);
                        }
                    }
                }
                return nameList.Count > 0;
            }
        }

        public IList<RelateView> GetRelateViews(int moduleId, int viewId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var moduleIds = context.Eli_Modules.Where(r => r.RelatedTo.HasValue && r.RelatedTo.Value == moduleId)
                                       .Select(r => r.Id).ToArray();
                if (moduleIds.Length == 0) return new List<RelateView>();
                var currentViewId = "{" + viewId + "}";
                var currentView = context.Eli_Views.Single(r => r.Id == viewId);
                if (currentView != null && currentView.MasterViewId.HasValue)
                {
                    currentViewId = "{" + currentView.MasterViewId.Value + "}";
                }
                var views = context.Eli_Views.Where(r => !string.IsNullOrEmpty(r.ParentId) && moduleIds.Contains(r.ModuleId) &&
                                                         r.ParentId.Contains(currentViewId)).ToList();
                return views.Select(item => new RelateView { ViewId = item.Id, ModuleId = item.ModuleId, Collapsed = true, Name = "RELATED_SUBVIEW_" }).ToList();
            }
        }

        /// <summary>
        /// Get View For CRM WebService
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public Eli_Views GetDefaultViewByModuleId(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var defaultView = context.vwModules.SingleOrDefault(r => r.Id == moduleId);
                if (defaultView == null) return null;
                var view = context.Eli_Views.Include("Eli_ViewColumns").Include("Eli_ViewConditions")
                    .Include("Eli_ViewOrderBy").SingleOrDefault(r => r.Id == defaultView.DefaultViewId && r.IsActive);
                return view;
            }
        }

        public List<vwViewMenu> GetFirstMenuEachModuleByRole(int role, int[] modules)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var filterValue = "," + role + ",";
                return context.vwViewMenus.OrderBy(x => x.SortOrder)
                                                   .Where(x => ("," + x.UserRole + ",").Contains(filterValue) && modules.Contains(x.ModuleId))
                                                   .GroupBy(x => x.ModuleId).Select(x => x.FirstOrDefault()).ToList();
            }
        }
    }
}
