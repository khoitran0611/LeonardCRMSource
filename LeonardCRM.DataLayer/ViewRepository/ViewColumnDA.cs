using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;


namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewColumnDA :EF5RepositoryBase<LeonardUSAEntities,Eli_ViewColumns>
    {
        private static volatile ViewColumnDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewColumnDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new ViewColumnDA();
                }
                return _instance;
            }
        }
        private  ViewColumnDA():base(Settings.ConnectionString){}
    }
}
