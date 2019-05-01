using System;
using System.Collections.Generic;
using System.Data;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using System.Linq;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesOrderCompleteDA : EF5RepositoryBase<LeonardUSAEntities, SalesOrderComplete>
    {
        private static volatile SalesOrderCompleteDA _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesOrderCompleteDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrderCompleteDA();
                    }
                }

                return _instance;
            }
        }
        private LeonardUSAEntities _context;

        public SalesOrderCompleteDA() : base(Settings.ConnectionString) { }

        public int UpdateSaleComplete(SalesOrderComplete saleComplete)
        {
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var appId = saleComplete.OrderId;

                var saleOrder = _context.SalesOrders.Include("SalesOrderDeliveries").FirstOrDefault(x => x.Id == appId);                

                if(saleComplete.SalesOrder != null )
                {
                    if (saleOrder.SerialNumber != saleComplete.SalesOrder.SerialNumber)
                    {
                        saleOrder.SerialNumber = saleComplete.SalesOrder.SerialNumber;                       
                    }

                    if (saleOrder.Status != saleComplete.SalesOrder.Status)
                    {
                        saleOrder.Status = saleComplete.SalesOrder.Status;
                    }

                    saleComplete.SalesOrder = null;
                }

                saleOrder.ModifiedBy = saleComplete.ModifiedBy;
                saleOrder.ModifiedDate = DateTime.Now;                
                _context.Entry(saleOrder).State = System.Data.Entity.EntityState.Modified;

                if (saleOrder.SalesOrderDeliveries.Any())
                {
                    var saleDelivery = saleOrder.SalesOrderDeliveries.First();
                    saleDelivery.CustomerAccepted = true;
                    _context.Entry(saleDelivery).State = System.Data.Entity.EntityState.Modified;
                }

                _context.Entry(saleComplete).State = saleComplete.Id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;
                               
                var status = _context.SaveChanges();
                if (status > 0)
                {
                    saleComplete.SalesOrder = saleOrder;
                }

                return status;
            }
        }
    }
}
