using System;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public class ReportObject
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Status { get; set; }
        public int[] UserIds { get; set; }
        public bool ByClient { get; set; }
        public int Currency { get; set; }
    }
}
