using System;
using System.Collections.Generic;
using System.Data.Entity;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using System.Linq;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using Newtonsoft.Json;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesOrderDA : EF5RepositoryBase<LeonardUSAEntities, SalesOrder>
    {
        private static volatile SalesOrderDA _instance;
        private static readonly object SyncRoot = new Object();
        public static SalesOrderDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesOrderDA();
                    }
                }

                return _instance;
            }
        }
        private LeonardUSAEntities _context;

        public SalesOrderDA() : base(Settings.ConnectionString) { }

        public int CreateSalesOrders(SalesOrder order)
        {
            int status;
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                //First, add the new customer if have 
                if (order.SalesCustomer != null && order.SalesCustomer.Id == 0)
                {
                    var customer = order.SalesCustomer;
                    _context.SalesCustomers.Add(customer);
                    status = _context.SaveChanges();
                    if (status > 0)
                    {
                        order.CustomerId = customer.Id;

                        //add the new relationships if have
                        if (customer.SalesCustReferences != null &&
                            customer.SalesCustReferences.Any())
                        {
                            foreach (var cusRef in customer.SalesCustReferences)
                            {
                                cusRef.CustomerId = customer.Id;
                                _context.Entry(cusRef).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                    }
                }

                //Stop when the order is not belong to any customer
                if(order.CustomerId == 0) return 0;

                //Add the new order
                _context.SalesOrders.Add(order);
                status = _context.SaveChanges();
                if (status > 0)
                {
                    //responsible users
                    if (order.ResponsibleUsers != null)
                    {
                        var userIds = order.ResponsibleUsers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                        foreach (var userid in userIds)
                        {
                            var user = new SalesOrdersUser()
                            {
                                OrderId = order.Id,
                                ModifiedBy = order.ModifiedBy,
                                CreatedBy = order.CreatedBy,
                                ModifedDate = order.ModifiedDate,
                                CreatedDate = order.CreatedDate,
                                UserId = userid
                            };
                            _context.SalesOrdersUsers.Add(user);
                        }
                    }

                    //customer field
                    if (order.FieldData != null)
                    {
                        foreach (var fieldData in order.FieldData.Where(f => f.Id == 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")))
                        {
                            fieldData.MaterRecordId = order.Id;
                            _context.Eli_FieldData.Attach(fieldData);
                            _context.Eli_FieldData.Add(fieldData);
                        }
                    }

                    //note
                    if (order.Notes != null)
                    {
                        foreach (var note in order.Notes)
                        {
                            note.RecordId = order.Id;
                            _context.Eli_Notes.Attach(note);
                            _context.Eli_Notes.Add(note);
                        }
                    }

                    _context.SaveChanges();
                }
                status = order.Id;
            }

            return status;
        }

        public int UpdateSalesOrders(SalesOrder order, string documentPath = "")
        {
            int status;

            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                //modify the customer if have
                if (order.SalesCustomer != null)
                {
                    var customer = order.SalesCustomer;

                    //modify the relationship if have
                    if (customer.SalesCustReferences != null &&
                        customer.SalesCustReferences.Any())
                    {
                        foreach (var cusRef in customer.SalesCustReferences)
                        {
                            if (cusRef.Id > 0)
                            {
                                _context.SalesCustReferences.Attach(cusRef);
                                _context.Entry(cusRef).State = System.Data.Entity.EntityState.Modified;
                            }
                            else if (cusRef.Id == 0)
                            {
                                _context.Entry(cusRef).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                    }
                    _context.SalesCustomers.Attach(customer);
                    _context.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                }

                //Update order
                _context.SalesOrders.Attach(order);
                _context.Entry(order).State = System.Data.Entity.EntityState.Modified;

                var userEntities = _context.SalesOrdersUsers.Where(user => user.OrderId == order.Id).ToList();

                foreach (var user in userEntities)
                {
                    _context.SalesOrdersUsers.Attach(user);
                    _context.SalesOrdersUsers.Remove(user);
                }

                //Update deliveries if have
                if (order.SalesOrderDeliveries != null && order.SalesOrderDeliveries.Any())
                {
                    foreach (var delivery in order.SalesOrderDeliveries)
                    {
                        if (delivery.Id > 0)
                        {
                            _context.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            delivery.OrderId = order.Id;
                            _context.Entry(delivery).State = System.Data.Entity.EntityState.Added;
                        }
                    }
                }

                //Update completes if have
                if (order.SalesOrderCompletes != null && order.SalesOrderCompletes.Any())
                {
                    foreach (var complete in order.SalesOrderCompletes)
                    {
                        if (complete.Id > 0)
                        {
                            _context.Entry(complete).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            complete.OrderId = order.Id;
                            _context.Entry(complete).State = System.Data.Entity.EntityState.Added;
                        }
                    }
                }
                List<SalesDocument> customerDocument = null;
                //Remove all related documents
                if (order.Filenames != null && order.Filenames.Count > 0)
                {
                    customerDocument = _context.SalesDocuments.Where(doc => doc.OrderId == order.Id).ToList();
                    foreach (var doc in customerDocument)
                    {
                        //_context.SalesDocuments.Attach(doc);
                        //_context.Entry(doc).State = System.Data.Entity.EntityState.Deleted;
                        _context.SalesDocuments.Remove(doc);
                    }
                }

                //Update the responsible users if have
                if (order.ResponsibleUsers != null)
                {
                    var userIds = order.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var userid in userIds)
                    {
                        int temp;
                        if (int.TryParse(userid, out  temp))
                        {
                            var user = new SalesOrdersUser()
                            {
                                OrderId = order.Id,
                                ModifiedBy = order.ModifiedBy,
                                CreatedBy = order.CreatedBy,
                                ModifedDate = order.ModifiedDate,
                                CreatedDate = order.CreatedDate,
                                UserId = int.Parse(userid)
                            };
                            _context.SalesOrdersUsers.Add(user);
                        }
                    }
                }

                ////Add new related documents
                if (order.Filenames != null && order.Filenames.Count > 0)
                {
                    foreach (var filename in order.Filenames)
                    {
                        var file = new SalesDocument()
                        {
                            OrderId = order.Id,
                            ModifiedBy = order.ModifiedBy,
                            CreatedBy = order.CreatedBy,
                            ModifiedDate = order.ModifiedDate,
                            CreatedDate = order.CreatedDate,
                            FileName = filename,
                            Folder = null//order.UploadDirectory
                        };
                        _context.SalesDocuments.Attach(file);
                        _context.SalesDocuments.Add(file);
                    }
                }

                if (order.FieldData != null)
                {
                    var deletedfielddata = order.FieldData.Where(f => f.Id > 0 && (string.IsNullOrEmpty(f.FieldData) || f.FieldData.Contains("-000-") || f.FieldData.Contains("undefined"))).ToList();
                    var addedfielddata = order.FieldData.Where(f => f.Id == 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();
                    var updatedfielddata = order.FieldData.Where(f => f.Id > 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();

                    order.FieldData = null;
                    foreach (var fieldData in deletedfielddata)
                    {
                        _context.Eli_FieldData.Attach(fieldData);
                        _context.Entry(fieldData).State = System.Data.Entity.EntityState.Deleted;
                        _context.Eli_FieldData.Remove(fieldData);
                    }

                    foreach (var newfield in addedfielddata)
                    {
                        if (!string.IsNullOrEmpty(newfield.FieldData))
                        {
                            newfield.MaterRecordId = order.Id;
                            _context.Eli_FieldData.Attach(newfield);
                            _context.Eli_FieldData.Add(newfield);
                        }
                    }

                    foreach (var updatefield in updatedfielddata)
                    {
                        _context.Entry(updatefield).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                status = _context.SaveChanges();

                //delete the physical file
                if (status > 0 && customerDocument != null && customerDocument.Any() &&
                    order.Filenames != null && order.Filenames.Any())
                {
                    var deleteFiles = customerDocument.Where(x => !order.Filenames.Contains(x.FileName)).Select(x => Path.GetFileName(x.FileName));
                    if (deleteFiles != null && deleteFiles.Any())
                    {
                        foreach (var f in deleteFiles)
                        {
                            var fpath = documentPath + "\\" + f;
                            if (File.Exists(fpath))
                            {
                                File.Delete(fpath);
                            }
                        }
                    }
                }
            }

            return status;
        }

        public int CloneApp(int appId, int currentUserId)
        {
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var app = SingleLoadWithReferences(x => x.Id == appId, "SalesCustomer");   
                if (app != null)
                {
                    //create new app
                    //**clear app data
                    app.Id = 0;
                    app.SignatureDate = null;
                    app.SignatureIP = null;
                    app.LesseeSignature = null;
                    app.CoSignature = null;
                    app.CoSignatureDate = null;
                    app.CoSignatureIP = null;
                    app.CustomerId = 0;
                    app.Status = OrderStatus.Pending.GetHashCode();
                    
                    app.CreatedDate = DateTime.Now;
                    app.ModifiedDate = DateTime.Now;
                    app.IsActive = true;

                    //clear customer data
                    if (app.SalesCustomer != null)
                    {
                        var custReferences = _context.SalesCustReferences.Where(x => x.CustomerId == app.SalesCustomer.Id).ToList();
                        app.SalesCustomer.Id = 0;
                        app.SalesCustomer.CreatedDate = DateTime.Now;
                        app.SalesCustomer.ModifiedDate = DateTime.Now;
                        app.SalesCustomer.IsActive = true;
                        app.SalesCustomer.SalesCustReferences = new List<SalesCustReference>();
                        
                        if (custReferences != null && custReferences.Any())
                        {
                            foreach (var custRef in custReferences)
                            {
                                app.SalesCustomer.SalesCustReferences.Add(new SalesCustReference
                                {
                                    Id = 0,
                                    Name = custRef.Name,
                                    Relationship = custRef.Relationship,
                                    Phone = custRef.Phone,
                                    CustomerId = 0,
                                    CreatedBy = custRef.CreatedBy,
                                    ModifiedBy = custRef.ModifiedBy,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    IsActive = custRef.IsActive,
                                });
                            }
                        }
                    }                    

                    
                    _context.Entry(app).State = System.Data.Entity.EntityState.Added;                    
                    _context.SaveChanges();

                    //**cancel old app                    
                    var oldApp = Single(x => x.Id == appId);
                    oldApp.Status = OrderStatus.Rejected.GetHashCode();
                    oldApp.IsApproveOrder = false;
                    oldApp.ModifiedBy = currentUserId;
                    oldApp.ModifiedDate = DateTime.Now;
                    _context.Entry(oldApp).State = System.Data.Entity.EntityState.Modified;

                    _context.SaveChanges();
                    return app.Id;
                }

                return 0;
            }
        }

        public int UpdateSalesOrderDeliveries(SalesOrder order)
        {
            int status;

            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                _context.SalesOrders.Attach(order);
                _context.Entry(order).State = System.Data.Entity.EntityState.Modified;
               
                //Save order delivery
                foreach (var delivery in order.SalesOrderDeliveries)
                {
                    if (delivery.Id > 0)
                    {
                        _context.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        delivery.OrderId = order.Id;
                        _context.Entry(delivery).State = System.Data.Entity.EntityState.Added;
                    }
                }
                status = _context.SaveChanges();
            }

            return status;
        }

        public IList<SalesOrder> GetRecentlyAddedOrders(int userId, int roleId, bool onlyMe)
        {
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.SalesOrders.OrderByDescending(c => c.CreatedDate).Take(10).ToList();

                return _context.SalesOrders.Where(c => c.CreatedBy == userId).OrderByDescending(c => c.CreatedDate).Take(10).ToList();
            }
        }

        public IList<sp_OrderReportByDays_Result> GetReportDataByDays(int userId, int roleId, bool onlyMe, int days)
        {
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.sp_OrderReportByDays(null, days).ToList();

                return _context.sp_OrderReportByDays(userId, days).ToList();
            }
        }

        public IList<sp_OrderReportDashboard_Result> GetOrderReportDashboard(DateTime fromDate, DateTime toDate, int status, string idArray)
        {
            using (_context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return _context.sp_OrderReportDashboard(fromDate, toDate, status, idArray).ToList();
            }
        }

        public int SaveOrderApi(SalesOrder model)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var status = 0;
                var documents = model.SalesDocuments.ToList();
                var documentIds = documents.Where(r => r.Id != 0).Select(r => r.Id).Distinct().ToArray();
                var newdocument = documents.Where(r => r.Id == 0);
                var deletedocument = context.SalesDocuments.AsNoTracking()
                                        .Where(r => !documentIds.Contains(r.Id) && r.OrderId == model.Id)
                                        .ToList();
                var updatedocument = documents.Where(r => deletedocument.Any(dl => dl.Id != r.Id)).ToList();
                model.SalesDocuments.Clear();

                // for update
                foreach (var doc in updatedocument)
                {
                    context.SalesDocuments.Attach(doc);
                    context.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                }
                // for update
                foreach (var link in deletedocument)
                {
                    context.SalesDocuments.Attach(link);
                    context.SalesDocuments.Remove(link);
                }
                // for create
                context.SalesOrders.Attach(model);
                foreach (var link in newdocument)
                {
                    model.SalesDocuments.Add(link);
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
                    context.SalesOrders.Add(model);
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

        public NgTableModel GetApplicationByUser(int userId, int roleId, bool assistMode)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var deliveryRole = UserRoles.DeliveryStaff.GetHashCode();                
                var pendingDeliveryStatus = OrderStatus.PendingDelivery.GetHashCode();
                var deliveredNotSigned = OrderStatus.DeliveredNotSigned.GetHashCode();
                var isAdminUser = (roleId == UserRoles.Administrator.GetHashCode() || roleId == UserRoles.ClientAdmin.GetHashCode());

                var data = context.vwApplications.AsNoTracking().ToArray();
                data = data.Where(x => (x.CreatedBy == userId || ("," + x.ResponsibleUsers).Contains("," + userId + ",") || (assistMode && isAdminUser)) &&
                                       (roleId != deliveryRole || (roleId == deliveryRole && (x.StatusCode == pendingDeliveryStatus || x.StatusCode == deliveredNotSigned)))).ToArray();
                return new NgTableModel() { Data = data, Total = data.Length };
            }
        }

        public int CancelApplication(int appId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var data = context.SalesOrders.FirstOrDefault(x => x.Id == appId);
                data.Status = OrderStatus.Rejected.GetHashCode();
                context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                return context.SaveChanges();
            }
        }
        
        public OverdueApps GetOverdueApp(int pageIndex, int pageSize, string sortDesc, int overdueMonth, FilterOverdueAppsParams filterParams)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var totalParam = new ObjectParameter("totalRow", typeof(int));
                var data = context.sp_GetAllOverdueApps(filterParams.Id,filterParams.Status, filterParams.CustomerName, filterParams.PartNumber, 
                                                        filterParams.CapitalizationPeriod, filterParams.ModifiedDate, pageIndex, pageSize, 
                                                        sortDesc, overdueMonth, totalParam);

                var result = new OverdueApps()
                {
                    ReturnCode = ResultCodes.Success,
                    Data = data != null ? data.ToList() : new List<sp_GetAllOverdueApps_Result>(),
                    Total = (int)totalParam.Value
                };

                return result;
            }
        }

        public int DeleteOverdueApp(int overdueMonth, int total, FilterOverdueAppsParams filterParams)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var totalParam = new ObjectParameter("totalRow", typeof(int));
                var data = context.sp_GetAllOverdueApps(filterParams.Id, filterParams.Status, filterParams.CustomerName, filterParams.PartNumber,
                                                        filterParams.CapitalizationPeriod, filterParams.ModifiedDate, 1, total,
                                                        "Id asc", overdueMonth, totalParam).Select(x => new SalesOrder() { Id = x.Id });

                foreach (var order in data)
                {
                    context.Entry(order).State = System.Data.Entity.EntityState.Deleted;
                }

                return context.SaveChanges();
            }
        }


        public bool CheckExpectedStatus(int status, int appId, out bool isExistOrder)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var order = context.SalesOrders.FirstOrDefault(o => o.Id == appId);
                isExistOrder = order != null;
                return isExistOrder && order.Status == status;
            }
        }

        public int SaveContractSignature(SalesOrder app)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {               
                if (app.SalesCustomer != null)
                {
                    var customer = context.SalesCustomers.First(x => x.Id == app.CustomerId);
                    customer.CustomerInitials = app.SalesCustomer.CustomerInitials;
                    customer.ModifiedDate = DateTime.Now;
                    customer.ModifiedBy = app.ModifiedBy;

                    context.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                }

                app.SalesCustomer = null;
                context.Entry(app).State = System.Data.Entity.EntityState.Modified;
               
                return context.SaveChanges();
            }
        }

        public IList<Eli_User> GetSenderForCancelApp(int appId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var app = context.SalesOrders.Include("SalesCustomer").FirstOrDefault(x => x.Id == appId);
                if (app != null)
                {
                    var managerRole = UserRoles.ContractManager.GetHashCode();
                    var creator = app.CreatedBy.Value;
                    var responsibleUsers = app.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));

                    var res = context.Eli_User.Where(x => x.Id == creator || (responsibleUsers.Contains(x.Id) && x.RoleId == managerRole)).ToList();
                    if (!string.IsNullOrEmpty(app.SalesCustomer.Email))
                    {
                        res.Add(new Eli_User() { Email = app.SalesCustomer.Email });
                    }

                    return res;
                }

                return new List<Eli_User>();
            }
        }

        public bool CheckDriverAssigned(int[] assignedUserIds)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var deliverRole = UserRoles.DeliveryStaff.GetHashCode();
                return context.Eli_User.Any(x => assignedUserIds.Contains(x.Id) && x.RoleId == deliverRole);
            }
        }

        public int AssignAppToNewUser(Eli_User user, SalesOrder app)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var status = 0;

                //insert new user if need
                if(user.Id == 0)
                {
                    context.Entry(user).State = System.Data.Entity.EntityState.Added;
                    status = context.SaveChanges();
                }
                

                if (status > 0)
                {
                    app.CreatedBy = user.Id;
                    app.CreatedDate = DateTime.Now;
                    app.ModifiedBy = user.Id;
                    app.ModifiedDate = DateTime.Now;
                    context.Entry(app).State = System.Data.Entity.EntityState.Modified;

                    var customer = app.SalesCustomer;
                    if (customer == null)
                    {
                        var customerId = app.CustomerId;
                        customer = context.SalesCustomers.Single(x => x.Id == customerId);                        
                    }

                    customer.CreatedBy = user.Id;
                    customer.CreatedDate = DateTime.Now;
                    customer.ModifiedBy = user.Id;
                    customer.ModifiedDate = DateTime.Now;
                    context.Entry(customer).State = System.Data.Entity.EntityState.Modified;

                    status = context.SaveChanges();
                }
                else
                {
                    status = 0;
                }

                return status;
            }
        }
    }
}
