using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class NoteDA : EF5RepositoryBase<LeonardUSAEntities, Eli_Notes>
    {
        private static volatile NoteDA _instance;
        private static readonly object SyncRoot = new Object();

        public static NoteDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new NoteDA();
                    }
                }

                return _instance;
            }
        }

        private NoteDA()
            : base(Settings.ConnectionString)
        {
        }

        public IList<Eli_Notes> GetNoteByRecordId(int moduleId, int recordId, bool? isActive)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                IQueryable<Eli_Notes> result = context.Eli_Notes.Where(r => r.ModuleId == moduleId && r.RecordId == recordId);

                if (isActive != null)
                    result = result.Where(r => r.IsActive == isActive);
                return result.OrderByDescending(r => r.NoteDate).ToList();
            }
        }

        public int SaveNote(Eli_Notes entity)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (entity.Id > 0)
                {
                    context.Eli_Notes.Attach(entity);
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.Eli_Notes.Add(entity);
                }
                return context.SaveChanges();
            }
        }
        public int DeleteNote(int id, int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var entity = context.Eli_Notes.SingleOrDefault(r => r.Id == id && r.ModuleId == moduleId);
                if (entity != null)
                {
                    context.Eli_Notes.Remove(entity);
                }
                return context.SaveChanges();
            }
        }
    }
}
