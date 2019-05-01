using System;
using System.Collections.Generic;
using Eli.Common;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class LogBM : BusinessBase<IRepository<Eli_Log>, Eli_Log>
    {
        private static volatile LogBM _instance;
        private static readonly object SyncRoot = new Object();
        public static LogBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new LogBM();
                    }
                }

                return _instance;
            }
        }
        private LogBM()
            : base(LogDA.Instance)
        { }

        public void ClearLog()
        {
            LogDA.Instance.ExecuteSqlCommand(ProcedureContainer.ClearLog);
        }

        public IList<Eli_Log> GetLogView(int? pageIndex, int? pageSize, out int totalRow)
        {
            var entities = LogDA.Instance.SelectWithPaging(null, pageIndex -1 , pageSize, t => t.Date, true);
            totalRow = LogDA.Instance.Count();
            return entities;
        }

    }
}
