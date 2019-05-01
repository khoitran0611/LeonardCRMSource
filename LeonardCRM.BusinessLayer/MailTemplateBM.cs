using System;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class MailTemplateBM : BusinessBase<IRepository<Eli_MailTemplates>, Eli_MailTemplates>
    {
        private static volatile MailTemplateBM _instance;
        private static readonly object SyncRoot = new Object();
        public static MailTemplateBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new MailTemplateBM();
                    }
                }

                return _instance;
            }
        }
        private MailTemplateBM()
            : base(MailTemplateDA.Instance)
        {}
    }
}
