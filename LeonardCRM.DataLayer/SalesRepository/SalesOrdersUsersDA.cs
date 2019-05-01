using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesOrdersUsersDA : EF5RepositoryBase<LeonardUSAEntities,SalesOrdersUser>
    {
        private static volatile SalesOrdersUsersDA _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesOrdersUsersDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrdersUsersDA();
                    }
                }

                return _instance;
            }
        }
        public SalesOrdersUsersDA():base(Settings.ConnectionString){}
    }
}
