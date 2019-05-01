using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.ViewRepository
{
    public sealed class ViewConditionDA :EF5RepositoryBase<LeonardUSAEntities, Eli_ViewConditions>
    {
        private static volatile ViewConditionDA _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewConditionDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new ViewConditionDA();
                }
                return _instance;
            }
        }

        private  ViewConditionDA():base(Settings.ConnectionString){}
    }
}
