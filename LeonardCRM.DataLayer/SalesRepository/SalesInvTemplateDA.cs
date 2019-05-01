using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesInvTemplateDA : EF5RepositoryBase<LeonardUSAEntities,SalesInvTemplate>
    {
        private static volatile SalesInvTemplateDA _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesInvTemplateDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new SalesInvTemplateDA();
                }
                return _instance;
            }
        }
        private SalesInvTemplateDA():base(Settings.ConnectionString){}
    }
}
