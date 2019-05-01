using System;
using System.Collections.Generic;
using Eli.Common;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ModuleBM : BusinessBase<IRepository<Eli_Modules>, Eli_Modules>
    {
        private static volatile ModuleBM _instance;
        private static readonly object SyncRoot = new Object();
        public static ModuleBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ModuleBM();
                    }
                }

                return _instance;
            }
        }
        private ModuleBM()
            : base(ModuleDA.Instance)
        {}

        public IList<vwModule> GetAllModulesWithDefaultView(int roleId)
        {
            var cache = new CacheManager();
            if(!ConfigValues.ENABLE_CACHE)
                cache.Remove(Constant.ModulesCacheKey + roleId);
    
            if (!cache.Exist(Constant.ModulesCacheKey + roleId))
            {
                cache.Remove(Constant.ModulesCacheKey + roleId);
                cache.Add(ModuleDA.Instance.GetAllModules(roleId), Constant.ModulesCacheKey + roleId);
            }
            return cache.Get<IList<vwModule>>(Constant.ModulesCacheKey + roleId);
        }

        public IList<vwModuleHasEntityField> GetModuleForEntityFields()
        {
            return ModuleDA.Instance.GetModuleForEntityFields();
        }

        public IList<Eli_Modules> GetModules()
        {
            return ModuleDA.Instance.GetModules();
        }
    }
}
