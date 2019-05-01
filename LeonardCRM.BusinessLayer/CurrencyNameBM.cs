using System;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;
using LeonardCRM.DataLayer.CommonRepository;

namespace LeonardCRM.BusinessLayer
{
    public sealed class CurrencyNameBM :BusinessBase<IRepository<Eli_CurrencyNames>,Eli_CurrencyNames>
    {
        private static volatile CurrencyNameBM _instance;
        private static readonly object SyncRoot = new Object();
        public static CurrencyNameBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new CurrencyNameBM();
                    }
                }

                return _instance;
            }
        }
        private CurrencyNameBM() : base(CurrencyNameDA.Instance) { }
    }
}
