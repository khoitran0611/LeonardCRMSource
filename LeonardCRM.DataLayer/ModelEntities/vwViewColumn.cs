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
    
    public partial class vwViewColumn
    {
        public int FieldId { get; set; }
        public string ColumnName { get; set; }
        public int ModuleId { get; set; }
        public int SortOrder { get; set; }
        public bool IsDate { get; set; }
        public bool IsList { get; set; }
        public bool IsMultiSelecttBox { get; set; }
        public bool IsCheckBox { get; set; }
        public Nullable<int> ListNameId { get; set; }
        public string LabelDisplay { get; set; }
        public Nullable<bool> ForeignKey { get; set; }
        public string ListSql { get; set; }
        public Nullable<bool> AdvanceSearch { get; set; }
        public bool Display { get; set; }
        public bool IsDateTime { get; set; }
        public bool IsDecimal { get; set; }
        public bool IsInteger { get; set; }
        public Nullable<bool> Sortable { get; set; }
        public bool IsCurrency { get; set; }
        public Nullable<bool> AllowGroup { get; set; }
        public int RoleId { get; set; }
        public bool Locked { get; set; }
        public Nullable<int> Point { get; set; }
        public Nullable<bool> Visible { get; set; }
        public Nullable<bool> IsTextShow { get; set; }
        public bool Mandatory { get; set; }
        public bool IsLoadReference { get; set; }
    }
}
