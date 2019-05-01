using System;
using Eli.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using System.Collections.Generic;
using System.IO;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesDocumentsDA : EF5RepositoryBase<LeonardUSAEntities,SalesDocument>
    {
        private static volatile SalesDocumentsDA _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesDocumentsDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new SalesDocumentsDA();
                }
                return _instance;
            }
        }
        public SalesDocumentsDA():base(Settings.ConnectionString){}

        public int SaveAttachment(int appId, List<SalesDocument> attachment, string folderPath, bool isOnlyAdd)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var postedAttachment = attachment.FindAll(x => x.OrderId == appId);

                var addedAttachment = postedAttachment.Where(x => x.Id == 0);

                foreach (var item in addedAttachment)
                {
                    _context.Entry(item).State = System.Data.Entity.EntityState.Added;
                }

               
                IList<SalesDocument> deletedAttachment = null;
                if(!isOnlyAdd)
                {
                    var postedIds = postedAttachment.Select(x => x.Id);
                    deletedAttachment = _context.SalesDocuments.Where(x => x.OrderId == appId && !postedIds.Any(c => c == x.Id)).ToList();
                    foreach (var item in deletedAttachment)
                    {
                        _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                }

                var status = _context.SaveChanges();

                if (status > 0)
                {
                    if(deletedAttachment != null)
                    {
                        foreach (var item in deletedAttachment)
                        {
                            var url = folderPath + "\\" + Path.GetFileName(item.FileName);
                            if (File.Exists(url))
                            {
                                File.Delete(url);
                            }
                        }
                    }                    
                }

                return status;
            }
        }
    }
}
