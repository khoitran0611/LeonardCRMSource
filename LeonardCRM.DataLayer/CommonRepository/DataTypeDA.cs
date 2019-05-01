using System;
using Eli.Common;
using Elinext.DataLib;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class DataTypeDA : EF5RepositoryBase<LeonardUSAEntities, Eli_DataTypes>
    {
        private static volatile DataTypeDA _instance;
        private static readonly object SyncRoot = new Object();

        public static DataTypeDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DataTypeDA();
                    }
                }

                return _instance;
            }
        }

        private DataTypeDA() : base(Settings.ConnectionString) { }

    }
}