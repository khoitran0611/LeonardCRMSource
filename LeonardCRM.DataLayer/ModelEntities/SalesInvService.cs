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
    
    public partial class SalesInvService
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public decimal Cost { get; set; }
        public Nullable<int> InvId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual SalesInvoice SalesInvoice { get; set; }
    }
}