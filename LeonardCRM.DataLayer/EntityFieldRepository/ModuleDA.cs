using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class ModuleDA:EF5RepositoryBase<LeonardUSAEntities, Eli_Modules>
    {
        private static volatile ModuleDA _instance;
        private static readonly object SyncRoot = new Object();
        public static ModuleDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ModuleDA();
                    }
                }

                return _instance;
            }
        }
        private ModuleDA():base(Settings.ConnectionString){}

        public IList<vwModule> GetAllModules(int roleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.sp_GetModulesByRole(roleId).ToList();
            }
        }

        public IList<vwModuleEnittyRelationship> GetModuleRelationship()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwModuleEnittyRelationships.AsNoTracking().ToList();
            }
        }

        public IList<vwModuleEntityField> GetModuleEntityByModuleId(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwModuleEntityFields.AsNoTracking().Where(r=> r.ModuleId == moduleId).ToList();
            }
        }

        public IList<vwModuleHasEntityField> GetModuleForEntityFields()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwModuleHasEntityFields.AsNoTracking().OrderBy(r=> r.DisplayName).ToList();
            }
        }

        public IList<Eli_Modules> GetModules()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_Modules.AsNoTracking().Where(r => r.IsPublished)
                              .OrderBy(r=> r.SortOrder).ToList();
            }
        }
    }
}
