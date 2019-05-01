using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class EntityFieldBM : BusinessBase<IRepository<Eli_EntityFields>, Eli_EntityFields>
    {
        private static volatile EntityFieldBM _instance;
        private static readonly object SyncRoot = new Object();
        public static EntityFieldBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new EntityFieldBM();
                    }
                }

                return _instance;
            }
        }
        private EntityFieldBM()
            : base(EntityFieldDA.Instance)
        { }

        public IList<vwFieldsDataType> GetAllFields()
        {
            //if the cache is empty, then getting from db
            var cache = new CacheManager();
            if (!ConfigValues.ENABLE_CACHE)
                cache.Remove(Constant.AllEntityFieldsCacheKey); 

            if (!cache.Exist(Constant.AllEntityFieldsCacheKey))
            {
                cache.Remove(Constant.AllEntityFieldsCacheKey);
                cache.Add(EntityFieldDA.Instance.GetAllFields(), Constant.AllEntityFieldsCacheKey);
            }
            return cache.Get<IList<vwFieldsDataType>>(Constant.AllEntityFieldsCacheKey);
        }

        public IList<vwFieldsDataType> GetAllFieldsByModule(int moduleId)
        {
            return GetAllFields().Where(v => v.ModuleId == moduleId).ToList();
        }

        public IList<vwViewColumn> GetCachedViewColumns()
        {
            var cache = new CacheManager();
            
            if(!ConfigValues.ENABLE_CACHE)
            cache.Remove(Constant.ViewColumnsCacheKey); 

            if (!cache.Exist(Constant.ViewColumnsCacheKey))
            {
                cache.Remove(Constant.ViewColumnsCacheKey);
                cache.Add(EntityFieldDA.Instance.GetAllViewColumns(), Constant.ViewColumnsCacheKey);
            }
            return cache.Get<IList<vwViewColumn>>(Constant.ViewColumnsCacheKey);
        }
        public IList<vwField> GetCachedFields()
        {
            var cache = new CacheManager();

            if (!ConfigValues.ENABLE_CACHE)
                cache.Remove(Constant.ViewColumnsCacheKey);

            if (!cache.Exist(Constant.ViewColumnsCacheKey))
            {
                cache.Remove(Constant.ViewColumnsCacheKey);
                cache.Add(EntityFieldDA.Instance.GetFields(), Constant.ViewColumnsCacheKey);
            }
            return cache.Get<IList<vwField>>(Constant.ViewColumnsCacheKey);
        }

        public IList<vwViewColumn> GetViewColumnsByModule(int moduleId, bool onlyDisplay, int roleId)
        {
            return onlyDisplay
                ? GetCachedViewColumns()
                    .Where(m => m.ModuleId == moduleId && m.RoleId == roleId && m.Visible.HasValue && m.Visible.Value)
                    .ToList()
                : GetCachedViewColumns().Where(m => m.ModuleId == moduleId && m.RoleId == roleId).ToList();
        }

        public IList<Eli_EntityFields> GetManageFieldsByModuleId(int moduleId)
        {
            return EntityFieldDA.Instance.GetManageFieldsByModuleId(moduleId);
        }

        public IList<vwEntityFieldData> GetCustomFieldByModuleId(int moduleid, int masterId,int roleId)
        {
            return EntityFieldDA.Instance.GetCustomFieldByModuleId(moduleid, masterId,roleId);
        }

        public bool CheckEntityFieldIsUsing(int id)
        {
            return EntityFieldDA.Instance.CheckEntityFieldIsUsing(id);
        }

        public IList<vwField> GetFields(int moduleId,bool onlyDisplay)
        {
            return onlyDisplay ? GetCachedFields().Where(r=> r.ModuleId == moduleId && r.Display).ToList()
                               : GetCachedFields().Where(r => r.ModuleId == moduleId).ToList();
        }
        public IList<Eli_EntityFields> GetAllCustFieldByModule(int moduleId)
        {
            return EntityFieldDA.Instance.GetAllCustFieldByModule(moduleId);
        }
    }
}
