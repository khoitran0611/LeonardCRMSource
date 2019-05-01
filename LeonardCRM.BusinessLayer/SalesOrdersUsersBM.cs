using System;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.SalesRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class SalesOrdersUsersBM :BusinessBase<IRepository<SalesOrdersUser>,SalesOrdersUser>
    {
        private static volatile SalesOrdersUsersBM _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesOrdersUsersBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrdersUsersBM();
                    }
                }

                return _instance;
            }
        }
        public SalesOrdersUsersBM():base(SalesOrdersUsersDA.Instance){}
    }
}
