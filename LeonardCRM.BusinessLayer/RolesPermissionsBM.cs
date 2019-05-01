using System;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class RolesPermissionsBM : BusinessBase<IRepository<Eli_RolesPermissions>, Eli_RolesPermissions>
    {
        private static volatile RolesPermissionsBM _instance;
        private static readonly object SyncRoot = new Object();
        public static RolesPermissionsBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RolesPermissionsBM();
                    }
                }

                return _instance;
            }
        }
        private RolesPermissionsBM()
            : base(RolesPermissionsDA.Instance)
        {}
    }
}
