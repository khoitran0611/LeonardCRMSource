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
    
    public partial class Eli_ListNames
    {
        public Eli_ListNames()
        {
            this.Eli_ListValues = new HashSet<Eli_ListValues>();
        }
    
        public int Id { get; set; }
        public string ListName { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<Eli_ListValues> Eli_ListValues { get; set; }
    }
}