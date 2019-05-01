using System;
using System.Collections.Generic;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ModulesRelationshipBM : BusinessBase<IRepository<Eli_ModuleRelationship>, Eli_ModuleRelationship>
    {
        private static volatile ModulesRelationshipBM _instance;
        private static readonly object SyncRoot = new Object();

        public static ModulesRelationshipBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ModulesRelationshipBM();
                    }
                }

                return _instance;
            }
        }
        private ModulesRelationshipBM()
            : base(ModulesRelationshipDA.Instance)
        { }

        public void UpdateChildEntity(Eli_EntityFields childEntity, int moduleId, string masterFieldName, string masterDisplayColumn)
        {
            ModulesRelationshipDA.Instance.UpdateChildEntity(childEntity, moduleId, masterFieldName, masterDisplayColumn);
        }

        public IList<Eli_Modules> GetModules()
        {
            return ModulesRelationshipDA.Instance.GetModules();
        }

        public IList<Eli_ModuleRelationship> GetRelationshipByModules(int masterModuleId
            , int childModuleId)
        {
            return ModulesRelationshipDA.Instance.GetRelationshipByModules(masterModuleId, childModuleId);
        }

        public int GetMaxAlias()
        {
            return ModulesRelationshipDA.Instance.GetMaxAlias();
        }

        public bool IsDuplicatedRelationship(Eli_ModuleRelationship moduleRelationship)
        {
            return ModulesRelationshipDA.Instance.IsDuplicatedRelationship(moduleRelationship);
        }
    }
}
