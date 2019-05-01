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
    public sealed class SalesOrderDeliveryBM : BusinessBase<IRepository<SalesOrderDelivery>, SalesOrderDelivery>
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("SALES_CUSTOMER", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        private static volatile SalesOrderDeliveryBM _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesOrderDeliveryBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrderDeliveryBM();
                    }
                }

                return _instance;
            }
        }

        private SalesOrderDeliveryBM()
            : base(SalesOrderDeliveryDA.Instance)
        { }

        public int UpdateSaleDelivery(SalesOrderDelivery saleDelivery)
        {
            return SalesOrderDeliveryDA.Instance.UpdateSaleDelivery(saleDelivery);
        }
    }
}
