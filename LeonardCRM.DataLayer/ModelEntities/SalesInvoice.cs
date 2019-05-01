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
    
    public partial class SalesInvoice
    {
        public SalesInvoice()
        {
            this.SalesInvServices = new HashSet<SalesInvService>();
            this.SalesInvTaxes = new HashSet<SalesInvTax>();
        }
    
        public int Id { get; set; }
        public int InvTemplateId { get; set; }
        public int Status { get; set; }
        public Nullable<int> OrderId { get; set; }
        public System.DateTime IssuedDate { get; set; }
        public string Details { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string ResponsibleUsers { get; set; }
    
        public virtual Eli_Currency Eli_Currency { get; set; }
        public virtual SalesInvTemplate SalesInvTemplate { get; set; }
        public virtual ICollection<SalesInvService> SalesInvServices { get; set; }
        public virtual ICollection<SalesInvTax> SalesInvTaxes { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }
    }
}