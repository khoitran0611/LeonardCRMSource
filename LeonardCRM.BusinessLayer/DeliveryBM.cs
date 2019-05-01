using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeonardCRM.BusinessLayer
{
    public class DeliveryBM
    {
        private SalesOrderBM _salesOrderBM;
        private SalesOrderCompleteBM _salesOrderCompleteBM;

        public DeliveryBM()
        {
            _salesOrderBM = SalesOrderBM.Instance;
            _salesOrderCompleteBM = SalesOrderCompleteBM.Instance;
        }

        public DeliveryInfo GetWeeklyDeliveryInfo( DateTime fromDate, DateTime toDate)
        {
            DeliveryInfo weeklyDeliveryInfo = null;

            var data = _salesOrderBM.GetAll().Join(_salesOrderCompleteBM.GetAll(), o => o.Id, oc => oc.OrderId,
                         (o, oc) => new {
                             OrderId = o.Id,
                             DeliveryDate = oc.DeliveryDate,
                             SerialNumber = o.SerialNumber,
                             ResponsibleUsers = o.ResponsibleUsers,
                             Status = o.Status
                         });

            data = data.Where(o => o.Status == (int)OrderStatus.Completed);

            foreach(var d in data)
            {
                var hasDeliveryUser = GetDeliverymanUserId(d.ResponsibleUsers) != null;
            }
            
            return weeklyDeliveryInfo;
        }

        /// <summary>
        /// Assume that only 1 deliveryman is allowed for 1 order
        /// </summary>
        /// <param name="resposibleUserIds"></param>
        /// <returns></returns>
        private Eli_User GetDeliverymanUserId(string resposibleUserIds)
        {
            int deliveryManUserId = 0;

            if(!string.IsNullOrEmpty(resposibleUserIds)) {
                var resposibleUsers = resposibleUserIds.Split(',');

                foreach(var id in resposibleUsers)
                {
                    if(int.TryParse(id, out deliveryManUserId))
                    {
                        var user = UserBM.Instance.GetById(deliveryManUserId);

                        if(user != null)
                        {
                            if(user.RoleId == (int)UserRoles.DeliveryStaff)
                            {
                                return user;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
