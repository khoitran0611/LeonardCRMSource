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
    public sealed class SalesCustomerBM : BusinessBase<IRepository<SalesCustomer>, SalesCustomer>
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("SALES_CUSTOMER", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }
        private static volatile SalesCustomerBM _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesCustomerBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesCustomerBM();
                    }
                }

                return _instance;
            }
        }
        private SalesCustomerBM()
            : base(SalesCustomerDA.Instance)
        { }

        public int SaveSalesCustomer(SalesCustomer entity, string attachmentFolder)
        {
            return SalesCustomerDA.Instance.SaveSalesCustomer(entity, attachmentFolder);
        }

        public IList<SalesCustomer> GetRecentlyAddedCustomers(int userId, int roleId, bool onlyMe)
        {
            return SalesCustomerDA.Instance.GetRecentlyAddedCustomers(userId, roleId, onlyMe);
        }

        public IList<GoogleGraph> GetReportDataByDays(int userId, int roleId, bool onlyMe, string dateFormat)
        {
            var graphs = new List<GoogleGraph>();

            foreach (var day in Enum.GetNames(typeof(OverviewReportOptions)))
            {
                var value = Enum.Parse(typeof(OverviewReportOptions), day)
                                .GetHashCode();
                if (value <= 0) continue;
                var data = SalesCustomerDA.Instance.GetReportDataByDays(userId, roleId, onlyMe, value);
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

        public GoogleGraph GetReportDataByUsers(int?[] responsibleUsers)
        {
            var data = SalesCustomerDA.Instance.GetReportDataByUsers(responsibleUsers);
            var uniqueData = data.Select(x => x.UserName).Distinct();
            var graph = new GoogleGraph();
            var dataPointSet = new List<DataPointSet>();
            foreach (var user in uniqueData)
            {
                var count = data.Count(u => u.UserName == user);
                var customers = string.Join("</li><li>", data.Where(x => x.UserName == user).Select(x => x.CustomerName));
                dataPointSet.Add(
                        new DataPointSet
                        {
                            c = new[] {
                                                new DataPoint { v = user },
                                                new DataPoint { v = count.ToString(), f = count.ToString()},
                                                new DataPoint { f = "<b>" + user + "</b> "+ GetText("CHART_RESPONSIBLE_FOR_TOOLTIP") +" <br><ul><li>"+customers + "</li></ul>"}
                                      }
                        });
            }
            graph.rows = dataPointSet.ToArray();
            graph.p = new Dictionary<string, string>();
            return graph;
        }

        public IList<vwClient> GetAllClients(int currentRole, int userId)
        {
            return SalesCustomerDA.Instance.GetAllClients(currentRole, userId);
        }

        public int SaveCustomerApi(SalesCustomer model)
        {
            return SalesCustomerDA.Instance.SaveCustomerApi(model);
        }

        public IList<vwAllCustomer> GetAllCustomers(int currentRole, int userId)
        {
            return SalesCustomerDA.Instance.GetAllCustomers(currentRole, userId);
        }

        public int GetIdByEmail(string email)
        {
            return SalesCustomerDA.Instance.GetIdByEmail(email);
        }


        public SalesCustomer GetApplicantById(int appId)
        {
            return SalesCustomerDA.Instance.GetApplicantById(appId);
        }

        public string GetContractContent(int appId, string promotionText, bool onlyGetBody, bool isGetImageBase64, string signatureFolder)
        {
            return SalesCustomerDA.Instance.GetContractContent(appId, promotionText, onlyGetBody, isGetImageBase64, signatureFolder);

        }

        public string GetCustomerEmail(int appId)
        {
            return SalesCustomerDA.Instance.GetCustomerEmail(appId);
        }
    }
}
