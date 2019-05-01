//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeonardCRM.DataLayer.ModelEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Eli_EntityFields
    {
        public Eli_EntityFields()
        {
            this.Eli_FieldData = new HashSet<Eli_FieldData>();
            this.Eli_FieldsSectionDetail = new HashSet<Eli_FieldsSectionDetail>();
            this.Eli_RolesFields = new HashSet<Eli_RolesFields>();
            this.Eli_ViewCustomColumns = new HashSet<Eli_ViewCustomColumns>();
        }
    
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string Description { get; set; }
        public string LabelDisplay { get; set; }
        public int ModuleId { get; set; }
        public int DataTypeId { get; set; }
        public Nullable<int> MinLength { get; set; }
        public Nullable<int> DataLength { get; set; }
        public int SortOrder { get; set; }
        public bool Mandatory { get; set; }
        public Nullable<int> ListNameId { get; set; }
        public string ListSql { get; set; }
        public Nullable<bool> AdvanceSearch { get; set; }
        public string DefaultValue { get; set; }
        public Nullable<bool> Deletable { get; set; }
        public Nullable<bool> Searchable { get; set; }
        public Nullable<bool> Sortable { get; set; }
        public Nullable<bool> AllowGroup { get; set; }
        public bool Display { get; set; }
        public bool IsUnique { get; set; }
        public bool PrimaryKey { get; set; }
        public Nullable<bool> ForeignKey { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsWebform { get; set; }
        public Nullable<bool> IsTextShow { get; set; }
        public string RegularExpression { get; set; }
        public Nullable<int> Point { get; set; }
        public bool IsLoadReference { get; set; }
    
        public virtual Eli_DataTypes Eli_DataTypes { get; set; }
        public virtual Eli_Modules Eli_Modules { get; set; }
        public virtual ICollection<Eli_FieldData> Eli_FieldData { get; set; }
        public virtual ICollection<Eli_FieldsSectionDetail> Eli_FieldsSectionDetail { get; set; }
        public virtual ICollection<Eli_RolesFields> Eli_RolesFields { get; set; }
        public virtual ICollection<Eli_ViewCustomColumns> Eli_ViewCustomColumns { get; set; }
    }
}
