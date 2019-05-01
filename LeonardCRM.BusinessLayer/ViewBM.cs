using System;
using System.Collections.Generic;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;
using LeonardCRM.DataLayer.ViewRepository;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ViewBM : BusinessBase<IRepository<Eli_Views>, Eli_Views>
    {
        private static volatile ViewBM _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ViewBM();
                    }
                }

                return _instance;
            }
        }
        private ViewBM() : base(ViewDA.Instance) { }

        public IList<object> GetView(out string moduleName, int viewId, int moduleId, int id, int userId, int roleId, int pageIndex, int pageSize, out int totalRow, string sortExpression, bool defaultOderBy,string groupColumn,out string groupResult)
        {
            return ViewDA.Instance.GetView(out moduleName, viewId, moduleId, id, userId, roleId, pageIndex, pageSize, out totalRow, sortExpression, defaultOderBy, groupColumn, out groupResult);
        }

        public IList<object> AdvanceSearch(out string moduleName, int viewId, int moduleId, int id, string script, int userId,
            int roleId, int pageIndex, int pageSize, out int totalRow, string sortExpression, bool defaultOderBy, string groupColumn,out string groupResult, List<int> fieldIdsSelected = null)
        {
            return ViewDA.Instance.AdvanceSearch(out moduleName, viewId, moduleId,id, script, userId, roleId, pageIndex,
                pageSize, out totalRow, sortExpression, defaultOderBy, groupColumn, out groupResult, fieldIdsSelected);
        }
        
        public IList<vwFieldNameDataType> GenView(int viewId, int moduleId,int roleId)
        {
            return ViewDA.Instance.GenView(viewId, moduleId,roleId);
        }
        public IList<vwFieldNameDataType> GenView(int viewId, int moduleId,int roleId, IList<int> fieldIds)
        {
            return ViewDA.Instance.GenView(viewId, moduleId,roleId,fieldIds);
        }
        public IList<vwFieldNameDataType> GenSubView(int viewId, int moduleId,int roleId)
        {
            return ViewDA.Instance.GenView(viewId, moduleId,roleId);
        }

        public IList<vwViewMenu> GetViewsWithTotal(int moduleId, int roleId, int userId)
        {
            return ViewDA.Instance.GetViewsWithTotal(moduleId, roleId, userId);
        }

        public Eli_Views GetViewByModule(string moduleName)
        {
            return ViewDA.Instance.GetDefaultViewByModule(moduleName);
        }

        public IList<Eli_Views> GetViewList(int moduleId)
        {
            return ViewDA.Instance.GetViewList(moduleId);
        }

        public int SaveView(Eli_Views entity)
        {
            return ViewDA.Instance.SaveView(entity);
        }

        public Eli_TempViews GetTempView(int viewId, int userId)
        {
            return ViewDA.Instance.GetTempView(viewId, userId);
        }

        public int SaveTempView(Eli_TempViews entity)
        {
            return ViewDA.Instance.SaveTempView(entity);
        }
        public int ResetSqlTempView(int viewId, int userId)
        {
            return ViewDA.Instance.ResetSqlTempView(viewId, userId);
        }
        public int DeleteViews(string idArray, out string denyNames)
        {
            return ViewDA.Instance.DeleteViews(idArray, out denyNames);
        }

        public bool CheckSubviewBelongParent(Eli_Views entity, out IList<string> nameList)
        {
            return ViewDA.Instance.CheckSubviewBelongParent(entity, out nameList);
        }

        public IList<RelateView> GetRelateViews(int moduleId, int viewId)
        {
            return ViewDA.Instance.GetRelateViews(moduleId, viewId);
        }

        /// <summary>
        /// get View For CRM WebService
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public Eli_Views GetDefaultViewByModuleId(int moduleId)
        {
            return ViewDA.Instance.GetDefaultViewByModuleId(moduleId);
        }

        public List<vwViewMenu> GetFirstMenuEachModuleByRole(int role, int[] moduleNames)
        {
            return ViewDA.Instance.GetFirstMenuEachModuleByRole(role, moduleNames);
        }
    }
}
