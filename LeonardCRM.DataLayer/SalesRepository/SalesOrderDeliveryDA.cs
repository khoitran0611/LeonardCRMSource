using System;
using System.Collections.Generic;
using System.Data.Entity;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using System.Linq;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesOrderDeliveryDA : EF5RepositoryBase<LeonardUSAEntities, SalesOrderDelivery>
    {
        private static volatile SalesOrderDeliveryDA _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesOrderDeliveryDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrderDeliveryDA();
                    }
                }

                return _instance;
            }
        }
        private LeonardUSAEntities _context;

        public SalesOrderDeliveryDA() : base(Settings.ConnectionString) { }



        public int UpdateSaleDelivery(SalesOrderDelivery saleDelivery)
        {
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                _context.Entry(saleDelivery).State = saleDelivery.Id == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;

                if (saleDelivery.SalesOrder != null)
                {
                    _context.Entry(saleDelivery.SalesOrder).State = System.Data.Entity.EntityState.Modified;

                    if (saleDelivery.SalesOrder.SalesDocuments != null &&
                        saleDelivery.SalesOrder.SalesDocuments.Any())
                    {
                        foreach (var doc in saleDelivery.SalesOrder.SalesDocuments)
                        {
                            _context.Entry(doc).State = EntityState.Unchanged;
                        }
                    }

                    if (saleDelivery.SalesOrder.SalesCustomer != null)
                    {
                        _context.Entry(saleDelivery.SalesOrder.SalesCustomer).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                return _context.SaveChanges();
            }
        }
    }
}
