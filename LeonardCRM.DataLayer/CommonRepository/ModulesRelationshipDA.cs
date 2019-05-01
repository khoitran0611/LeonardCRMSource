using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class ModulesRelationshipDA : EF5RepositoryBase<LeonardUSAEntities, Eli_ModuleRelationship>
    {
        private static volatile ModulesRelationshipDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ModulesRelationshipDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ModulesRelationshipDA();
                    }
                }

                return _instance;
            }
        }

        public ModulesRelationshipDA()
            : base(Settings.ConnectionString)
        {
        }

        public void UpdateChildEntity(Eli_EntityFields childEntity, int moduleId, string masterFieldName, string masterDisplayColumn)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                childEntity.ForeignKey = true;

                var masterModule = context.Eli_Modules.FirstOrDefault(p => p.Id.Equals(moduleId));
                if (masterModule != null && masterModule.DefaultTable != null)
                {
                    childEntity.ListSql = string.Format("Select {0},{1} as Description from {2}"
                        , masterFieldName
                        , masterDisplayColumn
                        , masterModule.DefaultTable);
                }

                context.Eli_EntityFields.Attach(childEntity);
                context.Entry(childEntity).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public IList<Eli_Modules> GetModules()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_Modules
                    .Where(p => p.DefaultTable != null && p.DefaultTable != "")
                    .OrderBy(r => r.DisplayName).ToList();
            }
        }

        public IList<Eli_ModuleRelationship> GetRelationshipByModules(int masterModuleId, int childModuleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var vwModuleEnittyRelationship = context.vwModuleEnittyRelationships.Where(p => p.MasterModuleId.Equals(masterModuleId) && p.ChildModuleId.Equals(childModuleId))
                        .AsNoTracking()
                        .ToList();

                return vwModuleEnittyRelationship.Select(p => new Eli_ModuleRelationship
                {
                    Id = p.Id,
                    MasterFieldId = p.MasterId,
                    MasterFieldName = p.MasterFieldName,
                    MasterDisplayColumn = p.MasterDisplayColumn,
                    ChildFieldId = p.ChildId,
                    ChildFieldName = p.ChildFieldName,
                    Alias = p.MasterAlias,
                    MasterTableName = p.MasterModule,
                    ChildTableName = p.ChildModule
                }).ToList();
            }
        }

        public int GetMaxAlias()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var moduleRelationship = context.Eli_ModuleRelationship.OrderByDescending(p => p.Id).FirstOrDefault();
                if (moduleRelationship != null)
                    return Convert.ToInt32(moduleRelationship.Alias.Remove(0, 1));
                return 0;
            }
        }

        public bool IsDuplicatedRelationship(Eli_ModuleRelationship moduleRelationship)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_ModuleRelationship.Any(
                        p => p.MasterFieldId.Equals(moduleRelationship.MasterFieldId) &&
                             p.ChildFieldId.Equals(moduleRelationship.ChildFieldId));
            }
        }
    }
}
