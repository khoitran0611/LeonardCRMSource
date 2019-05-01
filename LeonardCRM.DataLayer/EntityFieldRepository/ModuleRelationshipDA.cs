using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class ModuleRelationshipDA :EF5RepositoryBase<LeonardUSAEntities, Eli_ModuleRelationship>
    {
        private static volatile ModuleRelationshipDA _instance;
        private static readonly object SyncRoot = new Object();
        public static ModuleRelationshipDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ModuleRelationshipDA();
                    }
                }

                return _instance;
            }
        }
        private ModuleRelationshipDA():base(Settings.ConnectionString){}
    }
}
