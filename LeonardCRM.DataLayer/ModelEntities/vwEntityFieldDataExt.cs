using System;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class vwEntityFieldData
    {
        public int MasterRecordId { get; set; }
        public string FieldData { get; set; }
        public int FieldDataId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
