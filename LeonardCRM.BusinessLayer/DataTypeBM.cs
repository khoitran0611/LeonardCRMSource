using System;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class DataTypeBM : BusinessBase<IRepository<Eli_DataTypes>, Eli_DataTypes>
    {
        private static volatile DataTypeBM _instance;
        private static readonly object SyncRoot = new Object();
        public static DataTypeBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DataTypeBM();
                    }
                }

                return _instance;
            }
        }
        private DataTypeBM()
            : base(DataTypeDA.Instance)
        {}
    }
}
