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
    
    public partial class Eli_FieldsSectionDetail
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int FieldId { get; set; }
        public byte SortOrder { get; set; }
        public bool LeftSide { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Eli_EntityFields Eli_EntityFields { get; set; }
        public virtual Eli_FieldsSection Eli_FieldsSection { get; set; }
    }
}
