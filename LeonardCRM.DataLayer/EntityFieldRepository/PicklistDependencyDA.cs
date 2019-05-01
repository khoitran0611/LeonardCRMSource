using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class PicklistDependencyDA : EF5RepositoryBase<LeonardUSAEntities, Eli_ListDependency>
    {
        private static volatile PicklistDependencyDA _instance;
        private static readonly object SyncRoot = new Object();
        public static PicklistDependencyDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new PicklistDependencyDA();
                    }
                }

                return _instance;
            }
        }
        private PicklistDependencyDA() : base(Settings.ConnectionString) { }

        public Eli_ListDependency GetObjById(int modelId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var model = context.Eli_ListDependency.SingleOrDefault(r => r.Id == modelId);
                if (model != null)
                {
                    model.Eli_ListDependencyDetail =
                        context.Eli_ListDependencyDetail.Where(r => r.ParentId == modelId).ToList();
                }
                else
                {
                    model = new Eli_ListDependency();
                }
                return model;
            }
        }

        public int SaveObject(Eli_ListDependency model)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var items = model.Eli_ListDependencyDetail.ToList();
                model.Eli_ListDependencyDetail.Clear();
                var newItems = items.Where(r => r.Id == 0).ToList();

                context.Eli_ListDependency.Attach(model);
                // Insert
                foreach (var item in newItems)
                {
                    model.Eli_ListDependencyDetail.Add(item);
                }
                if (model.Id > 0)
                {
                    var updateItems = items.Where(r => r.Id > 0).ToList();
                    var ids = updateItems.Select(r => r.Id).ToArray();
                    var deleteItems = context.Eli_ListDependencyDetail.Where(r => r.ParentId == model.Id &&
                                                                                  !ids.Contains(r.Id)).ToList();
                    foreach (var item in deleteItems)
                    {
                        context.Eli_ListDependencyDetail.Remove(item);
                    }
                    context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.Eli_ListDependency.Add(model);
                }
                return context.SaveChanges();
            }
        }

        public IList<vwPicklistDependency> GetPicklistDependenciesByModuleId(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwPicklistDependencies.Where(r => r.ModuleId == moduleId).ToList();
            }
        }
    }
}
