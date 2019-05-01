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
    
    public partial class SalesOrderDelivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DeliveryType { get; set; }
        public Nullable<System.DateTime> DeliveryDate1 { get; set; }
        public Nullable<System.DateTime> DeliveryDate2 { get; set; }
        public Nullable<int> DeliveryTime { get; set; }
        public Nullable<decimal> MilesToSite { get; set; }
        public bool CustomerPresent { get; set; }
        public Nullable<int> LoadDoorFacing { get; set; }
        public string MoveFromAddress { get; set; }
        public string MoveToAddress { get; set; }
        public string DeliveryRequirement { get; set; }
        public string DirectionToSite { get; set; }
        public string CustomerInitials { get; set; }
        public Nullable<System.DateTime> InitialDate { get; set; }
        public string CustomerSignature { get; set; }
        public Nullable<System.DateTime> CustomerSignDate { get; set; }
        public string CustomerSignIP { get; set; }
        public Nullable<bool> WaiverAccepted { get; set; }
        public Nullable<bool> CustomerAccepted { get; set; }
        public string DriverName { get; set; }
        public string DriverSignature { get; set; }
        public Nullable<System.DateTime> DriverSignDate { get; set; }
        public string DriverSignIP { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual SalesOrder SalesOrder { get; set; }
    }
}
