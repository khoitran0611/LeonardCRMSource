using System;
using System.Collections.Generic;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class FieldSectionBM : BusinessBase<IRepository<Eli_FieldsSection>, Eli_FieldsSection>
    {
        private static volatile FieldSectionBM _instance;
        private static readonly object SyncRoot = new Object();

        public static FieldSectionBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new FieldSectionBM();
                    }
                }

                return _instance;
            }
        }

        private FieldSectionBM()
            : base(FieldSectionDA.Instance)
        {
        }

        public IList<Eli_FieldsSection> GetFieldSectionByModuleId(int moduleId)
        {
            return FieldSectionDA.Instance.GetFieldSectionByModuleId(moduleId);
        }

        public int SaveObjs(IList<Eli_FieldsSection> models, int moduleId)
        {
            return FieldSectionDA.Instance.SaveObjs(models, moduleId);
        }
    }
}
