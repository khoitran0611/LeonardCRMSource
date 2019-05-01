using System.Collections;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class SalesCustomer : CustomField
    {
        public int ModuleId { get; set; }
        public IList<Eli_FieldData> FieldData { get; set; }
        public IList<Eli_Notes> Notes { get; set; }
        public int[] UserIds { get; set; }

        public IList<RelateView> RelateViews { get; set; }

        public bool IsDuplicated { get; set; }
        public bool Editable { get; set; }

        public List<string> AttachmentFiles { get; set; }

        public bool IsCustomer { get; set; }
        public bool IsDeliveryPerson { get; set; }
        public bool IsStore { get; set; }

        public string ClientIP { get; set; }
    }
}
