using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class FieldDataDA : EF5RepositoryBase<LeonardUSAEntities,Eli_FieldData>
    {
        private static volatile FieldDataDA _instance;
        private static readonly object SyncRoot = new Object();
        public static FieldDataDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new FieldDataDA();
                    }
                }

                return _instance;
            }
        }
        private  FieldDataDA():base(Settings.ConnectionString){}
    }
}
