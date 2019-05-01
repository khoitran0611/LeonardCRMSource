using System;
using System.Collections.Generic;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class SysAuditBM : BusinessBase<IRepository<Eli_SysAudit>, Eli_SysAudit>
    {
        private static volatile SysAuditBM _instance;
        private static readonly object SyncRoot = new Object();

        public static SysAuditBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SysAuditBM();
                    }
                }

                return _instance;
            }
        }
        private SysAuditBM()
            : base(SysAuditDA.Instance)
        { }

        public IList<vwSystmAudit> GetByModuleIdnRecordId(PageInfo pageInfo)
        {
            return SysAuditDA.Instance.GetByModuleIdnRecordId(pageInfo);
        }

        public IList<vwSystmAudit> ServerFilter(PageInfo pageInfo, Eli_SysAudit condition)
        {
            return SysAuditDA.Instance.ServerFilter(pageInfo, condition);
        }
    }
}
