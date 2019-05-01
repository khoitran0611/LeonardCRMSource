using System;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.ViewRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ViewGroupByBM : BusinessBase<IRepository<Eli_ViewGroupBy>, Eli_ViewGroupBy>
    {
        private static volatile ViewGroupByBM _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewGroupByBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ViewGroupByBM();
                    }
                }

                return _instance;
            }
        }
        private ViewGroupByBM() : base(ViewGroupByDA.Instance) { }
    }
}
