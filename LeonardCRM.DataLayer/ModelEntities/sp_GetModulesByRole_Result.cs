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
    
    public partial class sp_GetModulesByRole_Result
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FeatureName { get; set; }
        public string Description { get; set; }
        public bool Dashboard { get; set; }
        public string DefaultTable { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public int Parent { get; set; }
        public bool IsPublished { get; set; }
        public Nullable<int> DefaultViewId { get; set; }
        public bool NeedPickList { get; set; }
        public bool ReportModule { get; set; }
        public string MenuIcon { get; set; }
        public bool AllowCreateView { get; set; }
        public bool AllowImport { get; set; }
        public bool AllowExport { get; set; }
    }
}
