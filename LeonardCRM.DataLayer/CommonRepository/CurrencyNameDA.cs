using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class CurrencyNameDA :EF5RepositoryBase<LeonardUSAEntities,Eli_CurrencyNames>
    {
        private static volatile CurrencyNameDA _instance;
        private static readonly object SyncRoot = new Object();
        public static CurrencyNameDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new CurrencyNameDA();
                    }
                }

                return _instance;
            }
        }
        private CurrencyNameDA():base(Settings.ConnectionString){}
    }
}
