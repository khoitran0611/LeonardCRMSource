using System;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.ViewRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ViewColumnBM : BusinessBase<IRepository<Eli_ViewColumns>, Eli_ViewColumns>
    {
        private static volatile ViewColumnBM _instance;
        private static readonly object SyncRoot = new Object();

        public static ViewColumnBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ViewColumnBM();
                    }
                }

                return _instance;
            }
        }
        private ViewColumnBM() : base(ViewColumnDA.Instance) { }
    }
}
