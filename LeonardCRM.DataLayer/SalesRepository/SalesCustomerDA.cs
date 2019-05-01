using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;
using ExpressionEvaluator;
using System.IO;
using System.Collections.Specialized;

namespace LeonardCRM.DataLayer.SalesRepository
{
    public sealed class SalesCustomerDA : EF5RepositoryBase<LeonardUSAEntities, SalesCustomer>
    {
        private static volatile SalesCustomerDA _instance;
        private static readonly object SyncRoot = new Object();

        public static SalesCustomerDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new SalesCustomerDA();
                    }
                }

                return _instance;
            }
        }
        
        public SalesCustomerDA()
            : base(Settings.ConnectionString)
        {
        }

        #region Property
        private class ExpressionValue
        {
            public object Value { get; set; }
        }
        #endregion

        #region Public Method
        public int SaveSalesCustomer(SalesCustomer entity, string attachmentFolder)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var status = 0;
                //var deletedfielddata = entity.FieldData.Where(f => f.Id > 0 && (string.IsNullOrEmpty(f.FieldData) || f.FieldData.Contains("-000-") || f.FieldData.Contains("undefined"))).ToList();
                //var newfielddata = entity.FieldData.Where(f => f.Id == 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();
                //var updatefielddata = entity.FieldData.Where(f => f.Id > 0 && !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")).ToList();

                if (entity.Id > 0)
                {
                    //foreach (var fieldData in deletedfielddata)
                    //{
                    //    context.Eli_FieldData.Attach(fieldData);
                    //    context.Entry(fieldData).State = System.Data.Entity.EntityState.Deleted;
                    //    context.Eli_FieldData.Remove(fieldData);
                    //}

                    //foreach (var field in newfielddata)
                    //{
                    //    if (!string.IsNullOrEmpty(field.FieldData))
                    //    {
                    //        context.Eli_FieldData.Attach(field);
                    //        context.Eli_FieldData.Add(field);
                    //    }
                    //}

                    //foreach (var updatefield in updatefielddata)
                    //{
                    //    context.Entry(updatefield).State = System.Data.Entity.EntityState.Modified;
                    //}

                    if (entity.SalesOrders != null)
                    {
                        foreach (var order in entity.SalesOrders)
                        {
                            context.Entry(order).State = System.Data.Entity.EntityState.Modified;
                        }
                         // Update Delivery Information
                        var saleOrder = entity.SalesOrders.FirstOrDefault();
                        if (saleOrder != null)
                        {
                            foreach (var delivery in saleOrder.SalesOrderDeliveries)
                            {
                                if (delivery.Id > 0)
                                {
                                    context.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    delivery.OrderId = saleOrder.Id;
                                    context.Entry(delivery).State = System.Data.Entity.EntityState.Added;
                                }
                            }
                        }
                    }

                    if (entity.SalesCustReferences != null)
                    {
                        foreach (var custRef in entity.SalesCustReferences)
                        {
                            if (custRef.Id > 0)
                            {
                                context.Entry(custRef).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                custRef.CustomerId = entity.Id;
                                context.Entry(custRef).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                    }
                    context.SalesCustomers.Attach(entity);
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    status += context.SaveChanges();
                }
                else
                {
                    if (entity.SalesOrders != null && entity.SalesOrders.Any())
                    {
                        var app = entity.SalesOrders.First();
                        if (app != null)
                        {
                            app.SalesCustomer = null;

                            foreach (var deliver in app.SalesOrderDeliveries)
                            {
                                deliver.SalesOrder = null;
                            }
                        }                        
                    }

                    context.SalesCustomers.Add(entity);
                    status += context.SaveChanges();
                    if (status > 0)
                    {
                        foreach (var field in entity.FieldData.Where(
                            f => !string.IsNullOrEmpty(f.FieldData) && !f.FieldData.Contains("-000-") && !f.FieldData.Contains("undefined")))
                        {
                            field.MaterRecordId = entity.Id;
                            context.Eli_FieldData.Attach(field);
                            context.Eli_FieldData.Add(field);
                        }
                        //note
                        foreach (var note in entity.Notes)
                        {
                            note.RecordId = entity.Id;
                            context.Eli_Notes.Attach(note);
                            context.Eli_Notes.Add(note);
                        }

                        var appObj =  entity.SalesOrders.FirstOrDefault();
                        var appId = appObj != null ? appObj.Id : 0;
                        if(appObj != null)
                        {          
                            if(entity.AttachmentFiles != null)
                            {
                                foreach (var fName in entity.AttachmentFiles)
                                {
                                    var att = new SalesDocument()
                                    {
                                        Id = 0,
                                        OrderId = appId,
                                        FileName = fName,
                                        Folder = ConfigValues.UPLOAD_DIRECTORY_SALE_DOCUMENT.Replace(ConfigValues.UPLOAD_DIRECTORY, ""),
                                        CreatedBy = entity.CreatedBy,
                                        CreatedDate = DateTime.Now,
                                        ModifiedBy = entity.CreatedBy,
                                        ModifiedDate = DateTime.Now
                                    };
                                    context.Entry(att).State = System.Data.Entity.EntityState.Added;
                                }
                            }
                            
                            //with the "Sold" process: update signature file name
                            if(appObj.IsSold == true && 
                               appObj.SalesOrderDeliveries != null && appObj.SalesOrderDeliveries.Any())
                            {
                                var relatedDelivery = appObj.SalesOrderDeliveries.First();
                                relatedDelivery.CustomerSignature = string.Format(Constant.DeliveryCustomerSignName, appObj.Id);
                                relatedDelivery.ModifiedDate = DateTime.Now;
                                context.Entry(relatedDelivery).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        

                        status += context.SaveChanges();                       
                    }

                }
                return status;
            }
        }

        public IList<SalesCustomer> GetRecentlyAddedCustomers(int userId, int roleId, bool onlyMe)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.SalesCustomers.OrderByDescending(c => c.CreatedDate).Take(10).ToList();

                return _context.SalesCustomers.Where(c => c.CreatedBy == userId).OrderByDescending(c => c.CreatedDate).Take(10).ToList();
            }
        }

        public IList<sp_CustomerReportByDays_Result> GetReportDataByDays(int userId, int roleId, bool onlyMe, int days)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                if (!onlyMe && _context.fn_GetRolesHierachy(roleId).Count() > 1)
                    return _context.sp_CustomerReportByDays(null, days).ToList();

                return _context.sp_CustomerReportByDays(userId, days).ToList();
            }
        }

        public IList<sp_CustomerReportDashboard_Result> GetReportDataByUsers(int?[] responsibleUsers)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return _context.sp_CustomerReportDashboard(responsibleUsers != null && responsibleUsers.Length > 0 ? string.Join(",", responsibleUsers) : null).ToList();
            }
        }

        public IList<vwClient> GetAllClients(int currentRole, int userId)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var roles = _context.fn_GetRolesHierachy(currentRole);
                if (roles.Count() > 1)
                    return _context.vwClients.AsNoTracking().Where(r => roles.Contains(r.RoleId)).ToList().GroupBy(t => t.Name).Select(group => group.First()).OrderBy(c => c.Name).ToList();

                return _context.vwClients.AsNoTracking().Where(r => r.ResponsibleUser == userId).OrderBy(c => c.Name).ToList();
            }
        }

        /// <summary>
        /// Public Api: For Insert & Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveCustomerApi(SalesCustomer model)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var status = 0;
                // for create
                context.SalesCustomers.Attach(model);
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

                    context.SalesCustomers.Add(model);
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

        public IList<vwAllCustomer> GetAllCustomers(int currentRole, int userId)
        {
            using (var _context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var roles = _context.fn_GetRolesHierachy(currentRole);
                if (roles.Count() > 1)
                    return _context.vwAllCustomers.AsNoTracking().Where(r => roles.Contains(r.RoleId)).ToList().GroupBy(t => t.Name).Select(group => group.First()).OrderBy(c => c.Name).ToList();

                return _context.vwAllCustomers.AsNoTracking().Where(r => r.ResponsibleUser == userId).OrderBy(c => c.Name).ToList();
            }
        }

        public int GetIdByEmail(string email)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var salesCustomers = context.SalesCustomers.FirstOrDefault(p => p.Email.Equals(email));

                return salesCustomers != null ? salesCustomers.Id : -1;
            }
        }

        public SalesCustomer GetApplicantById(int appId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var applicant = context.SalesOrders.Include("SalesCustomer")
                    .Include("SalesCustomer.SalesCustReferences")
                    .Include("SalesOrderDeliveries")
                    .Include("SalesDocuments")
                    //.Include("SalesInvoices")
                    .Include("SalesOrderCompletes").FirstOrDefault(p => p.Id == appId);
                if (applicant != null)
                {
                    var customer = applicant.SalesCustomer;
                    customer.Editable = applicant.Status == OrderStatus.Pending.GetHashCode();
                    if (applicant.Status >= (int)OrderStatus.PendingCusAccept)//from the "Pending Customer Acceptance" status
                    {
                        var orderDelivery = applicant.SalesOrderDeliveries.FirstOrDefault();
                        if (orderDelivery == null)
                        {
                            orderDelivery = new SalesOrderDelivery
                            {
                                OrderId = applicant.Id,
                                InitialDate = DateTime.Now,
                                DeliveryType = DeliveryType.StandardDelivery.GetHashCode()
                            };

                            applicant.SalesOrderDeliveries.Add(orderDelivery);
                            orderDelivery.CustomerSignImageUrl = "";
                            orderDelivery.DriverSignImageUrl = "";
                        }
                        else
                        {
                            if (orderDelivery.DeliveryType != DeliveryType.StandardDelivery.GetHashCode())
                            {
                                orderDelivery.DeliveryType = DeliveryType.StandardDelivery.GetHashCode();
                            }

                            if (orderDelivery.InitialDate == null)
                                orderDelivery.InitialDate = DateTime.Now;

                            //Get signature
                            orderDelivery.CustomerSignImageUrl = !string.IsNullOrEmpty(orderDelivery.CustomerSignature)
                                ? ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + orderDelivery.CustomerSignature + "?t=" +
                                  DateTime.Now.Ticks
                                : "";
                            orderDelivery.DriverSignImageUrl = !string.IsNullOrEmpty(orderDelivery.DriverSignature)
                                ? ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + orderDelivery.DriverSignature + "?t=" +
                                  DateTime.Now.Ticks
                                : "";
                        }
                        orderDelivery.CustomerSignIP = Utilities.GetClientIpAddress();
                        orderDelivery.DriverSignIP = Utilities.GetClientIpAddress();
                    }

                    //Get IP Client
                    applicant.SignatureIP = Utilities.GetClientIpAddress();

                    var status = context.Eli_ListValues.SingleOrDefault(s => s.Id == applicant.Status);
                    if (status != null)
                    {
                        applicant.StatusName = string.Format("<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>", status.Color, status.Description);
                        applicant.StatusDescription = status.AdditionalInfo;
                    }

                    if (!applicant.SalesOrderCompletes.Any())
                    {
                        var newSalesComplete = new SalesOrderComplete()
                        {
                            DeliveryDate = DateTime.Now,
                            SignIP = Utilities.GetClientIpAddress()
                        };
                        applicant.SalesOrderCompletes.Add(newSalesComplete);
                    }
                    else
                    {
                        var saleComplete = applicant.SalesOrderCompletes.First();
                        if (string.IsNullOrEmpty(saleComplete.Signature))
                        {
                            saleComplete.SignIP = Utilities.GetClientIpAddress();
                        }
                        else
                        {
                            saleComplete.CustomerSignatureUrl = ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.Signature + "?t=" + DateTime.Now.Ticks;
                        }

                        saleComplete.DeliverySignatureUrl = !string.IsNullOrEmpty(saleComplete.DeliverSignature) ? 
                                                                    ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.DeliverSignature + "?t=" + DateTime.Now.Ticks
                                                                    : "";

                        saleComplete.ManagerSignatureUrl = !string.IsNullOrEmpty(saleComplete.ManagerSignature) ?
                                                                   ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.ManagerSignature + "?t=" + DateTime.Now.Ticks
                                                                   : "";
                    }
                    
                    return customer;
                }
                return null;
            }
        }

        //get the contract content from applicant
        public string GetContractContent(int appId, string promotionText, bool onlyGetBody, bool isGetImageBase64, string signatureFolder = "")
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var result = "";
                var appObj = context.SalesOrders.Include("SalesCustomer").FirstOrDefault(x => x.Id == appId);
                
                //only get the body innerHtml 
                var stateId = appObj.SalesCustomer.PhysicalState.Value;
                var tempate = context.SalesContractStates.Include("SalesContractTemplate").FirstOrDefault(x=>x.StateId == stateId);
                result = tempate != null ? tempate.SalesContractTemplate.TemplateContent : "";

                if (!string.IsNullOrEmpty(result))
                {
                    if (onlyGetBody)
                    {
                        var startPos = result.IndexOf("<body>");
                        var endPos = result.IndexOf("</body>");
                        result = result.Substring(startPos, endPos - startPos).Replace("<body>", "");
                    }

                    var registry = context.vwRegistries.Where(x => x.Name == "DATE_FORMAT" || x.Name == "TIME_FORMAT").ToList();

                    var orderFields = context.vwViewColumns.Where(x => x.ModuleId == Constant.ModuleOrder).ToList();

                    result = result.Replace(Constant.ReplaceSignatureField, !string.IsNullOrEmpty(appObj.LesseeSignature) ?
                        (string.Format("<img src=\"{0}\" width=\"250\"/>", isGetImageBase64 && File.Exists(signatureFolder + "\\" + appObj.LesseeSignature) ?
                                                                           ImageHelper.GetImageBase64(signatureFolder + "\\" + appObj.LesseeSignature, "data:image/png;base64,") :
                                                                           ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + appObj.LesseeSignature + "?t=" + DateTime.Now.Ticks)) :
                        Constant.BlankSignature);

                    result = result.Replace(Constant.ReplaceCoSignatureField, !string.IsNullOrEmpty(appObj.CoSignature) ?
                        (string.Format("<img src=\"{0}\" width=\"250\"/>", isGetImageBase64 && File.Exists(signatureFolder + "\\" + appObj.CoSignature) ?
                                                                           ImageHelper.GetImageBase64(signatureFolder + "\\" + appObj.CoSignature, "data:image/png;base64,") :
                                                                           ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + appObj.CoSignature + "?t=" + DateTime.Now.Ticks)) :
                    Constant.BlankSignature);
                    ReplaceValue(appObj, "order_", orderFields, registry, context, promotionText, ref result);

                    var customerFields = context.vwViewColumns.Where(x => x.ModuleId == Constant.ModuleCustomer).ToList();
                    var customerObj = appObj.SalesCustomer;
                    ReplaceValue(customerObj, "customer_", customerFields, registry, context, promotionText, ref result);

                    ReplaceExpression(ref result);
                }

                return result;
            }
        }
        
        //get email customer or submitter
        public string GetCustomerEmail(int appId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var order = context.SalesOrders.Include("SalesCustomer").SingleOrDefault(x => x.Id == appId);
                if (order != null && order.SalesCustomer != null && (!string.IsNullOrEmpty(order.SalesCustomer.Email)))
                {
                    return order.SalesCustomer.Email;
                }
                else if (order != null)
                {
                    var createBy = order.CreatedBy;
                    var creator = context.Eli_User.Single(x => x.Id == createBy);
                    return creator.Email;
                }
                return "";
            }
        }

        #endregion

        #region Internal Method
        
        //Replace the content for the contract template
        private void ReplaceValue(object obj, string prefixTmpl, List<vwViewColumn> fields, List<vwRegistry> registries, LeonardUSAEntities context, string promotionText, ref string content)
        {            
            var props = obj.GetType().GetProperties();
            var dateFormat = registries.Single(x => x.Name == "DATE_FORMAT").Value;
            var datetimeFormat = registries.Single(x => x.Name == "DATE_FORMAT").Value + " " + registries.Single(x => x.Name == "TIME_FORMAT").Value;
            var setting = Config.GetConfiguration();

            var order = obj as SalesOrder;
            if (order != null)
            {
                content = content.Replace("@" + Constant.OrderPromotion + "@", order.IsAddPromotion == true ? ("<input type=\"checkbox\" checked type=\"checkbox\" disabled/>" + promotionText) : "");                                           
            }

            foreach (var prop in props)
            {
                var value = prop.GetValue(obj);
                var replaceValue = "";
                var field = fields.FirstOrDefault(x => x.ColumnName == prop.Name);
                if (field != null)
                {
                    if (value != null)
                    {
                        var valueType = prop.PropertyType;

                        if (field.IsDate)
                        {
                            replaceValue = (value as DateTime?).Value.ToString(dateFormat);
                        }
                        else if (field.IsDateTime)
                        {
                            replaceValue = (value as DateTime?).Value.ToString(datetimeFormat);
                        }
                        else if (field.IsList)
                        {
                            if (!field.ForeignKey.HasValue || !field.ForeignKey.Value)
                            {
                                var plValue = (int)value;
                                var plObject = context.vwListNameValues.FirstOrDefault(x => x.Id == plValue);
                                replaceValue = plObject != null ? plObject.Description : "";
                                content = content.Replace("@" + prefixTmpl + prop.Name + "_Addition@", plObject != null ? plObject.AdditionalInfo : "0");
                            }
                        }
                        else if (field.IsDecimal)
                        {
                            replaceValue = (value as decimal?).Value.ToString(Constant.DecimalFormat);
                        }
                        else if (field.IsCheckBox)
                        {
                            var chkValue = (value as bool?);
                            replaceValue = chkValue == true ? "Yes" : "No";
                        }
                        else
                        {
                            replaceValue = value.ToString();
                        }
                    }
                    else
                    {
                        if (field.IsDecimal)
                        {
                            replaceValue = "0.00";
                        }
                        else if (field.IsCheckBox)
                        {                           
                            replaceValue = "No";
                        }
                        else if (field.IsList)
                        {
                            if (!field.ForeignKey.HasValue || !field.ForeignKey.Value)
                            {
                                content = content.Replace("@" + prefixTmpl + prop.Name + "_Addition@", "0");
                            }
                        }
                    }
                }

                content = content.Replace("@" + prefixTmpl + prop.Name + "@", replaceValue);
            }

            //replace external field            
            if (order != null)
            {
                var isContractSigned = order.Status >= OrderStatus.PendingCusAccept.GetHashCode() && order.SignatureDate.HasValue;
                content = content.Replace("@" + Constant.CurrentDayExtField + "@", isContractSigned ? order.SignatureDate.Value.Day.ToString() : DateTime.Now.Day.ToString());
                content = content.Replace("@" + Constant.CurrentMonthExtField + "@", isContractSigned ? order.SignatureDate.Value.ToString("MMMM") : DateTime.Now.ToString("MMMM"));
                content = content.Replace("@" + Constant.CurrentYearExtField + "@", isContractSigned ? order.SignatureDate.Value.Year.ToString() : DateTime.Now.Year.ToString());
                content = content.Replace("@" + Constant.CurrentDateExtField + "@", isContractSigned ? order.SignatureDate.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat));
                content = content.Replace("@" + Constant.CurrentDateTimeExtField + "@", isContractSigned ? order.SignatureDate.Value.ToString(datetimeFormat) : DateTime.Now.ToString(datetimeFormat));
                content = content.Replace("@" + Constant.PercentOfCapitializationPeriod + "@", GetPercentByCapitialPeriod(setting, order.CapitalizationPeriod.GetValueOrDefault(0)).ToString());
            }
        }

        //Replace the expression 
        private void ReplaceExpression(ref string content)
        {
            var startExpessionPos = content.IndexOf("{{", 0, StringComparison.Ordinal);
            var endExpessionPos = content.IndexOf("}}", 0, StringComparison.Ordinal);

            var a = new ExpressionValue();
            var t = new TypeRegistry();
            t.RegisterSymbol("a", a);

            while (startExpessionPos > -1 && endExpessionPos > -1)
            {
                try
                {
                    var replaceString = content.Substring(startExpessionPos, (endExpessionPos + 2) - startExpessionPos);

                    var expression = new CompiledExpression { StringToParse = "a.Value = " + replaceString.Replace("{{", "")
                                                                                                          .Replace("}}", "")
                                                                                                          .Replace("==", "[**]").Replace("=", "").Replace("[**]", "==")//fix to excute the equal operator 
                                                                                                          .Replace(",", "")
                                                                                                          .Replace("&nbsp;", "") + ";", TypeRegistry = t };
                    expression.ExpressionType = CompiledExpressionType.StatementList;
                    expression.Eval();

                    var resultValue = a.Value;
                    var resultType = resultValue.GetType();                    
                    string result;
                    if (resultType == typeof(decimal) ||
                        resultType == typeof(float) ||
                        resultType == typeof(double))
                    {
                        var temp = decimal.Parse(resultValue.ToString());
                        result = temp == 0 ? "0.00" : temp.ToString(Constant.DecimalFormat);                            
                    }
                    else
                    {
                        result = resultValue.ToString();
                    }
                    content = content.Replace(replaceString, result);
                    
                    //update new expression
                    startExpessionPos = content.IndexOf("{{", 0, StringComparison.Ordinal);
                    endExpessionPos = content.IndexOf("}}", 0, StringComparison.Ordinal);
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.Message, ex);
                    break;
                }
            }
        }

        private double GetPercentByCapitialPeriod(NameValueCollection setting, int capitialPeriodId)
        {
            var config = setting.Get(string.Format(Constant.CapitializationPeriodConfigFormat, capitialPeriodId));
            double result;
            return double.TryParse(config, out result) ? result : 0;
        }
        #endregion

    }
}
