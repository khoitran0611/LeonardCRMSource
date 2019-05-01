using System;
using System.Collections.Generic;
using System.Data;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using System.Linq;


namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesInvoiceDA : EF5RepositoryBase<LeonardUSAEntities, SalesInvoice>
    {
        private static volatile SalesInvoiceDA _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesInvoiceDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesInvoiceDA();
                    }
                }

                return _instance;
            }
        }
        private SalesInvoiceDA()
            : base(Settings.ConnectionString)
        { }

        public int SaveInvoice(SalesInvoice invoice)
        {
            int status = 0;
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                try
                {
                    if (invoice.OrderId == -1)
                    {
                        invoice.OrderId = null;
                    }
                    invoice.Total = invoice.Services.Sum(s => s.Cost);
                    invoice.IsActive = true;
                    _context.SalesInvoices.Attach(invoice);
                    _context.SalesInvoices.Add(invoice);

                    status = _context.SaveChanges();
                    if (status > 0)
                    {
                        //note
                        foreach (var note in invoice.Notes)
                        {
                            note.RecordId = invoice.Id;
                            _context.Eli_Notes.Attach(note);
                            _context.Eli_Notes.Add(note);
                        }

                        var needUpdate = invoice.Taxes.Sum(t => t.TaxValue) > 0;
                        decimal totalTax = 0;
                        foreach (var salesInvTax in invoice.Taxes)
                        {
                            if (needUpdate)
                            {
                                salesInvTax.InvoiceId = invoice.Id;
                                salesInvTax.TaxId = salesInvTax.Id;
                                salesInvTax.CreatedDate = DateTime.Now;
                                salesInvTax.ModifiedDate = DateTime.Now;
                                salesInvTax.CreatedBy = invoice.CreatedBy;
                                salesInvTax.ModifiedBy = invoice.ModifiedBy;
                                _context.SalesInvTaxes.Attach(salesInvTax);
                                _context.SalesInvTaxes.Add(salesInvTax);
                                totalTax += salesInvTax.TaxValue * invoice.Total.Value / 100;
                            }
                        }
                        invoice.Total += totalTax;
                        foreach (var service in invoice.Services)
                        {
                            var invoiceService = new SalesInvService()
                            {
                                Id = 0, //remove id from client side
                                InvId = invoice.Id,
                                ServiceName = service.ServiceName,
                                Cost = service.Cost,
                                Description = service.Description,
                                Comments = service.Comments,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                CreatedBy = invoice.CreatedBy,
                                ModifiedBy = invoice.ModifiedBy

                            };
                            _context.SalesInvServices.Attach(invoiceService);
                            _context.SalesInvServices.Add(invoiceService);
                            needUpdate = true;
                        }
                        foreach (var fieldData in invoice.FieldData.Where(
                            f => !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")))
                        {
                            fieldData.MaterRecordId = invoice.Id;
                            _context.Eli_FieldData.Attach(fieldData);
                            _context.Eli_FieldData.Add(fieldData);
                            needUpdate = true;
                        }
                        if (needUpdate)
                            _context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (invoice.Id != 0)
                    {
                        _context.SalesInvoices.Remove(invoice);
                    }
                    throw ex;
                }
            }

            return status;
        }

        public int UpdateInvoice(SalesInvoice invoice)
        {
            int status = 0;
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                try
                {
                    if (invoice.OrderId == -1)
                    {
                        invoice.OrderId = null;
                    }
                    var deletedfielddata =
                        invoice.FieldData.Where(
                            f => f.Id > 0 && (string.IsNullOrEmpty(f.FieldData) || f.FieldData.Contains("-000-") || f.FieldData.Contains("undefined")));
                    var newFieldData = invoice.FieldData.Where(
                            f => f.Id == 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();

                    var updateFieldData = invoice.FieldData.Where(
                            f => f.Id > 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();

                    invoice.Total = invoice.Services.Sum(s => s.Cost);
                    _context.SalesInvoices.Attach(invoice);
                    _context.Entry(invoice).State = System.Data.Entity.EntityState.Modified;

                    var invServices = _context.SalesInvServices.Where(s => s.InvId == invoice.Id).ToList();
                    var services = invoice.Services.Where(s => s.Id != 0).ToList();

                    foreach (var salesInvService in invServices)
                    {

                        var entity = services.FirstOrDefault(s => s.Id == salesInvService.Id);

                        if (entity != null)
                        {
                            salesInvService.Cost = entity.Cost;
                            _context.Entry(salesInvService).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            _context.SalesInvServices.Remove(salesInvService);
                        }
                    }

                    foreach (var service in invoice.Services)
                    {
                        if (service.Id == 0)
                        {
                            service.InvId = invoice.Id;
                            _context.SalesInvServices.Attach(service);
                            _context.SalesInvServices.Add(service);
                        }
                    }

                    var taxes = _context.SalesInvTaxes.Where(t => t.InvoiceId == invoice.Id);
                    var needUpdate = invoice.Taxes.Sum(t => t.TaxValue) > 0;
                    decimal totalTax = 0;
                    if (taxes.Any())
                    {
                        foreach (var salesInvTax in invoice.Taxes)
                        {
                            var oldTax = taxes.SingleOrDefault(t => t.TaxId == salesInvTax.Id);
                            if (needUpdate)
                            {
                                if (oldTax != null)
                                    oldTax.TaxValue = salesInvTax.TaxValue;
                                else
                                    CreateTax(invoice, salesInvTax, _context);
                                totalTax += salesInvTax.TaxValue * invoice.Total.Value / 100;
                            }
                            else
                                _context.Entry(oldTax).State = System.Data.Entity.EntityState.Deleted;
                        }
                        invoice.Total += totalTax;
                    }
                    else
                    {
                        if (needUpdate)
                        {
                            foreach (var invTax in invoice.Taxes)
                            {
                                CreateTax(invoice, invTax, _context);
                                totalTax += invTax.TaxValue * invoice.Total.Value / 100;
                            }
                            invoice.Total += totalTax;
                        }
                    }

                    foreach (var fieldData in deletedfielddata)
                    {
                        _context.Eli_FieldData.Attach(fieldData);
                        _context.Entry(fieldData).State = System.Data.Entity.EntityState.Deleted;
                        _context.Eli_FieldData.Remove(fieldData);
                    }

                    foreach (var updatefield in updateFieldData)
                    {
                        _context.Entry(updatefield).State = System.Data.Entity.EntityState.Modified;
                    }

                    foreach (var newfield in newFieldData)
                    {
                        if (!string.IsNullOrEmpty(newfield.FieldData))
                        {
                            _context.Eli_FieldData.Attach(newfield);
                            _context.Eli_FieldData.Add(newfield);
                        }
                    }

                    status = _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return status;
        }

        private static void CreateTax(SalesInvoice invoice, SalesInvTax invTax, LeonardUSAEntities _context)
        {
            invTax.InvoiceId = invoice.Id;
            invTax.TaxId = invTax.Id;
            invTax.CreatedDate = DateTime.Now;
            invTax.ModifiedDate = DateTime.Now;
            invTax.CreatedBy = invoice.CreatedBy;
            invTax.ModifiedBy = invoice.ModifiedBy;
            _context.SalesInvTaxes.Attach(invTax);
            _context.SalesInvTaxes.Add(invTax);
        }

        public IList<SalesInvoice> GetRecentlyAddedInvoices(int userId, int roleId, bool onlyMe)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.SalesInvoices.OrderByDescending(c => c.CreatedDate).Take(10).ToList();

                return _context.SalesInvoices.Where(c => c.CreatedBy == userId).OrderByDescending(c => c.CreatedDate).Take(10).ToList();
            }
        }

        public IList<sp_InvoiceReportByDays_Result> GetReportDataByDays(int userId, int roleId, bool onlyMe, int days)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.sp_InvoiceReportByDays(null, days).ToList();
                return _context.sp_InvoiceReportByDays(userId, days).ToList();
            }
        }

        public IList<sp_InvoiceReportPerDays_Result> GetReportDataPerDays(int userId, int roleId, bool onlyMe, int days, int currencyId)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.sp_InvoiceReportPerDays(null, days, currencyId).ToList();
                return _context.sp_InvoiceReportPerDays(userId, days, currencyId).ToList();
            }
        }

        public IList<sp_InvoiceReportDashboard_Result> GetInvoiceReportDashboard(DateTime fromDate, DateTime toDate, int status, string idArray, bool byClient, int currencyId)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return _context.sp_InvoiceReportDashboard(fromDate, toDate, status, idArray, byClient, currencyId).ToList();
            }
        }

        /// <summary>
        /// Public Api: For Insert & Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveInvoiceApi(SalesInvoice model)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var status = 0;
                var invServices = model.SalesInvServices.ToList();
                var invServicesIds = invServices.Where(r => r.Id != 0).Select(r => r.Id).Distinct().ToArray();
                var newInvServices = invServices.Where(r => r.Id == 0);
                var deleteInvServices = context.SalesInvServices.AsNoTracking()
                                        .Where(r => !invServicesIds.Contains(r.Id) && r.InvId == model.Id)
                                        .ToList();
                var updateInvServices = invServices.Where(r => deleteInvServices.Any(dl => dl.Id != r.Id)).ToList();
                model.SalesInvServices.Clear();
                // for update
                foreach (var item in updateInvServices)
                {
                    context.SalesInvServices.Attach(item);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                // for update
                foreach (var item in deleteInvServices)
                {
                    context.SalesInvServices.Attach(item);
                    context.SalesInvServices.Remove(item);
                }

                var invTaxes = model.SalesInvTaxes.ToList();
                var invTaxesIds = invTaxes.Where(r => r.Id != 0).Select(r => r.Id).Distinct().ToArray();
                var newInvTaxes = invTaxes.Where(r => r.Id == 0);
                var deleteInvTaxes = context.SalesInvTaxes.AsNoTracking()
                                        .Where(r => !invTaxesIds.Contains(r.Id) && r.InvoiceId == model.Id)
                                        .ToList();
                var updateInvTaxess = invTaxes.Where(r => deleteInvTaxes.Any(dl => dl.Id != r.Id)).ToList();
                model.SalesInvTaxes.Clear();
                // for update
                foreach (var item in updateInvTaxess)
                {
                    context.SalesInvTaxes.Attach(item);
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                // for update
                foreach (var item in deleteInvTaxes)
                {
                    context.SalesInvTaxes.Attach(item);
                    context.SalesInvTaxes.Remove(item);
                }

                // for create
                context.SalesInvoices.Attach(model);
                foreach (var item in newInvServices)
                {
                    model.SalesInvServices.Add(item);
                }
                foreach (var item in newInvTaxes)
                {
                    model.SalesInvTaxes.Add(item);
                }
                if (model.Id > 0)
                {
                    var deletedfielddata = model.FieldData.Where(f => f.Id > 0 && (string.IsNullOrEmpty(f.FieldData) || f.FieldData.Contains("-000-") || f.FieldData.Contains("undefined"))).ToList();
                    var newfielddata = model.FieldData.Where(f => f.Id == 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();
                    var updatefielddata = model.FieldData.Where(f => f.Id > 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();

                    foreach (var fieldData in deletedfielddata)
                    {
                        context.Eli_FieldData.Attach(fieldData);
                        context.Entry(fieldData).State = System.Data.Entity.EntityState.Deleted;
                        context.Eli_FieldData.Remove(fieldData);
                    }

                    foreach (var field in newfielddata)
                    {
                        if (!string.IsNullOrEmpty(field.FieldData))
                        {
                            context.Eli_FieldData.Attach(field);
                            context.Eli_FieldData.Add(field);
                        }
                    }

                    foreach (var updatefield in updatefielddata)
                    {
                        context.Entry(updatefield).State = System.Data.Entity.EntityState.Modified;
                    }

                    context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    status += context.SaveChanges();
                }
                else
                {

                    context.SalesInvoices.Add(model);
                    status += context.SaveChanges();
                    if (status > 0)
                    {
                        foreach (var field in model.FieldData.Where(
                            f => !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")))
                        {
                            field.MaterRecordId = model.Id;
                            context.Eli_FieldData.Attach(field);
                            context.Eli_FieldData.Add(field);
                        }
                        status += context.SaveChanges();
                    }
                }
                return status;
            }
        }
    }
}
