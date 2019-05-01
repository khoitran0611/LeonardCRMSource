using System;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class RegistryDA : EF5RepositoryBase<LeonardUSAEntities, Eli_Registry>
    {
        private static volatile RegistryDA _instance;
        private static readonly object SyncRoot = new Object();

        public static RegistryDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RegistryDA();
                    }
                }

                return _instance;
            }
        }

        private RegistryDA() : base(Settings.ConnectionString) { }

        public System.Collections.Generic.IList<vwRegistry> GetRegistryItems()
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return _context.vwRegistries.ToList();
            }
        }
    }
}
