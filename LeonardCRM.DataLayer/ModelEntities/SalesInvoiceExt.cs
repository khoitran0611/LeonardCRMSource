using System.Collections;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class SalesInvoice :CustomField
    {
        public IList<SalesInvService> Services { get; set; }
        public IList<SalesInvTax> Taxes { get; set; }
        public IList<Eli_Currency> Currencies { get; set; }
        public string ServiceLine { get; set; }
        public IList<Eli_FieldData> FieldData { get; set; }
        public int UserIds { get; set; }
        public int ModuleId { get; set; }
        public IList<Eli_Notes> Notes { get; set; }
    }
}
