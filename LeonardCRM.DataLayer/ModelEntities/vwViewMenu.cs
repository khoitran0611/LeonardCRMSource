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
    
    public partial class vwViewMenu
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ViewName { get; set; }
        public int ViewId { get; set; }
        public int Total { get; set; }
        public bool Shared { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public bool DefaultView { get; set; }
        public int SortOrder { get; set; }
        public string UserRole { get; set; }
    }
}
