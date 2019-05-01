using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class TaxDA : EF5RepositoryBase<LeonardUSAEntities, Eli_Tax>
    {
        private static volatile TaxDA _instance;
        private static readonly object SyncRoot = new Object();

        public static TaxDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new TaxDA();
                    }
                }

                return _instance;
            }
        }

        public TaxDA()
            : base(Settings.ConnectionString)
        {
        }

        
    }
}
