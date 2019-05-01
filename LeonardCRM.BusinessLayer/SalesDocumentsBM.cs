using System;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.SalesRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;
using System.Collections.Generic;

namespace LeonardCRM.BusinessLayer
{
    public sealed class SalesDocumentsBM :BusinessBase<IRepository<SalesDocument>,SalesDocument>
    {
        private static volatile SalesDocumentsBM _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesDocumentsBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesDocumentsBM();
                    }
                }

                return _instance;
            }
        }
        public SalesDocumentsBM():base(SalesDocumentsDA.Instance){}

        public int SaveAttachment(int appId, List<SalesDocument> attachment, string folderPath, bool isOnlyAdd)
        {
            return SalesDocumentsDA.Instance.SaveAttachment(appId, attachment, folderPath, isOnlyAdd);
        }
    }
}
