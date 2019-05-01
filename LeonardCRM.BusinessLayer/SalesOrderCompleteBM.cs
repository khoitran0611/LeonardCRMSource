using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.SalesRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class SalesOrderCompleteBM : BusinessBase<IRepository<SalesOrderComplete>, SalesOrderComplete>
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("SALES_CUSTOMER", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        private static volatile SalesOrderCompleteBM _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesOrderCompleteBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrderCompleteBM();
                    }
                }

                return _instance;
            }
        }

        private SalesOrderCompleteBM()
            : base(SalesOrderCompleteDA.Instance)
        { }

        public int UpdateSaleComplete(SalesOrderComplete saleComplete)
        {
            return SalesOrderCompleteDA.Instance.UpdateSaleComplete(saleComplete);
        }
    }
}
