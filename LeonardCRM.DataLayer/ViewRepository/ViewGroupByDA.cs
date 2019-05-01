using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewGroupByDA:EF5RepositoryBase<LeonardUSAEntities, Eli_ViewGroupBy>
    {
        private static volatile ViewGroupByDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewGroupByDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new ViewGroupByDA();
                }
                return _instance;
            }
        }
        private ViewGroupByDA() : base(Settings.ConnectionString) { }
    }
}
