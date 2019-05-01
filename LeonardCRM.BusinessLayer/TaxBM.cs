using System;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class TaxBM : BusinessBase<IRepository<Eli_Tax>, Eli_Tax>
    {
        private static volatile TaxBM _instance;
        private static readonly object SyncRoot = new Object();

        public static TaxBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new TaxBM();
                    }
                }

                return _instance;
            }
        }
        private TaxBM()
            : base(TaxDA.Instance)
        { }
    }
}
