using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class SysAuditDA : EF5RepositoryBase<LeonardUSAEntities, Eli_SysAudit>
    {
        private static volatile SysAuditDA _instance;
        private static readonly object SyncRoot = new Object();

        public static SysAuditDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SysAuditDA();
                    }
                }

                return _instance;
            }
        }

        public SysAuditDA()
            : base(Settings.ConnectionString)
        {
        }

        public IList<vwSystmAudit> GetByModuleIdnRecordId(PageInfo pageInfo)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var param = new ObjectParameter("totalRow", typeof(int));
                var models = context.sp_FilterSystemAudit(pageInfo.ModuleId, pageInfo.Id, null, string.Empty,
                    string.Empty, null, pageInfo.PageIndex, pageInfo.PageSize, param).ToList();

                pageInfo.TotalRow = (int)param.Value;
                return models;
            }
        }

        public IList<vwSystmAudit> ServerFilter(PageInfo pageInfo, Eli_SysAudit condition)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var param = new ObjectParameter("totalRow", typeof(int));
                var models = context.sp_FilterSystemAudit(pageInfo.ModuleId, pageInfo.Id, condition.DateModified, condition.Operation,
                    condition.ColumnName, condition.CreatedBy > 0 ? condition.CreatedBy : null, pageInfo.PageIndex, pageInfo.PageSize, param).ToList();

                pageInfo.TotalRow = (int) param.Value;
                return models;
            }
        }
    }
}
