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
    public sealed class SalesInvoiceBM : BusinessBase<IRepository<SalesInvoice>, SalesInvoice>
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("INVOICES", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }
        private static volatile SalesInvoiceBM _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesInvoiceBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesInvoiceBM();
                    }
                }

                return _instance;
            }
        }
        private SalesInvoiceBM()
            : base(SalesInvoiceDA.Instance)
        { }

        public int SaveInvoice(SalesInvoice invoice)
        {
            return SalesInvoiceDA.Instance.SaveInvoice(invoice);
        }

        public int UpdateInvoice(SalesInvoice invoice)
        {
            return SalesInvoiceDA.Instance.UpdateInvoice(invoice);
        }

        public IList<SalesInvoice> GetRecentlyAddedInvoices(int userId, int roleId, bool onlyMe)
        {
            return SalesInvoiceDA.Instance.GetRecentlyAddedInvoices(userId, roleId, onlyMe);
        }


        public IList<GoogleGraph> GetReportDataByDays(int userId, int roleId, bool onlyMe, string dateFormat)
        {
            var graphs = new List<GoogleGraph>();
            foreach (var day in Enum.GetNames(typeof(OverviewReportOptions)))
            {
                var value = Enum.Parse(typeof(OverviewReportOptions), day)
                                .GetHashCode();
                if (value <= 0) continue;
                var data = SalesInvoiceDA.Instance.GetReportDataByDays(userId, roleId, onlyMe, value);
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

        public IList<GoogleGraph> GetReportDataPerDays(int userId, int roleId, bool onlyMe, string dateFormat, int currencyId)
        {
            var currency = CurrencyBM.Instance.GetById(currencyId).Symbol;
            var graphs = new List<GoogleGraph>();

            foreach (var day in Enum.GetNames(typeof(OverviewReportOptions)))
            {
                var value = Enum.Parse(typeof(OverviewReportOptions), day)
                                .GetHashCode();
                if (value <= 0) continue;
                var data = SalesInvoiceDA.Instance.GetReportDataPerDays(userId, roleId, onlyMe, value, currencyId);
                var graph = new GoogleGraph
                {
                    rows =
                        data.Select(r =>
                            new DataPointSet
                            {
                                c = new[] {
                                                new DataPoint { v = value == (int)OverviewReportOptions.Last365Days ? r.Date: DateTime.Parse(r.Date).ToString(dateFormat) },
                                                new DataPoint { v = r.TotalNew.ToString(), f = string.Format("{0}{1}",currency, r.TotalNew.Value.ToString("C").Replace("$","")) },
                                                new DataPoint { v = r.TotalPaid.ToString(), f = string.Format("{0}{1}",currency, r.TotalPaid.Value.ToString("C").Replace("$","")) }
                                      }
                            }).ToArray()
                        ,
                    p = new Dictionary<string, string>()
                };
                graphs.Add(graph);
            }

            return graphs;
        }

        public GoogleGraph GetInvoiceReportDashboard(DateTime fromDate, DateTime toDate,
            int status, string idArray, bool byClient, string dateFormat, int currencyId)
        {
            string currency = CurrencyBM.Instance.GetById(currencyId).Symbol;
            var graph = new GoogleGraph();
            var dataSource = SalesInvoiceDA.Instance.GetInvoiceReportDashboard(fromDate, toDate, status, idArray,
                byClient, currencyId);

            var users = dataSource.Select(r => r.Name).Distinct().ToList();
            var date = dataSource.Select(r => r.IssuedDate).Distinct().ToList();

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
                    var obj = dataSource.SingleOrDefault(r => r.IssuedDate == issueDate && r.Name == user);
                    var total = obj != null ? obj.Amount : 0;
                    var dataPoint = new DataPoint { v = total.ToString(), f = string.Format("{0}{1}", currency, total.Value.ToString("C").Replace("$", "")) };
                    list.Add(dataPoint);
                }
                dataPointSet.c = list.ToArray();
                dataPointSetList.Add(dataPointSet);
            }
            graph.rows = dataPointSetList.ToArray();
            return graph;
        }

        public int SaveInvoiceApi(SalesInvoice model)
        {
            return SalesInvoiceDA.Instance.SaveInvoiceApi(model);
        }
    }
}
