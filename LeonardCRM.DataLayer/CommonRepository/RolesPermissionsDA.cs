using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class RolesPermissionsDA : EF5RepositoryBase<LeonardUSAEntities,Eli_RolesPermissions>
    {
        private static volatile RolesPermissionsDA _instance;
        private static readonly object SyncRoot = new Object();
        public static RolesPermissionsDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RolesPermissionsDA();
                    }
                }

                return _instance;
            }
        }
        private RolesPermissionsDA() : base(Settings.ConnectionString) { }
    }
}
