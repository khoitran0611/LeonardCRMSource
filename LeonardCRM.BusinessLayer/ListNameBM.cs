using System;
using System.Collections.Generic;
using Eli.Common;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ListNameBM : BusinessBase<IRepository<Eli_ListNames>, Eli_ListNames>
    {
        private static volatile ListNameBM _instance;
        private static readonly object SyncRoot = new Object();
        private readonly CacheManager _cacheManager = new CacheManager();
        public static ListNameBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ListNameBM();
                    }
                }

                return _instance;
            }
        }
        private ListNameBM()
            : base(ListNameDA.Instance)
        { }

        public IList<vwListNameValue> GetListNameValuesByModuleId(int moduleId)
        {
            if (!_cacheManager.Exist("PickList_" + moduleId))
            {
                _cacheManager.Remove("PickList_" + moduleId);
                _cacheManager.Add(ListNameDA.Instance.GetListNameValuesByModuleId(moduleId), "PickList_" + moduleId);
            }
            return _cacheManager.Get<IList<vwListNameValue>>("PickList_" + moduleId);
        }

        public IList<vwListNameValue> GetListNameValuesByModules(int[] modules)
        {
            return ListNameDA.Instance.GetListNameValuesByModules(modules);
        }

        public IList<GetReferenceListValues_Result> GetReferenceListsByModules(int[] modules)
        {
            return ListNameDA.Instance.GetReferenceListsByModules(modules);
        }

        public IList<GetReferenceListValues_Result> GetReferenceListsByModule(int moduleId)
        {
            return ListNameDA.Instance.GetReferenceListsByModule(moduleId);
        }

        public int AddListName(Eli_ListNames listName)
        {
            return ListNameDA.Instance.AddListName(listName);
        }

        public int UpdateListName(Eli_ListNames listName)
        {
            return ListNameDA.Instance.UpdateListName(listName);
        }

        public IList<vwListNameValue> GetListNameValuesByModuleId(int moduleId, string listName)
        {
            return ListNameDA.Instance.GetListNameValuesByModuleId(moduleId, listName);
        }
        public IList<vwListNameValue> GetAllListNameValues()
        {
            return ListNameDA.Instance.GetAllListNameValues();
        }
    }
}
