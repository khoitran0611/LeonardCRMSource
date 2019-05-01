using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Http;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eli.Common;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;


namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SalesInvoiceApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        public string ServicelLineHeader = String.Empty;

        public const string ServiceLineFooter = "</table>";

        public const string BodyTemplate = "<tr>" +
                                  "<td>Number</td>" +
                                  "<td>ServiceName</td>" +
                                  "<td>Description</td>" +
                                  "<td>Comments</td>" +
                                  "<td>Cost</td>" +
                                  "</tr>";

        public string TotalLineNoTax = string.Empty;
        public string TotalLine = string.Empty;
        public string TaxLine = string.Empty;
        public string GrandTotal = string.Empty;

        public SalesInvoiceApiController()
        {
            string no = GetText("SERVICES", "NO");
            string servicename = GetText("SERVICES", "SERVICE_NAME");
            string desc = GetText("SERVICES", "DESCRIPTION");
            string comment = GetText("SERVICES", "COMMENTS");
            string cost = GetText("SERVICES", "COST");
            string totalCost = GetText("SERVICES", "TOTAL_COST");
            string preTaxTotal = GetText("SERVICES", "PRE_TAX_TOTAL");
            string tax = GetText("SERVICES", "TAX");
            string granTotal = GetText("SERVICES", "GRAND_TOTAL");

            ServicelLineHeader =
           "<table cellspacing=\"1\" border=\"0.5\" ><tr><td><b>" + no + "</b></td><td><b>" + servicename + "</b></td><td><b>" + desc + "</b></td><td><b>" + comment + "</b></td><td><b>" + cost + "</b></td></tr>";

            TotalLineNoTax = "<tr>" +
            "<td colspan=\"4\" style=\"text-align: right\"><b>" + totalCost + "</b></td>" +
            "<td ><b>total</b></td>" +
            "</tr>";

            TotalLine = "<tr>" +
                        "<td colspan=\"4\" style=\"text-align: right\"><b>" + preTaxTotal + "</b></td>" +
                        "<td ><b>pretaxtotal</b></td>" +
                        "</tr>";

            TaxLine = "<tr>" +
                      "<td colspan=\"4\" style=\"text-align: right\"><b>" + tax + "</b></td>" +
                      "<td ><b>tax</b></td>" +
                      "</tr>";

            GrandTotal = "<tr>" +
                                "<td colspan=\"4\" style=\"text-align: right\"><b>" + granTotal + "</b></td>" +
                                "<td ><b>grandtotal</b></td>" +
                        "</tr>";
        }
        [HttpPost]
        public virtual SalesInvoice GetSalesInvoicesById([FromBody]string jsonObject)
        {
            var entity = new SalesInvoice();
            var reference = new[] { "SalesInvServices", "SalesInvTaxes" };
            try
            {
                var param = jsonObject.Split(',');
                if (param.Length > 0)
                {
                    int invoiceId = 0;
                    int moduleId = 0;
                    int.TryParse(param[0], out invoiceId);
                    int.TryParse(param[1], out moduleId);
                    if (invoiceId != 0)
                    {
                        entity = SalesInvoiceBM.Instance.SingleLoadWithReferences(invoice => invoice.Id == invoiceId,
                                                                                 reference);
                        if (!string.IsNullOrEmpty(entity.Details) && entity.Details.Contains("<br>"))
                        {
                            entity.Details = entity.Details.Replace("<br>", "\n");
                        }
                    }
                    entity.Currencies = CurrencyBM.Instance.Find(c => c.IsActive);
                    if (string.IsNullOrEmpty(entity.ResponsibleUsers))
                    {
                        entity.UserIds = CurrentUserID ;
                        entity.ResponsibleUsers = CurrentUserID + ",";
                    }
                    else
                    {
                        entity.ResponsibleUsers = entity.ResponsibleUsers.Substring(0, entity.ResponsibleUsers.Length - 1);
                        entity.UserIds = int.Parse(entity.ResponsibleUsers);
                    }
                    entity.CustomFields = EntityFieldBM.Instance.GetCustomFieldByModuleId(moduleId, invoiceId,CurrentUserRole.Id);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return null;
            }
            return entity;
        }

        [HttpPost]
        public virtual ResultObj AddSalesInvoice([FromBody]JObject jsonObject, [FromUri] int moduleId)
        {
            string msg = string.Empty;
            try
            {
                var invoice = JsonConvert.DeserializeObject<SalesInvoice>(jsonObject.ToString());
                if (invoice.UserIds > 0)
                {
                    invoice.ResponsibleUsers = String.Join(",", invoice.UserIds) + ",";
                }
                else
                {
                    invoice.ResponsibleUsers = CurrentUserID + ",";
                }
                SetAuditFields(invoice, invoice.Id);
                if (invoice.Id > 0 && invoice.UserIds != CurrentUserID )
                {
                    invoice.CreatedBy = invoice.UserIds;
                }

                msg = ValidateObject(invoice, moduleId);
                foreach (var field in invoice.FieldData)
                {
                    SetAuditFields(field, field.Id);
                }
                foreach (var service in invoice.Services)
                {
                    SetAuditFields(service, service.Id);
                }
                foreach (var tax in invoice.Taxes)
                {
                    SetAuditFields(tax, tax.Id);
                }

                invoice.Notes = invoice.Notes ?? new List<Eli_Notes>();
                foreach (var note in invoice.Notes)
                {
                    SetAuditFields(note, note.Id);
                }

                if (string.IsNullOrEmpty(msg))
                {
                    int status = 0;
                    if (!string.IsNullOrEmpty(invoice.Details) && invoice.Details.Contains("\n"))
                    {
                        invoice.Details = invoice.Details.Replace("\n", "<br>");
                    }
                    status = invoice.Id == 0 ? SalesInvoiceBM.Instance.SaveInvoice(invoice) : SalesInvoiceBM.Instance.UpdateInvoice(invoice);

                    if (status > 0)
                    {
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), invoice.Id);
                    }
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), invoice.Id);

                }
                return new ResultObj(ResultCodes.ValidationError, msg, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, ex.Message, 0);
            }
        }

        [HttpPost]
        public virtual ResultObj DeleteSalesInvoice([FromBody]JArray jsonArray)
        {
            try
            {
                int status = 0;
                var entities = JsonConvert.DeserializeObject<List<SalesInvoice>>(jsonArray.ToString());
                status = SalesInvoiceBM.Instance.Delete(entities);
                if (status > 0)
                {
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"), 0);
                }

                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), 0);
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.ToString(), 0);
            }
        }

        [HttpPost]
        public ResultObj ExportToPdf([FromBody]string jsonObject)
        {
            var reference = new string[] { "SalesInvServices", "SalesInvTaxes" };
            try
            {
                int invoiceId = 0;
                int moduleId = 0;
                var param = jsonObject.Split(',');
                int.TryParse(param[0], out invoiceId);
                int.TryParse(param[1], out moduleId);
                var entity = new SalesInvTemplate();
                var invoice = SalesInvoiceBM.Instance.SingleLoadWithReferences(inv => inv.Id == invoiceId,
                                                                               reference);
                entity = SalesInvTemplateBM.Instance.GetById(invoice.InvTemplateId);
                entity.TemplateContent = ReplaceContentTemplate(entity, invoice, moduleId);
                string fname = "Invoice-" + invoice.Id.ToString() + ".pdf";
                string uploadPath = HttpContext.Current.Server.MapPath(Eli.Common.ConfigValues.UPLOAD_DIRECTORY);
                string tempPath = uploadPath + "temp/";
                string fullPath = tempPath + fname;
                entity.TemplateContent = entity.TemplateContent.Replace(Eli.Common.ConfigValues.UPLOAD_DIRECTORY, uploadPath);
                try
                {
                    Document pdfDoc = null;
                    using (pdfDoc = pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f))
                    {
                        StringReader sr = null;
                        HTMLWorker htmlparser = null;
                        sr = new StringReader(entity.TemplateContent);
                        htmlparser = new HTMLWorker(pdfDoc);
                        PdfWriter.GetInstance(pdfDoc, new FileStream(fullPath, FileMode.Create));
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                    }
                    string fpath = Eli.Common.ConfigValues.UPLOAD_DIRECTORY + "temp/" + fname;
                    return new ResultObj(ResultCodes.Success, fpath, 0);
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.Message, ex);
                    return new ResultObj(ResultCodes.UnkownError, ex.Message, 0);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, exception.Message, 0);
            }
        }

        private string ReplaceContentTemplate(SalesInvTemplate template, SalesInvoice invoice, int moduleId)
        {
            string content = template.TemplateContent;
            string bodyHtml = string.Empty;
            decimal total = 0;
            try
            {
                var serviceLine = new vwViewColumn { };
                serviceLine.ColumnName = "ServiceLine";
                serviceLine.FieldId = -1;
                var entities = EntityFieldBM.Instance.GetViewColumnsByModule(moduleId,true,CurrentUserRole.Id);
                var insertedColumns = entities.Where(e => content.Contains(e.ColumnName)).ToList();
                insertedColumns.Add(serviceLine);

                var currency = CurrencyBM.Instance.GetById(invoice.CurrencyId).Symbol;
                if (invoice.SalesInvServices != null && invoice.SalesInvServices.Count > 0)
                {
                    int number = 1;

                    foreach (var salesInvService in invoice.SalesInvServices)
                    {
                        total += salesInvService.Cost;
                        bodyHtml += ReplaceData(salesInvService, number, currency);
                        number++;
                    }
                    decimal taxvalue = 0;
                    if (invoice.SalesInvTaxes != null && invoice.SalesInvTaxes.Count > 0)
                    {
                        bodyHtml += TotalLine.Replace("pretaxtotal", currency + total.ToString());

                        foreach (var invTax in invoice.SalesInvTaxes)
                        {
                            taxvalue += (invTax.TaxValue * total) / 100;
                        }
                        string taxvalueCurrencyFormat = taxvalue.ToString("c");
                        taxvalueCurrencyFormat = taxvalueCurrencyFormat.Replace("$", "");
                        bodyHtml += TaxLine.Replace("tax", currency + taxvalueCurrencyFormat);

                        decimal grandtotal = total + taxvalue;
                        string grandtotalCurrencyFormat = grandtotal.ToString("c");
                        grandtotalCurrencyFormat = grandtotalCurrencyFormat.Replace("$", "");
                        bodyHtml += GrandTotal.Replace("grandtotal", currency + grandtotalCurrencyFormat);
                    }
                    else
                    {
                        string totalvalue = total.ToString("c");
                        totalvalue = totalvalue.Replace("$", "");
                        bodyHtml += TotalLineNoTax.Replace("total", currency + totalvalue);
                    }

                    invoice.ServiceLine = ServicelLineHeader + "<tbody>" + bodyHtml + ServiceLineFooter;
                }

                var properties = typeof(SalesInvoice).GetProperties();

                foreach (var column in insertedColumns)
                {
                    var propertyInfo = properties.FirstOrDefault(p => p.Name.Equals(column.ColumnName));
                    if (propertyInfo != null)
                    {
                        var propertyValue = invoice.GetType()
                                                          .GetProperty(propertyInfo.Name)
                                                          .GetValue(invoice, null);
                        if (propertyValue != null)
                        {
                            if (column.IsList && string.IsNullOrEmpty(column.ListSql))
                            {

                                var listvalues = ListNameBM.Instance.GetListNameValuesByModuleId(moduleId);
                                var listname = listvalues.FirstOrDefault(l => l.Id == (int)propertyValue);
                                if (listname != null)
                                {
                                    content = content.Replace("@" + column.ColumnName + "@", listname.Description);
                                }
                            }
                            else if (!string.IsNullOrEmpty(column.ListSql))
                            {
                                var sqlListValues = ListNameBM.Instance.GetReferenceListsByModule(moduleId);
                                var listValuesByFieldId = sqlListValues.Where(s => s.FieldId == column.FieldId).ToList();
                                var value =
                                    listValuesByFieldId.FirstOrDefault(
                                        l => l.FieldId == column.FieldId && l.Id == (int)propertyValue);
                                if (value != null)
                                {
                                    content = content.Replace("@" + column.ColumnName + "@", value.Description);
                                }
                            }
                            else
                            {
                                if (propertyValue is DateTime)
                                {
                                    string shordatetime = string.Format("{0:M/d/yyyy}", propertyValue);
                                    content = content.Replace("@" + column.ColumnName + "@", shordatetime);
                                }
                                else
                                {
                                    content = content.Replace("@" + column.ColumnName + "@", propertyValue.ToString());
                                }
                            }
                        }
                        else
                        {
                            content = content.Replace("@" + column.ColumnName + "@", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
            }
            return content;
        }

        private string ReplaceData(SalesInvService salesInvService, int number, string currency)
        {
            string msg = BodyTemplate;
            msg = msg.Replace("Number", number.ToString());
            msg = msg.Replace("ServiceName", salesInvService.ServiceName);
            msg = msg.Replace("Description", salesInvService.Description);
            msg = msg.Replace("Comments", salesInvService.Comments);
            string totolCostCurrencyFormat = salesInvService.Cost.ToString("c");
            totolCostCurrencyFormat = totolCostCurrencyFormat.Replace("$", "");

            msg = msg.Replace("Cost", currency + totolCostCurrencyFormat);
            return msg;
        }

        [HttpGet]
        public IList<SalesInvoice> GetRecentlyAddedInvoices([FromUri] bool onlyMe)
        {
            try
            {
                return SalesInvoiceBM.Instance.GetRecentlyAddedInvoices(CurrentUserID, CurrentUserRole.Id, onlyMe);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }
        [HttpGet]
        public IList<GoogleGraph> GetChartData([FromUri] bool onlyMe)
        {
            try
            {
                var dateFormat = SiteSettings.DATE_FORMAT;
                return SalesInvoiceBM.Instance.GetReportDataByDays(CurrentUserID, CurrentUserRole.Id, onlyMe, dateFormat);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }
        [HttpGet]
        public IList<GoogleGraph> GetChartInvoicePaidData([FromUri] bool onlyMe, [FromUri] int currencyId)
        {
            try
            {
                var dateFormat = SiteSettings.DATE_FORMAT;
                return SalesInvoiceBM.Instance.GetReportDataPerDays(CurrentUserID, CurrentUserRole.Id, onlyMe, dateFormat, currencyId);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public GoogleGraph GetInvoiceReportDashboard([FromBody] JObject jsonObject)
        {
            try
            {
                var reportObject = JsonConvert.DeserializeObject<ReportObject>(jsonObject.ToString());
                var idsString = string.Join(",", reportObject.UserIds);
                var dateFormat = SiteSettings.DATE_FORMAT;
                return SalesInvoiceBM.Instance.GetInvoiceReportDashboard(reportObject.FromDate, reportObject.ToDate, reportObject.Status, idsString, reportObject.ByClient, dateFormat, reportObject.Currency);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        private string ValidateObject(SalesInvoice invoice, int moduleId)
        {
            string msg = string.Empty;
            var validator = new ObjectValidator(moduleId);
            var result = validator.ValidateObject(invoice);
            if (result.Length > 0)
            {
                msg += result;
            }
            if (invoice.Services.Count > 0)
            {
                foreach (var service in invoice.Services)
                {
                    if (string.IsNullOrEmpty(service.ServiceName))
                    {
                        msg += GetText("SERVICES", "SERVICE_NAME_REQUIRED");
                    }
                    if (string.IsNullOrEmpty(service.Cost.ToString().Trim()))
                    {
                        msg += GetText("SERVICES", "COST_REQUIRED") + " " + service.ServiceName + "<br>";
                    }
                    float cost = 0;
                    bool flag = float.TryParse(service.Cost.ToString(), out cost);
                    if (!flag)
                    {
                        msg += GetText("SERVICES", "COST_INVALID");
                    }
                }
            }
            return msg;
        }
    }
}
