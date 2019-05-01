using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class CurrencyDA : EF5RepositoryBase<LeonardUSAEntities,Eli_Currency>
    {
        private static volatile CurrencyDA _instance;
        private static readonly object SyncRoot = new Object();
        public static CurrencyDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new CurrencyDA();
                    }
                }

                return _instance;
            }
        }
        private  CurrencyDA() : base(Settings.ConnectionString){}
    }
}
