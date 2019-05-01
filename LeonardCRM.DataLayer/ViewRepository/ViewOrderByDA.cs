using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewOrderByDA :EF5RepositoryBase<LeonardUSAEntities, Eli_ViewOrderBy>
    {
        private static volatile ViewOrderByDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewOrderByDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new ViewOrderByDA();
                }
                return _instance;
            }
        }
        private ViewOrderByDA():base(Settings.ConnectionString){}
    }
}
