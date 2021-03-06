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
    
    public partial class Eli_ViewCustom
    {
        public Eli_ViewCustom()
        {
            this.Eli_ViewCustomColumns = new HashSet<Eli_ViewCustomColumns>();
            this.Eli_ViewCustomConditions = new HashSet<Eli_ViewCustomConditions>();
        }
    
        public int Id { get; set; }
        public string ViewName { get; set; }
        public bool IsPublic { get; set; }
        public bool DefaultView { get; set; }
        public string SelectClause { get; set; }
        public string FromClause { get; set; }
        public string WhereClause { get; set; }
        public string DefaultFilterStr { get; set; }
        public Nullable<int> PageSize { get; set; }
        public string OrderBy { get; set; }
        public string GroupBy { get; set; }
        public Nullable<int> MasterViewId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string Code { get; set; }
    
        public virtual ICollection<Eli_ViewCustomColumns> Eli_ViewCustomColumns { get; set; }
        public virtual ICollection<Eli_ViewCustomConditions> Eli_ViewCustomConditions { get; set; }
        public virtual Eli_User Eli_User { get; set; }
    }
}
