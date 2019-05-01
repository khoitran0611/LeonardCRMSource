using System;
using System.Collections;
using System.Collections.Generic;
using LeonardCRM.DataLayer.EntityFieldRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class ListValueBM : BusinessBase<IRepository<Eli_ListValues>, Eli_ListValues>
    {
        private static volatile ListValueBM _instance;
        private static readonly object SyncRoot = new Object();
        public static ListValueBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ListValueBM();
                    }
                }

                return _instance;
            }
        }
        private ListValueBM()
            : base(ListValueDA.Instance)
        { }

        public Hashtable GetListValueByFieldIds(int masterFieldId,int childFieldId)
        {
            return ListValueDA.Instance.GetListValueByFieldIds(masterFieldId, childFieldId);
        }
    }
}
