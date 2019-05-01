using System;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.SalesRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer 
{
    public sealed class SalesContractTemplateBM : BusinessBase<IRepository<SalesContractTemplate>, SalesContractTemplate>
    {
        private static volatile SalesContractTemplateBM _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesContractTemplateBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesContractTemplateBM();
                    }
                }

                return _instance;
            }
        }
        private SalesContractTemplateBM() : base(SalesContractTemplateDA.Instance) { }

        public SalesContractTemplate GetContractTmplById(int templateId)
        {
            return SalesContractTemplateDA.Instance.GetContractTmplById(templateId);
        }

        public int UpdateContractTemplate(SalesContractTemplate contractTemplate)
        {
            return SalesContractTemplateDA.Instance.GetContractTmplById(contractTemplate);
        }
    }
}
