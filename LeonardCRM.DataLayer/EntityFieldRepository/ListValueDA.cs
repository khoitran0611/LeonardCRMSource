using System;
using System.Collections;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class ListValueDA : EF5RepositoryBase<LeonardUSAEntities, Eli_ListValues>
    {
        private static volatile ListValueDA _instance;
        private static readonly object SyncRoot = new Object();
        public static ListValueDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ListValueDA();
                    }
                }

                return _instance;
            }
        }
        private ListValueDA():base(Settings.ConnectionString){}

        public Hashtable GetListValueByFieldIds(int masterFieldId, int childFieldId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var masterField = context.Eli_EntityFields.SingleOrDefault(r => r.Id == masterFieldId && r.IsActive);
                var childField = context.Eli_EntityFields.SingleOrDefault(r => r.Id == childFieldId && r.IsActive);
                if (masterField == null || childField == null) return null;
                var masterModels = context.Eli_ListValues.Where(r => r.ListNameId == masterField.ListNameId && r.Active).ToList();
                var childModels = context.Eli_ListValues.Where(r => r.ListNameId == childField.ListNameId && r.Active).ToList();
                foreach (var item in masterModels)
                {
                    item.Selected = true;
                }
                foreach (var item in childModels)
                {
                    item.Selected = true;
                }
                return new Hashtable
                {
                    {"SourceValues", masterModels},
                    {"TargetValues", childModels}
                };
            }
        }
    }
}
