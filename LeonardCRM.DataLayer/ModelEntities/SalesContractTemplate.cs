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
    
    public partial class SalesContractTemplate
    {
        public SalesContractTemplate()
        {
            this.SalesContractStates = new HashSet<SalesContractState>();
        }
    
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual ICollection<SalesContractState> SalesContractStates { get; set; }
    }
}
