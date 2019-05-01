using System;
using System.Collections.Generic;
using System.Linq;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class PicklistDependencyBM : BusinessBase<IRepository<Eli_ListDependency>, Eli_ListDependency>
    {
        private static volatile PicklistDependencyBM _instance;
        private static readonly object SyncRoot = new Object();
        public static PicklistDependencyBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new PicklistDependencyBM();
                    }
                }

                return _instance;
            }
        }

        private PicklistDependencyBM()
            : base(PicklistDependencyDA.Instance)
        {}

        public Eli_ListDependency GetObjById(int modelId)
        {
            var model = PicklistDependencyDA.Instance.GetObjById(modelId);
            return model;
        }

        public int SaveObject(Eli_ListDependency model)
        {
            return PicklistDependencyDA.Instance.SaveObject(model);
        }

        public IList<vwPicklistDependency> GetPicklistDependenciesByModuleId(int moduleId)
        {
            return PicklistDependencyDA.Instance.GetPicklistDependenciesByModuleId(moduleId);
        }
    }
}
