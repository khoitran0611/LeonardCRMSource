using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class ListNameDA : EF5RepositoryBase<LeonardUSAEntities, Eli_ListNames>
    {
        private static volatile ListNameDA _instance;
        private static readonly object SyncRoot = new Object();
        public static ListNameDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ListNameDA();
                    }
                }

                return _instance;
            }
        }
        private ListNameDA() : base(Settings.ConnectionString) { }

        public IList<vwListNameValue> GetListNameValuesByModuleId(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwListNameValues.AsNoTracking().Where(v => v.ModuleId == moduleId && v.Active).OrderBy(v=>v.ListOrder).ThenBy(v=>v.ListOrder).ToList();
            }
        }

        public IList<vwListNameValue> GetListNameValuesByModules(int[] modules)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwListNameValues.AsNoTracking().Where(v => modules.Contains(v.ModuleId) && v.Active).OrderBy(v => v.ListNameId).ThenBy(v => v.ListOrder).ToList();
            }
        }

        public IList<vwListNameValue> GetListNameValuesByModuleId(int moduleId, string listName)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                int listNameId = context.Eli_ListNames.Single(r => r.ListName == listName).Id;
                return context.vwListNameValues.AsNoTracking()
                               .Where(record => record.ModuleId == moduleId &&
                                record.ListNameId == listNameId && record.Active).OrderBy(v => v.ListNameId).ThenBy(v => v.ListOrder).ToList();
            }
        }

        public IList<GetReferenceListValues_Result> GetReferenceListsByModule(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.GetReferenceListValues(moduleId).ToList();
            }
        }

        public IList<GetReferenceListValues_Result> GetReferenceListsByModules(int[] modules)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var list = new List<GetReferenceListValues_Result>();
                foreach (var module in modules)
                {
                    list.AddRange(context.GetReferenceListValues(module));
                }
                return list;
            }
        }

        public int AddListName(Eli_ListNames listName)
        {
            var status = 0;
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                try
                {
                    var listValues = listName.Eli_ListValues.ToList();
                    listName.Eli_ListValues.Clear();
                    context.Eli_ListNames.Attach(listName);
                    context.Eli_ListNames.Add(listName);
                    status = context.SaveChanges();

                    if (status > 0 && listValues.Count > 0)
                    {
                        foreach (var listvalue in listValues)
                        {
                            listvalue.ListNameId = listName.Id;
                            context.Eli_ListValues.Attach(listvalue);
                            context.Eli_ListValues.Add(listvalue);
                        }
                        context.SaveChanges();
                    }
                }
                catch (Exception exception)
                {
                    if (listName.Id > 0)
                    {
                        context.Eli_ListNames.Remove(listName);
                    }
                }
            }
            return status;
        }

        public int UpdateListName(Eli_ListNames listName)
        {
            int status = 0;

            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                try
                {
                    var addedListValues = listName.Eli_ListValues.Where(l => l.Id == 0).ToList();
                    var updateListValues = listName.Eli_ListValues.Where(l => l.Id != 0).ToList();
                    listName.Eli_ListValues.Clear();

                    context.Eli_ListNames.Attach(listName);
                    context.Entry(listName).State = System.Data.Entity.EntityState.Modified;
                    //update list value
                    foreach (var listvalue in updateListValues)
                    {
                        context.Entry(listvalue).State = System.Data.Entity.EntityState.Modified;
                    }

                    //add new list value
                    foreach (var listvalue in addedListValues)
                    {
                        context.Eli_ListValues.Attach(listvalue);
                        context.Eli_ListValues.Add(listvalue);
                    }

                    status = context.SaveChanges();
                }
                catch
                {
                }
            }

            return status;
        }
        public IList<vwListNameValue> GetAllListNameValues()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwListNameValues.AsNoTracking().ToList();
            }
        }
    }
}
