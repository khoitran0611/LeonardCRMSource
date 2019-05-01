using System;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;
using LeonardCRM.DataLayer.CommonRepository;

namespace LeonardCRM.BusinessLayer
{
    public sealed class CurrencyBM : BusinessBase<IRepository<Eli_Currency>,Eli_Currency>
    {
        private static volatile CurrencyBM _instance;
        private static readonly object SyncRoot = new Object();
        public static CurrencyBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new CurrencyBM();
                    }
                }

                return _instance;
            }
        }
        private CurrencyBM():base(CurrencyDA.Instance){}
    }
}
