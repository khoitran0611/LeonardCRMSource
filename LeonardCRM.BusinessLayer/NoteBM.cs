using System;
using System.Collections.Generic;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class NoteBM : BusinessBase<IRepository<Eli_Notes>, Eli_Notes>
    {
        private static volatile NoteBM _instance;
        private static readonly object SyncRoot = new Object();

        public static NoteBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new NoteBM();
                    }
                }

                return _instance;
            }
        }

        public NoteBM()
            : base(NoteDA.Instance)
        {
        }

        public IList<Eli_Notes> GetNoteByRecordId(int moduleId, int recordId, bool? isActive)
        {
            return NoteDA.Instance.GetNoteByRecordId(moduleId, recordId, isActive);
        }

        public int SaveNote(Eli_Notes entity)
        {
            return NoteDA.Instance.SaveNote(entity);
        }

        public int DeleteNote(int id, int moduleId)
        {
            return NoteDA.Instance.DeleteNote(id, moduleId);
        }
    }
}
