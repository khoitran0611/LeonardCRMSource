using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class LogDA : EF5RepositoryBase<LeonardUSAEntities, Eli_Log>
    {

        private static volatile LogDA _instance;
        private static readonly object SyncRoot = new Object();
        public static LogDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new LogDA();
                    }
                }

                return _instance;
            }
        }

        private LogDA() : base(Settings.ConnectionString) { }

    }
}
