using System;
using System.Collections;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class SalesOrderComplete : CustomField
    {
        public bool IsPaidFull
        {
            get { return !BalanceDue.HasValue; }
        }

        public string CustomerSignatureUrl { get; set; }

        public string DeliverySignatureUrl { get; set; }

        public string CallTime { get; set; }

        public string ManagerSignatureUrl { get; set; }
    }
}
