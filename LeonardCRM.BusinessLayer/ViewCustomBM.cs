using System;
using System.Collections.Generic;
using Eli.Common;
using Elinext.BusinessLib;
using Elinext.DataLib;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.ViewRepository;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ViewCustomBM : BusinessBase<IRepository<Eli_ViewCustom>, Eli_ViewCustom>
    {
        private static volatile ViewCustomBM _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewCustomBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ViewCustomBM();
                    }
                }

                return _instance;
            }
        }

        private ViewCustomBM()
            : base(ViewCustomDA.Instance)
        {
            
        }

        public IList<object> GetViewCustom(int viewId, int pageIndex, int pageSize, out int totalRow
            , IList<FilterObj> filerArray
            , string sortExpression
            , string groupName
            , out string groupJson
            , int userId = 0
            , int roleId = 0)
        {
            return ViewCustomDA.Instance.GetViewCustom(viewId, pageIndex, pageSize, out totalRow, filerArray,
                sortExpression, groupName, out groupJson, userId, roleId);
        }

        public bool CreateCustomView(ViewCustomCreatedModel viewCustom, int currentUserId)
        {
            return ViewCustomDA.Instance.CreateCustomView(viewCustom, currentUserId);
        }

        public bool SaveCustomView(ViewCustomUpdatedModel viewCustom, int currentUserId)
        {
            return ViewCustomDA.Instance.SaveCustomView(viewCustom, currentUserId);
        }

        public Eli_ViewCustom GetViewObjById(int viewId)
        {
            return ViewCustomDA.Instance.GetViewObjById(viewId);
        }

        public bool SaveView(Eli_ViewCustom view, int CurrentUserID)
        {
            return ViewCustomDA.Instance.EditCustomView(view, CurrentUserID);
        }

        /// <summary>
        /// Delete view by Id
        /// </summary>
        /// <param name="viewId">specified view</param>
        /// <param name="userId">owner of view</param>
        /// <returns>0:fail, -1: not owner, else: success </returns>
        public int DeleteById(int viewId, int userId)
        {
            return ViewCustomDA.Instance.DeleteById(viewId, userId);
        }
    }
}
