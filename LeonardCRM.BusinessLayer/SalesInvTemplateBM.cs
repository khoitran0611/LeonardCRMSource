using System;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.SalesRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer 
{
    public sealed class SalesInvTemplateBM : BusinessBase<IRepository<SalesInvTemplate>, SalesInvTemplate>
    {
        private static volatile SalesInvTemplateBM _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesInvTemplateBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesInvTemplateBM();
                    }
                }

                return _instance;
            }
        }
        private SalesInvTemplateBM():base(SalesInvTemplateDA.Instance){}
    }
}
