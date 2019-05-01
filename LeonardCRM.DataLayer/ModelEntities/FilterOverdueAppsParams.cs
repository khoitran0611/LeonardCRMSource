using Eli.Common;
using System;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public class FilterOverdueAppsParams
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string PartNumber { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CapitalizationPeriod { get; set; }
    }
}
