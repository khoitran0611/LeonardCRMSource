using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.SalesRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer 
{
    public sealed class SalesOrderBM : BusinessBase<IRepository<SalesOrder>,SalesOrder>
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("ORDERS", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        private static volatile SalesOrderBM _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesOrderBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrderBM();
                    }
                }

                return _instance;
            }
        }
        public SalesOrderBM():base(SalesOrderDA.Instance){}

        public int CreateSalesOrders(SalesOrder order)
        {
            return SalesOrderDA.Instance.CreateSalesOrders(order);
        }

        public int UpdateSalesOrders(SalesOrder order, string documentPath = "")
        {
            return SalesOrderDA.Instance.UpdateSalesOrders(order, documentPath);
        }

        public int UpdateSalesOrderDeliveries(SalesOrder order)
        {
            return SalesOrderDA.Instance.UpdateSalesOrderDeliveries(order);
        }

        public IList<SalesOrder> GetRecentlyAddedOrders(int userId, int roleId, bool onlyMe)
        {
            return SalesOrderDA.Instance.GetRecentlyAddedOrders(userId, roleId, onlyMe);
        }

        public IList<GoogleGraph> GetReportDataByDays(int userId, int roleId, bool onlyMe, string dateFormat)
        {
            var graphs = new List<GoogleGraph>();

            foreach (var day in Enum.GetNames(typeof(OverviewReportOptions)))
            {
                var value = Enum.Parse(typeof(OverviewReportOptions), day)
                                .GetHashCode();
                if (value <= 0) continue;
                var data = SalesOrderDA.Instance.GetReportDataByDays(userId, roleId, onlyMe, value);
                var graph = new GoogleGraph
                {
                    rows =
                        data.Select(r =>
                            new DataPointSet
                            {
                                c = new[] {
                                                new DataPoint { v = value == (int)OverviewReportOptions.Last365Days?r.CreatedDate: DateTime.Parse(r.CreatedDate).ToString(dateFormat) },
                                                new DataPoint { v = r.Total.ToString(), f = string.Format(GetText("OVERVIEW_DATA_POINT_TEXT"), r.Total.ToString()) }
                                      }
                            }).ToArray()
                        ,
                    p = new Dictionary<string, string>()
                };
                graphs.Add(graph);
            }

            return graphs;
        }

        public GoogleGraph GetOrderReportDashboard(DateTime fromDate, DateTime toDate,
            int status, string idArray, string dateFormat)
        {
            var graph = new GoogleGraph();
            var dataSource = SalesOrderDA.Instance.GetOrderReportDashboard(fromDate, toDate, status, idArray);

            var users = dataSource.Select(r => r.Name).Distinct().ToList();
            var date = dataSource.Select(r => r.CreatedDate).Distinct().ToList();

            graph.cols = new ColInfo[users.Count + 1];
            graph.cols[0] = new ColInfo { id = "date", label = "Date", type = "string" };
            for (int i = 0; i < users.Count; i++)
            {
                graph.cols[i + 1] = new ColInfo { id = string.Format("user{0}", i), label = users[i], type = "number" };
            }
            var timespan = toDate - fromDate;
            var dataPointSetList = new List<DataPointSet>();
            foreach (var issueDate in date)
            {
                var dataPointSet = new DataPointSet();
                IList<DataPoint> list = new List<DataPoint>();
                list.Add(new DataPoint { v = timespan.TotalDays < 365 ? DateTime.Parse(issueDate).ToString(dateFormat) : issueDate });
                foreach (var user in users)
                {
                    var obj = dataSource.SingleOrDefault(r => r.CreatedDate == issueDate && r.Name == user);
                    var total = obj != null ? obj.Total : 0;
                    var dataPoint = new DataPoint { v = total.ToString(), f = total.ToString() };
                    list.Add(dataPoint);
                }
                dataPointSet.c = list.ToArray();
                dataPointSetList.Add(dataPointSet);
            }
            graph.rows = dataPointSetList.ToArray();
            return graph;
        }

        public int SaveOrderApi(SalesOrder model)
        {
            return SalesOrderDA.Instance.SaveOrderApi(model);
        }

        public NgTableModel GetApplicationByUser(int userId, int roleId, bool assistMode)
        {
            return SalesOrderDA.Instance.GetApplicationByUser(userId, roleId, assistMode);
        }

        public int CancelApplication(int appId)
        {
            return SalesOrderDA.Instance.CancelApplication(appId);
        }


        public static OverdueApps GetOverdueApp(int pageIndex, int pageSize, string sortDesc, int overdueMonth, FilterOverdueAppsParams filterParams)
        {
            return SalesOrderDA.Instance.GetOverdueApp(pageIndex, pageSize, sortDesc, overdueMonth, filterParams);
        }

        public int DeleteOverdueApp(int overdueMonth, int total, FilterOverdueAppsParams filterParams)
        {
            return SalesOrderDA.Instance.DeleteOverdueApp(overdueMonth, total, filterParams);
        }

        public bool CheckExpectedStatus(int status, int appId, out bool isExistOrder)
        {
            return SalesOrderDA.Instance.CheckExpectedStatus(status, appId, out isExistOrder);
        }

        public int SaveContractSignature(SalesOrder app)
        {
            return SalesOrderDA.Instance.SaveContractSignature(app);
        }

        public IList<Eli_User> GetSenderForCancelApp(int appId)
        {
            return SalesOrderDA.Instance.GetSenderForCancelApp(appId);
        }

        public bool CheckDriverAssigned(int[] assignedUserIds)
        {
            return SalesOrderDA.Instance.CheckDriverAssigned(assignedUserIds);
        }

        public int AssignAppToNewUser(Eli_User user, SalesOrder app)
        {
            return SalesOrderDA.Instance.AssignAppToNewUser(user, app);
        }

        public int CloneApp(int appId, int currentUserId)
        {
            return SalesOrderDA.Instance.CloneApp(appId, currentUserId);
        }

        public void SetRequireWaiverAndDocs(SalesCustomer customer, out bool isRequireWaiver, out int requiredDocNum)
        {
            isRequireWaiver = customer.ResidenceType == ResidenceType.Rent.GetHashCode() || customer.LandType == LandType.Rent.GetHashCode();
            requiredDocNum = 1;
            requiredDocNum += (isRequireWaiver ? 1 : 0) + (!string.IsNullOrWhiteSpace(customer.CoName) ? 1 : 0);
        }
    }
}
