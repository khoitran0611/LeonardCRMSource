using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.EntityFieldRepository
{
    public sealed class EntityFieldDA : EF5RepositoryBase<LeonardUSAEntities, Eli_EntityFields>
    {
        private static volatile EntityFieldDA _instance;
        private static readonly object SyncRoot = new Object();
        public static EntityFieldDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new EntityFieldDA();
                    }
                }

                return _instance;
            }
        }
        private EntityFieldDA() : base(Settings.ConnectionString) { }
        public IList<vwFieldsDataType> GetAllFields()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwFieldsDataTypes.AsNoTracking().ToList();
            }
        }
        public IList<vwViewColumn> GetFieldsByModuleId(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwViewColumns.AsNoTracking().Where(r => r.ModuleId == moduleId).ToList();
            }
        }
        public IList<Eli_EntityFields> GetManageFieldsByModuleId(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_EntityFields.AsNoTracking()
                              .Where(r => r.ModuleId == moduleId)
                              .OrderBy(r => r.SortOrder).ToList();
            }
        }
        public IList<vwViewColumn> GetAllViewColumns()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwViewColumns.OrderBy(r=> r.ModuleId).ThenBy(r=> r.RoleId)
                              .ThenBy(r=> r.SortOrder).ToList();
            }
        }
        public IList<vwEntityFieldData> GetCustomFieldByModuleId(int moduleId, int masterId, int roleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                // vwEntityFieldDatas view lieft join data Eli_FieldData table
                var customfields = context.vwEntityFieldDatas.Where(f => f.ModuleId == moduleId && f.RoleId == roleId
                                                                         && f.Visible.HasValue && f.Visible.Value)
                                          .OrderBy(r => r.ModuleId).ThenBy(r => r.RoleId)
                                          .ThenBy(r => r.SortOrder).ToList();

                var fieldIds = customfields.Select(r => r.FieldId).ToArray();
                var fielddatas = context.Eli_FieldData.Where(f => f.MaterRecordId == masterId 
                                                                && fieldIds.Contains(f.CustFieldId)).ToList();

                foreach (var fielddata in fielddatas)
                {
                    foreach (var customfield in customfields)
                    {
                        if (customfield.FieldId == fielddata.CustFieldId)
                        {
                            customfield.MasterRecordId = fielddata.MaterRecordId;
                            customfield.FieldDataId = fielddata.Id;
                            customfield.FieldData = fielddata.FieldData;
                            customfield.CreatedDate = fielddata.CreatedDate;
                            customfield.CreatedBy = fielddata.CreatedBy;
                            customfield.ModifiedDate = fielddata.ModifiedDate;
                            customfield.ModifiedBy = fielddata.ModifiedBy;
                            break;
                        }
                    }
                }

                // Remove 
                //foreach (var field in customfields)
                //{
                //    if (field.IsList || field.IsMultiSelecttBox)
                //    {
                //        field.ListValues = ListValueDA.Instance.Find(f => f.ListNameId == field.ListNameId).OrderBy(v => v.ListOrder).ToList();
                //    }
                //}
                return customfields;
            }
        }
        public bool CheckEntityFieldIsUsing(int id)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var result = context.Eli_ViewOrderBy.Any(r => r.FieldId == id) ||
                             context.Eli_ViewConditions.Any(r => r.FieldId == id)||
                             context.Eli_ViewGroupBy.Any(r => r.FieldId == id);
                return result;
            }
        }
        public IList<vwField> GetFields()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwFields.ToList();
            }
        }
        public IList<Eli_EntityFields> GetAllCustFieldByModule(int moduleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_EntityFields.Where(f => f.ModuleId == moduleId && f.Deletable.HasValue && f.Deletable == true && f.IsActive).ToList();
            }
        }
    }
}
