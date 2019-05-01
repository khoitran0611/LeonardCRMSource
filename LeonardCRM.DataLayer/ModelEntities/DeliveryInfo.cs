using System;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public class WeeklyDelivery
    {
        public Eli_User Deliveryman { get; set; }
        public List<DeliveryInfo> DeliveryInfos { get; set; }
    }

    public class DeliveryInfo
    {
        public DateTime DeliveryDate { get; set; }
        public string SerialNumber { get; set; }
    }
}
