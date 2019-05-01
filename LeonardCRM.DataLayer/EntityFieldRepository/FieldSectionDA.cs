using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class FieldSectionDA : EF5RepositoryBase<LeonardUSAEntities, Eli_FieldsSection>
    {
        private static volatile FieldSectionDA _instance;
        private static readonly object SyncRoot = new Object();
        public static FieldSectionDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new FieldSectionDA();
                    }
                }

                return _instance;
            }
        }
        private FieldSectionDA() : base(Settings.ConnectionString) { }

        public IList<Eli_FieldsSection> GetFieldSectionByModuleId(int moduleId)
        {
            using (var content = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return content.Eli_FieldsSection.Include("Eli_FieldsSectionDetail")
                              .Where(r => r.ModuleId == moduleId).ToList();
            }
        }

        public int SaveObjs(IList<Eli_FieldsSection> models,int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                // Insert
                var newSections = models.Where(r => r.Id == 0).ToList();
                foreach (var section in newSections)
                {
                    var newItems = section.Eli_FieldsSectionDetail.ToList();
                    section.Eli_FieldsSectionDetail.Clear();
                    context.Eli_FieldsSection.Attach(section);
                    foreach (var item in newItems)
                    {
                        item.SectionId = section.Id;
                        section.Eli_FieldsSectionDetail.Add(item);
                    }
                    context.Eli_FieldsSection.Add(section);
                }

                // Update
                var updateSections = models.Where(r => r.Id > 0).ToList();
                foreach (var section in updateSections)
                {
                    var detailItems = section.Eli_FieldsSectionDetail.ToList();
                    section.Eli_FieldsSectionDetail.Clear();
                    var newItems = detailItems.Where(r => r.Id == 0).ToList();
                    var updateItems = detailItems.Where(r => r.Id > 0).ToList();
                    var itemIds = updateItems.Select(r => r.Id).ToArray();
                    var deleteItems = context.Eli_FieldsSectionDetail.Where(r => r.SectionId == section.Id 
                                                                            && !itemIds.Contains(r.Id)).ToList();
                    foreach (var item in updateItems)
                    {
                        item.SectionId = section.Id;
                        context.Eli_FieldsSectionDetail.Attach(item);
                        context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    foreach (var item in deleteItems)
                    {
                        context.Eli_FieldsSectionDetail.Remove(item);
                    }
                    context.Eli_FieldsSection.Attach(section);
                    foreach (var item in newItems)
                    {
                        item.SectionId = section.Id;
                        section.Eli_FieldsSectionDetail.Add(item);
                    }
                    context.Entry(section).State = System.Data.Entity.EntityState.Modified;
                }


                // Delete
                var ids = updateSections.Select(r => r.Id).ToArray();
                var deleteSections =
                    context.Eli_FieldsSection.Where(r => r.ModuleId == moduleId && !ids.Contains(r.Id)).ToList();
                foreach (var section in deleteSections)
                {
                    context.Eli_FieldsSection.Remove(section);
                }

                return context.SaveChanges();
            }
        }
    }
}
