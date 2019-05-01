using System;
using System.IO;
using System.Web;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Eli.Common;
using System.Linq;
using System.Globalization;
using System.Threading;
using LeonardCRM.BusinessLayer.Feature;
using LeonardCRM.BusinessLayer.Helper;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SalesOrderApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("COMMON", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        #region API methods

        [HttpPost]
        public SalesOrder GetSalesOrderById([FromBody]string jsObject)
        {
            try
            {
                var references = new[] { "SalesDocuments", "SalesOrderCompletes", "SalesOrderDeliveries" };
                var entity = new SalesOrder();

                var param = jsObject.Split(',');
                if (param.Length > 0)
                {
                    int orderId = 0;
                    int moduleId = 0;
                    int.TryParse(param[0], out orderId);
                    int.TryParse(param[1], out moduleId);
                    if (orderId != 0)
                    {
                        entity = SalesOrderBM.Instance.SingleLoadWithReferences(order => order.Id == orderId, references);

                        if (entity != null && entity.SalesOrderCompletes != null && entity.SalesOrderCompletes.Any())
                        {
                            var saleComplete = entity.SalesOrderCompletes.First();
                            if (saleComplete != null)
                            {
                                if (saleComplete.FollowUpCall == null)
                                {
                                    saleComplete.FollowUpCall = true;
                                }

                                saleComplete.ManagerSignIP = Utilities.GetClientIpAddress();
                                saleComplete.ManagerSignatureUrl = !string.IsNullOrEmpty(saleComplete.ManagerSignature) ?
                                                                    ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.ManagerSignature + "?t=" + DateTime.Now.Ticks
                                                                    : "";
                                if (saleComplete.CallDate.HasValue && saleComplete.CallDate.Value.TimeOfDay.Ticks > 0)
                                {
                                    saleComplete.CallTime = saleComplete.CallDate.Value.ToString("HH:mm");
                                }
                            }
                        }

                        var customerRefs = new[] { "SalesCustReferences" };
                        var customerId = entity.CustomerId;

                        var customer = SalesCustomerBM.Instance.SingleLoadWithReferences(x => x.Id == customerId, customerRefs);
                        customer.IsStore = this.CurrentUserRole.Id == UserRoles.Store.GetHashCode();                        
                        entity.SalesCustomer = customer;

                    }

                    //This will be used for getting clients that a user owns
                    if (!string.IsNullOrEmpty(entity.ResponsibleUsers))
                    {                        
                        int n;
                        entity.UserIds = entity.ResponsibleUsers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.TryParse(s, out n) ? n : 0).ToArray();
                    }

                    entity.CustomFields = EntityFieldBM.Instance.GetCustomFieldByModuleId(moduleId, orderId, CurrentUserRole.Id);
                    entity.UsersPicklist = UserBM.Instance.GetResponsibleUserForApp();

                    if (entity.UsersPicklist != null && entity.UsersPicklist.Any())
                    {
                        entity.DeliveryUserIds = entity.UsersPicklist.Where(u => u.RoleId == UserRoles.DeliveryStaff.GetHashCode()).Select(u => u.Id).ToArray();
                    }

                    entity.IsAdmin = this.CurrentUserRole.IsHostAdmin || this.CurrentUserRole.Id == UserRoles.ContractManager.GetHashCode();
                    entity.IsAdminRoleUsers = this.CurrentUser.RoleId == UserRoles.ClientAdmin.GetHashCode() || this.CurrentUser.RoleId == UserRoles.Administrator.GetHashCode();                    
                }

                return entity;
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return null;
            }
        }

        [HttpPost]
        public virtual ResultObj DeleteSalesOrder([FromBody] JArray jsonArray)
        {
            try
            {
                int status = 0;
                var entities = JsonConvert.DeserializeObject<IList<SalesOrder>>(jsonArray.ToString());
                status = SalesOrderBM.Instance.Delete(entities);
                if (status > 0)
                {
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"), 0);
                }
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public virtual ResultObj CreateOrder([FromBody] JObject jsonObject, [FromUri] int moduleId)
        {
            try
            {
                var msg = string.Empty;
                var order = JsonConvert.DeserializeObject<SalesOrder>(jsonObject.ToString());
                string uploadFolderName = ConfigValues.UPLOAD_DIRECTORY;
                order.UploadDirectory = Path.Combine(HttpContext.Current.Server.MapPath(string.Format("~/{0}/", uploadFolderName)));

                //get the old data
                var orderId = order.Id;
                var oldOrder = order.Id > 0 ? SalesOrderBM.Instance.First(x => x.Id == orderId) : null;
                
                //set data
                SetObject(order, oldOrder);
               
                //validate
                msg = ValidateObject(order, oldOrder, moduleId);

                if (string.IsNullOrEmpty(msg))//valid
                {
                    var changesDescription = "";
                    var isChangeStatus = false;
                    var isChangeAssignees = false;

                    if (oldOrder != null)//update case
                    {
                        var oldStatus = oldOrder.Status;
                        isChangeStatus = oldStatus != order.Status;

                        //only tracking the changes in pharse which is from "Pending Customer Accepted" to In "Progress" status
                        if (oldStatus >= OrderStatus.PendingCusAccept.GetHashCode() && oldStatus <= OrderStatus.InProgress.GetHashCode())
                        {
                            changesDescription = TrackingOrderChanges(order, oldOrder);
                        }

                        //tracking the assigned delivery-mans
                        isChangeAssignees = TrackingReplaceOldAssignee(oldOrder, order);
                    }

                    var status = 0;
                    status = orderId != 0 //check if the applicant is updated
                                 ? SalesOrderBM.Instance.UpdateSalesOrders(order, HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SALE_DOCUMENT))
                                 : SalesOrderBM.Instance.CreateSalesOrders(order);

                    if (status > 0)//update success
                    {
                        var isSendMailSuccess = true;

                        //notify to submitter that have the changed infomation
                        if (!string.IsNullOrEmpty(changesDescription))
                        {
                            isSendMailSuccess = NotifyChangesToCustomer(order, changesDescription);
                        }

                        
                        if (order.Status == (int)OrderStatus.PendingCusAccept && 
                            string.IsNullOrEmpty(oldOrder.LesseeSignature) && !string.IsNullOrEmpty(order.LesseeSignature)) //contract is signed
                        {
                            //notify to submitter that require user to complete the Delivery Request Form
                            isSendMailSuccess = NotifyNeedCompleteDeliveryFormToCustomer(order);
                        }

                        if (isChangeStatus)
                        {
                            if (!(order.IsApproveOrder == false && order.Status == OrderStatus.InProgress.GetHashCode())) //Not Disapproved with Pending Delivery                                     
                            {
                                if (order.Status == OrderStatus.PendingDelivery.GetHashCode())// When app is approved
                                {
                                    isSendMailSuccess = NotifyApprovedToCustomer(order);
                                }
                                else//not yet approved or reject
                                {
                                    isSendMailSuccess = NotifyToCustomer(order, order.Status == OrderStatus.InProgress.GetHashCode() ||
                                                                                order.Status == OrderStatus.Rejected.GetHashCode(), // cc to store with InProgress or Cancel status
                                                                                order.Status == OrderStatus.Rejected.GetHashCode()); // cc to manager with Cancel status                                                                       
                                }
                            }
                            else
                            {
                                isSendMailSuccess = true;
                            }
                        }

                        if (isChangeAssignees)
                        {
                            var newDeliveries = order.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x))
                                                                    .Where(x => order.DeliveryUserIds.Contains(x)).ToArray();

                            if (newDeliveries != null && newDeliveries.Any())
                            {
                                var orderStatus = ListValueBM.Instance.GetById(order.Status);
                                isSendMailSuccess = NotifyToAssignee(newDeliveries, order, orderStatus);
                            }
                        }

                        if (oldOrder.Status >= OrderStatus.InProgress.GetHashCode() && order.Status >= OrderStatus.InProgress.GetHashCode() && //only check with Inprogress status or over
                            (order.IsApproveOrder != oldOrder.IsApproveOrder || oldOrder.DisapprovedReason != order.DisapprovedReason) && //have change the approved decision
                            order.IsApproveOrder.HasValue && !order.IsApproveOrder.Value)//disappove
                        {
                            isSendMailSuccess = NotifyDisapproveToCustomer(order);
                        }

                        return isSendMailSuccess ? new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), status) :
                                                   new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_SUCCESS_NOTSEND_MAIL_MESSAGE"), status);
                    }

                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), status);
                }

                return new ResultObj(ResultCodes.ValidationError, msg);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }


        [HttpGet]
        public IList<SalesOrder> GetRecentlyAddedOrders([FromUri] bool onlyMe)
        {
            try
            {
                return SalesOrderBM.Instance.GetRecentlyAddedOrders(CurrentUserID, CurrentUserRole.Id, onlyMe);
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
                return SalesOrderBM.Instance.GetReportDataByDays(CurrentUserID, CurrentUserRole.Id, onlyMe, dateFormat);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        public GoogleGraph GetOrderReportDashboard([FromBody] JObject jsonObject)
        {
            try
            {
                var reportObject = JsonConvert.DeserializeObject<ReportObject>(jsonObject.ToString());
                var idsString = string.Join(",", reportObject.UserIds);
                var dateFormat = SiteSettings.DATE_FORMAT;
                return SalesOrderBM.Instance.GetOrderReportDashboard(reportObject.FromDate, reportObject.ToDate, reportObject.Status, idsString, dateFormat);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpGet]
        public NgTableModel GetOwnedApplication([FromUri] bool assistMode = false)
        {
            try
            {
                return SalesOrderBM.Instance.GetApplicationByUser(CurrentUserID, CurrentUserRole.Id, assistMode);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new NgTableModel() { Total = 0, Data = null };
            }
        }

        [HttpGet]
        public ResultObj CancelApplication(int appId)
        {
            try
            {
                int status = SalesOrderBM.Instance.CancelApplication(appId);
                if (status > 0)
                {
                    var cancelStatus = OrderStatus.Rejected.GetHashCode();
                    var orderStatus = ListValueBM.Instance.First(s => s.Id == cancelStatus);
                    if (orderStatus != null)
                    {
                        if (SendCancelAppEmail(appId))
                        {
                            return new ResultObj(ResultCodes.Success, new
                                {
                                    Message = GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),
                                    Status = cancelStatus,
                                    StatusName = string.Format("<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>", orderStatus.Color, orderStatus.Description),
                                    StatusDescription = orderStatus.AdditionalInfo,
                                }, appId);
                        }

                        return new ResultObj(ResultCodes.SavingFailed, new { Message = GetText("ORDERS", "CANCEL_BUT_NOT_MAIL_ERROR_MSG") }, appId);
                    }
                }
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        //Save the electrical signature on contract
        [HttpPost]
        public ResultObj SaveSignature([FromBody] SalesOrder app, [FromUri] string t = null)
        {
            try
            {
                //check if params is valid
                if (!string.IsNullOrEmpty(t))
                {
                    var parts = SecurityHelper.Decrypt(t).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Count() < 2 || (int.Parse(parts[0]) != CurrentUserID) || (int.Parse(parts[1]) != CurrentUserRole.Id))
                    {
                        return new ResultObj(ResultCodes.ValidationError, LocalizeHelper.Instance.GetText("COMMON", "REQUEST_INVALID_ERROR_MSG"), 0);
                    }
                }

                bool isExistOrder;

                if (app.Status != OrderStatus.PendingCusAccept.GetHashCode())
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "NOT_PENDING_CUS_ACCEPTANCE_ERROR_MSG"));
                }

                var isExpectedStatus = SalesOrderBM.Instance.CheckExpectedStatus(app.Status, app.Id, out isExistOrder);

                if (!isExistOrder)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG"));
                }
                else if (isExistOrder && !isExpectedStatus)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG"));
                }

                SetSignature(app);                

                //store the signature image
                if (!string.IsNullOrEmpty(app.CustomerSignImage))
                {
                    ImageHelper.SaveImageFromBase64(app.CustomerSignImage.ToString(), HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + app.LesseeSignature));
                }

                if (!string.IsNullOrEmpty(app.CoCustomerSignImage))
                {
                    ImageHelper.SaveImageFromBase64(app.CoCustomerSignImage.ToString(), HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + app.CoSignature));
                }                

                int status = SalesOrderBM.Instance.SaveContractSignature(app);
                if (status > 0)
                {
                    var isExistDelivery = app.SalesOrderDeliveries != null &&
                                          app.SalesOrderDeliveries.Any() &&
                                          app.SalesOrderDeliveries.First().Id > 0;

                    var isSendMailSuccess = isExistDelivery ? OrderSendMailFeature.SendContractCopyToCustomer(app, ServerURL, CurrentUser) : true;

                    var orderStatus = ListValueBM.Instance.First(s => s.Id == app.Status);
                    if (orderStatus != null)
                    {
                        //check if the delivery form is completed or not.
                        var successMsg = app.Status == OrderStatus.PendingCusAccept.GetHashCode() &&
                                         isExistDelivery &&
                                         !string.IsNullOrEmpty(app.SalesOrderDeliveries.First().CustomerSignature) ? string.Format(GetText("APPLICANT_FORM", "DRIVER_LISCENSE_REQUIRE_MESSAGE"), GetText("APPLICANT_FORM", "LANDLORD_WAIVER_FORM_ACCEPT")) : 
                                                                                                                     GetText("COMMON", "SAVE_SUCCESS_MESSAGE");

                        return new ResultObj(ResultCodes.Success, new
                        {
                            Message = isSendMailSuccess ? successMsg : GetText("APPLICANT_FORM", "SAVE_SUCCESS_SEND_MAIL_FAIL_MESSAGE"),
                            Status = app.Status,
                            StatusName = string.Format("<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>", orderStatus.Color, orderStatus.Description),
                            StatusDescription = orderStatus.AdditionalInfo,
                        }, 0);
                    }
                }
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public ResultObj FinalizeOrer([FromBody] SalesOrder app)
        {
            try
            {
                bool isExistOrder;

                if (app.Status != OrderStatus.Completed.GetHashCode())
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "NOT_COMPLETE_ERROR_MSG"));
                }

                var isExpectedStatus = SalesOrderBM.Instance.CheckExpectedStatus(app.Status, app.Id, out isExistOrder);

                if (!isExistOrder)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG"));
                }
                else if (isExistOrder && !isExpectedStatus)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG"));
                }

                SetManagerSignature(app);
                var appId = app.Id;
                var msg = ValidateObject(app, SalesOrderBM.Instance.Single(x=>x.Id == appId), Constant.ModuleOrder);
                if (string.IsNullOrEmpty(msg))
                {
                    var contractPDFPath = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP + "/" + string.Format(Constant.ContractFileNameFormat, app.Id));
                    if (File.Exists(contractPDFPath))
                    {
                        File.Delete(contractPDFPath);
                    }

                    int status = SalesOrderBM.Instance.UpdateSalesOrders(app);
                    if (status > 0)
                    {                       
                        return new ResultObj(ResultCodes.Success, GetText("COMMON", "SAVE_SUCCESS_MESSAGE"));
                    }
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
                }

                return new ResultObj(ResultCodes.ValidationError, msg, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpGet]
        public ResultObj GetPDFByForm([FromUri]int formType, [FromUri]int appId, [FromUri]bool isGetExistFilePath = false, [FromUri]bool isFinal = false)
        {
            string physicalPath = "";

            try
            {
                var resultPath = "";
                switch (formType)
                {
                    case 0://Get PDF file for contract
                        resultPath = GetContractPDFPath(appId, isGetExistFilePath, ref physicalPath);
                        break;

                    case 1://Get PDF file for delivery form
                        resultPath = GetDeliveryPDFPath(appId, isGetExistFilePath, ref physicalPath);
                        break;

                    case 2://Get PDF file for acceptance form
                        resultPath = GetAcceptancePDFPath(appId, isFinal, ref physicalPath);
                        break;
                }

                if (!string.IsNullOrEmpty(resultPath))
                {
                    return new ResultObj(ResultCodes.Success, resultPath, appId);
                }
                else
                {
                    return new ResultObj(ResultCodes.SavingFailed, GetText("ORDERS", "GET_PDF_FAIL_ERROR_MSG"), appId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public OverdueApps GetOverdueApp([FromBody] FilterOverdueAppsParams filterParams, [FromUri]int pageIndex, [FromUri]int pageSize, [FromUri]string sortDesc, [FromUri] int overdueMonth = 0)
        {
            try
            {
                if (pageIndex <= 0 || pageSize <= 0 || overdueMonth <= 0)
                {
                    return new OverdueApps()
                    {
                        ReturnCode = ResultCodes.Success,                        
                        Data = new List<sp_GetAllOverdueApps_Result>(),
                        Total = 0
                    };
                }

                var result = SalesOrderBM.GetOverdueApp(pageIndex, pageSize, sortDesc, overdueMonth, filterParams);

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new OverdueApps()
                {
                    ReturnCode = ResultCodes.UnkownError,
                    Message = GetText("UNEXPECTED_ERROR_MESSAGE_USER"),
                    Data = new List<sp_GetAllOverdueApps_Result>(),
                    Total = 0
                };
            }
        }

        [HttpGet]
        public ResultObj DeleteOverdueApps([FromUri] int overdueMonth, [FromUri] int total)
        {
            try
            {
                if (overdueMonth <= 0)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("OVERDUE_APPLICANTS", "EMPTY_MONTH_ERROR_MSG"));
                }

                var status = SalesOrderBM.Instance.DeleteOverdueApp(overdueMonth, total, new FilterOverdueAppsParams());
                return status > 0 ? new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS")) :
                                    new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"));

            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        // Save delivery signature
        public ResultObj SaveDeliverySignature([FromBody] SalesOrder app)
        {
            try
            {
                bool isExistOrder;

                if (app.Status != OrderStatus.PendingCusAccept.GetHashCode())
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "NOT_PAID_FULL_ERROR_MSG"));
                }

                var isExpectedStatus = SalesOrderBM.Instance.CheckExpectedStatus(app.Status, app.Id, out isExistOrder);

                if (!isExistOrder)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG"));
                }
                else if (isExistOrder && !isExpectedStatus)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG"));
                }

                //set data
                SetDeliverySignature(app);

                var orderDelivery = app.SalesOrderDeliveries.FirstOrDefault();
                if (orderDelivery != null)
                {
                    if (!string.IsNullOrEmpty(orderDelivery.CustomerSignImage))
                    {
                        //save the signature image
                        ImageHelper.SaveImageFromBase64(orderDelivery.CustomerSignImage,
                            HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" +
                                                               orderDelivery.CustomerSignature));
                    }

                    //return the result to client
                    return new ResultObj(ResultCodes.Success, new
                    {
                        Message = GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),
                        Applicant = app
                    }, 0);
                }
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpGet]
        public ResultObj CloneApp([FromUri] int appId)
        {
            try
            {                
                var newAppId = SalesOrderBM.Instance.CloneApp(appId, this.CurrentUserID);
                return newAppId > 0 ? new ResultObj(ResultCodes.Success, GetText("ORDERS", "CLONE_APP_SUCCESS"), newAppId) :
                                      new ResultObj(ResultCodes.SavingFailed, GetText("ORDERS", "CLONE_APP_FALSE"), newAppId);
            }
            catch(Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }
        #endregion

        #region Internal method
        private void SetObject(SalesOrder order, SalesOrder oldOrder)
        {            
            //Handle for the customer
            if (order.SalesCustomer != null)
            {
                //set audit fields for the customer
                SetAuditFields(order.SalesCustomer, order.SalesCustomer.Id);

                if (order.SalesCustomer.SalesCustReferences != null &&
                    order.SalesCustomer.SalesCustReferences.Any())
                {
                    order.SalesCustomer.SalesCustReferences = order.SalesCustomer.SalesCustReferences.Where(x => !string.IsNullOrEmpty(x.Name) &&
                                                                                                                 !string.IsNullOrEmpty(x.Phone)).ToList();
                    foreach (var cusRef in order.SalesCustomer.SalesCustReferences)
                    {
                        if (cusRef.Id == 0)
                        {
                            cusRef.CustomerId = order.SalesCustomer.Id;
                        }
                        SetAuditFields(cusRef, cusRef.Id);
                    }
                }
            }

            if (order.Status == OrderStatus.PreApproved.GetHashCode() && 
                !string.IsNullOrEmpty(order.PartNumber) && order.CapitalizationPeriod != null && 
                order.SalesPrice != null && order.SalesPrice > 0 &&
                order.SaleTax != null && order.SaleTax > 0 && !string.IsNullOrEmpty(order.POSTicketNumber))
            {
                order.Status = OrderStatus.PendingCusAccept.GetHashCode();
            }

            //Handle for the responsible users
            if (order.UserIds != null && order.UserIds.Count() > 0)
            {
                if (!order.UserIds.Contains(CurrentUserID) && CurrentUserID != 1 && CurrentUserRole.Id != (int)UserRoles.Customer)
                {
                    order.UserIds = order.UserIds.Union(new[] { CurrentUserID }).ToArray();
                }
                order.ResponsibleUsers = String.Join(",", order.UserIds) + ",";
            }
            else
            {
                order.ResponsibleUsers = null;
            }

            if (order.ResponsibleUsers != oldOrder.ResponsibleUsers)
            {
                order.DriverAssigned = SalesOrderBM.Instance.CheckDriverAssigned(order.UserIds);
            }

            //contract manager approve this "in-progress" order
            if (this.CurrentUserRole.Id == UserRoles.ContractManager.GetHashCode() &&
                order.Status == OrderStatus.InProgress.GetHashCode() &&
                order.IsApproveOrder == true)
            {
                order.Status = OrderStatus.PendingDelivery.GetHashCode();
            }

            //Handle for the order complete
            if (order.SalesOrderCompletes != null && order.SalesOrderCompletes.Any())
            {
                var saleComplete = order.SalesOrderCompletes.First();
                if (saleComplete != null && saleComplete.CallDate.HasValue)
                {
                    DateTime tempDate;
                    if (DateTime.TryParseExact(saleComplete.CallDate.Value.Date.ToString(SiteSettings.DATE_FORMAT) + (!string.IsNullOrEmpty(saleComplete.CallTime) ? (" " + saleComplete.CallTime) : ""), 
                                               SiteSettings.DATE_FORMAT + (!string.IsNullOrEmpty(saleComplete.CallTime) ? " HH:mm" : ""), CultureInfo.CurrentCulture, DateTimeStyles.None, out tempDate))
                    {
                        saleComplete.CallDate = tempDate;
                    }
                    else
                    {
                        saleComplete.CallDate = saleComplete.CallDate.Value;
                    }

                    //clear the manager signature IP and date
                    if (string.IsNullOrEmpty(saleComplete.ManagerSignature))
                    {
                        saleComplete.ManagerSignIP = null;
                        saleComplete.ManagerSignDate = null;
                    }
                    //set audit fields for the order complete
                    SetAuditFields(saleComplete, saleComplete.Id);
                }
            }

            //set audit for customer field
            SetAuditFields(order, order.Id);
            foreach (var field in order.FieldData)
            {
                SetAuditFields(field, field.Id);
            }

            //set audit for notes
            order.Notes = order.Notes ?? new List<Eli_Notes>();
            foreach (var note in order.Notes)
            {
                SetAuditFields(note, note.Id);
            }
        }

        private void SetSignature(SalesOrder app)
        {
            if (!string.IsNullOrEmpty(app.CustomerSignImage))
            {
                app.SignatureIP = Utilities.GetClientIpAddress();
                app.SignatureDate = DateTime.Now;
                app.LesseeSignature = "Signature_" + app.Id + ".png";
            }

            if (!string.IsNullOrEmpty(app.CoCustomerSignImage))
            {
                app.CoSignatureIP = Utilities.GetClientIpAddress();
                app.CoSignatureDate = DateTime.Now;
                app.CoSignature = "CoSignature_" + app.Id + ".png";
            }

            if (!string.IsNullOrEmpty(app.CustomerSignImage) || !string.IsNullOrEmpty(app.CoCustomerSignImage))
            {
                SetAuditFields(app, app.Id);
            }

            if (app.SalesOrderCompletes != null && app.SalesOrderCompletes.Any())
            {
                app.SalesOrderCompletes.Clear();
            }

            var realatedCustomer = SalesCustomerBM.Instance.GetById(app.CustomerId);
            if (!string.IsNullOrEmpty(app.LesseeSignature) && //require the applicant's signature
                (string.IsNullOrEmpty(realatedCustomer.CoName) || !string.IsNullOrEmpty(app.CoSignature)) && //require the siganture if have the co-customer
                (app.SalesOrderDeliveries != null && app.SalesOrderDeliveries.Any() && app.SalesOrderDeliveries.First().Id > 0 &&  //require delivery
                 !string.IsNullOrEmpty(app.SalesOrderDeliveries.First().CustomerSignature)))  //require signature on delivery
            {
                bool isRequireWaiver;
                int docsRequired;
                SalesOrderBM.Instance.SetRequireWaiverAndDocs(realatedCustomer, out isRequireWaiver, out docsRequired);
                var appId = app.Id;

                if(SalesDocumentsBM.Instance.Count(x=>x.OrderId == appId) >= docsRequired)//require number of docs
                {
                    app.Status = OrderStatus.InProgress.GetHashCode();
                }                
            }
            else
            {
                app.Status = OrderStatus.PendingCusAccept.GetHashCode();
            }
        }

        private void SetDeliverySignature(SalesOrder app)
        {
            var orderDelivery = app.SalesOrderDeliveries.First();
            if (!string.IsNullOrEmpty(orderDelivery.CustomerSignImage))
            {
                //update the signature data
                orderDelivery.CustomerSignIP = Utilities.GetClientIpAddress();
                orderDelivery.CustomerSignDate = DateTime.Now;
                orderDelivery.CustomerSignature = string.Format(Constant.DeliveryCustomerSignName, app.Id);
            }
        }

        private string ValidateObject(SalesOrder order, SalesOrder oldOrder, int moduleId)
        {
            string msg = string.Empty;

            //check if status is changed
            if (oldOrder != null && order.OrginalStatus != oldOrder.Status)
            {
                return GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG");
            }

            var validator = new ObjectValidator(moduleId);
            var result = validator.ValidateObject(order);
            if (result.Length > 0)
            {
                msg += result;
            }

            if (order.SalesCustomer != null)
            {
                var customerValidator = new ObjectValidator(Constant.ModuleCustomer);
                result = order.IsSold.GetValueOrDefault(false) ? customerValidator.ValidateObject(order.SalesCustomer, new string[] { "Name", "Email", "HomePhone", "CellPhone", "MailingStreet", "MailingCity", "MailingZip", "MailingState", "PhysicalStreet", "PhysicalCity", "PhysicalZip", "PhysicalState" }) :
                                                                customerValidator.ValidateObject(order.SalesCustomer);
                if (result.Length > 0)
                {
                    msg += result;
                }

                var cusRefValidator = new ObjectValidator(Constant.ModuleCustomerReferences);
                foreach (var cusRef in order.SalesCustomer.SalesCustReferences)
                {
                    result = cusRefValidator.ValidateObject(cusRef);
                    if (result.Length > 0)
                    {
                        msg += result;
                    }
                }
            }

            if (order.IsSold != true && 
                order.Status >= OrderStatus.PreApproved.GetHashCode() && 
                order.Status != OrderStatus.Rejected.GetHashCode() && 
                order.IsApproveOrder != false )
            {

                if (order.CapitalizationPeriod == null || order.CapitalizationPeriod <= 0)
                {
                    msg += GetText("ORDERS", "CAPITALIZATION_REQUIRE_ERROR_MSG") + "<br/>";
                }

                if (order.SalesPrice == null || order.SalesPrice <= 0)
                {
                    msg += GetText("ORDERS", "SALE_PRICE_REQUIRE_ERROR_MSG") + "<br/>";
                }

                if (order.Tax == null || order.Tax <= 0)
                {
                    msg += GetText("ORDERS", "TAX_REQUIRE_ERROR_MSG") + "<br/>";
                }
            }

            if (order.SalesOrderCompletes != null && order.SalesOrderCompletes.Any())
            {
                var saleComplete = order.SalesOrderCompletes.First();
                var completeValidator = new ObjectValidator(Constant.ModuleSaleComplete);
                msg += completeValidator.ValidateObject(saleComplete);
            }
            
            // Check if POS Ticket # must input before changing to "Pending Customer Acceptance"
            if (order.IsSold != true && 
                (order.Status >= OrderStatus.PendingCusAccept.GetHashCode()) && 
                (order.Status != OrderStatus.Rejected.GetHashCode()) && 
                string.IsNullOrWhiteSpace(order.POSTicketNumber)) 
            {
                msg += GetText("DELIVERY_REQUEST_FORM", "POST_TICKET_NUMBER_REQUIRED") + " <br/>";
            }

            //Require input Part # before changing to "Pending Customer Acceptance"
            if (order.Id > 0 &&
                order.Status >= OrderStatus.PendingCusAccept.GetHashCode() &&
                order.Status != OrderStatus.Rejected.GetHashCode())
            {
                if (string.IsNullOrEmpty(order.PartNumber))
                {
                    msg += GetText("ORDERS", "PART_NUMBER_REQUIRE_MSG") + " <br/>";
                }
            }

            //Require input Serial # before changing to Pending Delivery
            if (order.Id > 0 && 
                order.Status >= OrderStatus.PendingDelivery.GetHashCode() && (order.Status != OrderStatus.Rejected.GetHashCode()) &&
                string.IsNullOrEmpty(order.SerialNumber))
            {
                msg += GetText("ORDERS", "CHANGE_DELIVERY_PENDING_STATUS_ERROR_MSG") + " <br/>";
            }

            if (order.Id > 0 &&
                order.IsSold != true && 
                order.Status >= OrderStatus.Completed.GetHashCode() && 
                order.Status != OrderStatus.Rejected.GetHashCode() && 
                string.IsNullOrEmpty(order.GPCustomerID))
            {
                msg += GetText("ORDERS", "GP_CUSTOMER_NUMBER_REQUIRE_MSG") + " <br/>";
            }

            if (order.IsAddPromotion.GetValueOrDefault(false) && !order.PromotionAmt.HasValue)
            {
                msg += GetText("ORDERS", "MISSING_PROMOTION_AMT_ERROR_MSG") + " <br/>";
            }

            if (order.Status >= OrderStatus.InProgress.GetHashCode() && //only check with inprogress status or over
                oldOrder != null &&
               (oldOrder.IsApproveOrder != order.IsApproveOrder || oldOrder.DisapprovedReason != order.DisapprovedReason) && //changed the approved decision                 
                order.IsApproveOrder.HasValue && order.IsApproveOrder == false && string.IsNullOrEmpty(order.DisapprovedReason))//disapproved
            {
                msg += GetText("ORDERS", "DISAPPROVE_REASON_REQUIRED_MSG") + " <br/>";
            }

            return msg;
        }
                

        // Notify customer to the applicant status
        private bool NotifyToCustomer(SalesOrder order, bool isSendToToManager = false, bool isCcToStore = false, bool isCcToManager = false)
        {
            try
            {
                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT_TO_CUSTOMER.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);                

                //email & name
                string email, name;
                if (order.SalesCustomer != null &&
                    !string.IsNullOrEmpty(order.SalesCustomer.Email) &&
                    !string.IsNullOrEmpty(order.SalesCustomer.Name))
                {
                    email = order.SalesCustomer.Email;
                    name = order.SalesCustomer.Name;
                }
                else
                {
                    //get submmitter
                    var user = UserBM.Instance.GetById(order.CreatedBy);
                    email = user.Email;
                    name = user.Name;
                }

                //cc to store if need
                var storeEmails = isCcToStore ? UserBM.Instance.GetStoreEmailList(order.StoreNumber.Value, isCcToManager) : "";               

                var status = ListValueBM.Instance.GetById(order.Status);

                //build mail content
                var content = template.TemplateContent;
                content = content.Replace(Constant.UserName, name);
                content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());
                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());
                content = content.Replace(Constant.ApplicantStatus, status.Description);
                content = content.Replace(Constant.HomeLink, this.ServerURL);

                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                MailHelper.SendAsyncEmail(mailServer, "", mailServer.Username, email, storeEmails, template.Subject, content, false);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }

        private bool NotifyNeedCompleteDeliveryFormToCustomer(SalesOrder order)
        {
            try
            {
                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_NOTIFY_COMPLETE_DELIVERY_REQUEST.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                //email & name
                string email, name;
                if (order.SalesCustomer == null)
                {
                    order.SalesCustomer = SalesCustomerBM.Instance.GetById(order.CustomerId);
                }

                if (!string.IsNullOrEmpty(order.SalesCustomer.Email) &&
                    !string.IsNullOrEmpty(order.SalesCustomer.Name))
                {
                    email = order.SalesCustomer.Email;
                    name = order.SalesCustomer.Name;
                }
                else
                {
                    //get submmitter
                    var user = UserBM.Instance.GetById(order.CreatedBy);
                    email = user.Email;
                    name = user.Name;
                }


                var status = ListValueBM.Instance.GetById(order.Status);

                //build mail content
                var content = template.TemplateContent;
                content = content.Replace(Constant.UserName, name);
                content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());
                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());
                content = content.Replace(Constant.ApplicantStatus, status.Description);
                content = content.Replace(Constant.HomeLink, this.ServerURL);
                
                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                MailHelper.SendAsyncEmail(mailServer, "", mailServer.Username, email, template.Subject, content);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }

        private bool NotifyChangesToCustomer(SalesOrder order, string changesDescription)
        {
            //get customer
            var user = UserBM.Instance.GetById(order.CreatedBy);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(changesDescription))//have any changes
                {
                    try
                    {
                        //create mail server
                        var mailServer = RegistryBM.Instance.GetMailServerInfo();

                        //get mail template
                        var tmpName = MailTemplates.TEMPLATE_MAIL_CHANGES_APPLICATION.ToString();
                        var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);


                        //build mail subject
                        var subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());

                        //build mail content
                        var content = template.TemplateContent.Replace(Constant.ApplicantNumber, order.Id.ToString());

                        var statusId = order.Status;
                        var statusObj = ListValueBM.Instance.First(x => x.Id == statusId);
                        content = content.Replace(Constant.ApplicantStatus, statusObj.Description);
                        content = content.Replace(Constant.Description, changesDescription);
                        content = content.Replace(Constant.HomeLink, this.ServerURL);
                        
                        // get first user by store
                        var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                        content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                        MailHelper.SendMail(mailServer, mailServer.Username, user.Email, subject, content);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log(ex.Message, ex);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool NotifyDisapproveToCustomer(SalesOrder order)
        {            
            var emailList = order.StoreNumber.HasValue ? UserBM.Instance.GetStoreEmailList(order.StoreNumber.Value, order.Status == OrderStatus.Rejected.GetHashCode()) : "";
            var customerEmail = order.SalesCustomer != null && !string.IsNullOrEmpty(order.SalesCustomer.Email) ? order.SalesCustomer.Email : SalesCustomerBM.Instance.GetCustomerEmail(order.Id);

            if (!string.IsNullOrEmpty(customerEmail))
            {
                try
                {
                    //create mail server
                    var mailServer = RegistryBM.Instance.GetMailServerInfo();

                    //get mail template
                    var tmpName = MailTemplates.TEMPLATE_MAIL_DISAPPOVE_APPLICANT.ToString();
                    var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                    //build mail subject
                    var subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());

                    //build mail content
                    var content = template.TemplateContent.Replace(Constant.ApplicantNumber, order.Id.ToString());

                    content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());
                    content = content.Replace(Constant.DisapproveReason, order.DisapprovedReason);
                    
                    // get first user by store
                    var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                    content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                    MailHelper.SendMailWithAttachments(mailServer, mailServer.Username, customerEmail,
                        (!string.IsNullOrEmpty(emailList) ? emailList : "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        subject, content, null, "", mailServer.Password);
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex.Message, ex);
                    return false;
                }
            }

            return true;
        }

        private string TrackingOrderChanges(SalesOrder order, SalesOrder oldOrder)
        {
            var entityFields = EntityFieldBM.Instance.GetViewColumnsByModule(Constant.ModuleOrder, true, CurrentUserRole.Id);
            var changesDescription = "";
            var orderType = order.GetType();
            var formatChangeText = "{0} : {1} -> {2} <br/>";
            var noTrackingFields = new string[] { "CreatedDate", "CreatedBy", "ModifiedDate", "ModifiedBy", "IsActive", "SerialNumber", "ResponsibleUsers", 
                                                  "RentToOwn", "SaleDate", "Color", "POSTicketNumber", "Status", "GPCustomerID", "GPOrderNumber", "AcceptDate", "PromoCode",
                                                  "IsOldCustomer", "PartNumber", "StoreNumber", "IsNewPart", "RampPartNumber", "DisapprovedReason", "RampSalePrice", "IsApproveOrder", "DisapprovedReason", "DriverAssigned" };

            foreach (var field in entityFields)
            {
                if (noTrackingFields.Contains(field.ColumnName)) continue;

                var property = orderType.GetProperty(field.ColumnName);
                if (property != null)
                {
                    var newVal = property.GetValue(order);
                    var oldVal = property.GetValue(oldOrder);
                    if ((oldVal == null && newVal != null) ||
                        (oldVal != null && newVal == null) ||
                        (newVal != null && oldVal != null && ((!field.IsDecimal && newVal.ToString() != oldVal.ToString()) ||
                                                              (field.IsDecimal && ((decimal)oldVal) != ((decimal)newVal)))))
                    {
                        if (field.IsDate)
                        {
                            changesDescription += string.Format(formatChangeText,
                                                                field.LabelDisplay,
                                                                oldVal != null ? ((DateTime)oldVal).ToString(SiteSettings.DATE_FORMAT) : "",
                                                                newVal != null ? ((DateTime)newVal).ToString(SiteSettings.DATE_FORMAT) : "");
                        }
                        else if (field.IsDateTime)
                        {
                            changesDescription += string.Format(formatChangeText,
                                                               field.LabelDisplay,
                                                               oldVal != null ? ((DateTime)oldVal).ToString(SiteSettings.DATE_FORMAT + " " + SiteSettings.TIME_FORMAT) : "",
                                                               newVal != null ? ((DateTime)newVal).ToString(SiteSettings.DATE_FORMAT + " " + SiteSettings.TIME_FORMAT) : "");
                        }
                        else if (field.IsList)
                        {
                            if (field.ForeignKey != true)
                            {
                                var newIntVal = newVal != null ? (int)newVal : 0;
                                var oldIntVal = oldVal != null ? (int)oldVal : 0;
                                var vals = ListValueBM.Instance.Find(x => x.Id == newIntVal || x.Id == oldIntVal);
                                var newObj = vals.FirstOrDefault(x => x.Id == newIntVal);
                                var oldObj = vals.FirstOrDefault(x => x.Id == oldIntVal);
                                changesDescription += string.Format(formatChangeText,
                                                             field.LabelDisplay,
                                                             oldObj != null ? oldObj.Description : "",
                                                             newObj != null ? newObj.Description : "");
                            }
                        }
                        else
                        {
                            changesDescription += string.Format(formatChangeText,
                                                               field.LabelDisplay,
                                                               oldVal != null ? oldVal.ToString() : "",
                                                               newVal != null ? newVal.ToString() : "");
                        }
                    }
                }

            }
            if (!string.IsNullOrEmpty(changesDescription))//have any changes
            {
                //update info
                order.Status = OrderStatus.PendingCusAccept.GetHashCode();
                order.SignatureIP = null;
                order.SignatureDate = null;
                order.LesseeSignature = null;
                order.CoSignatureIP = null;
                order.CoSignatureDate = null;
                order.CoSignature = null;
                order.IsApproveOrder = null;
                order.DisapprovedReason = null;                                
            }

            return changesDescription;
        }

        private void SetManagerSignature(SalesOrder app)
        {
            if (app.SalesOrderCompletes != null && app.SalesOrderCompletes.Any())
            {
                var saleComplete = app.SalesOrderCompletes.First();
                //if (saleComplete != null && !string.IsNullOrEmpty(saleComplete.ManagerSignatureUrl))
                //{
                //    DateTime tempDate;
                //    if (DateTime.TryParseExact(saleComplete.CallDate.Value.ToString(SiteSettings.DATE_FORMAT) + " " + saleComplete.CallTime, SiteSettings.DATE_FORMAT + " HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out tempDate))
                //    {
                //        saleComplete.CallDate = tempDate;
                //    }
                //    else
                //    {
                //        saleComplete.CallDate = saleComplete.CallDate.Value;
                //    }

                //    saleComplete.ManagerSignIP = Utilities.GetClientIpAddress();
                //    saleComplete.ManagerSignDate = DateTime.Now;
                //    saleComplete.ManagerSignature = "ManagerSign_" + app.Id + ".png";
                //    ImageHelper.SaveImageFromBase64(saleComplete.ManagerSignatureUrl.ToString(), HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.ManagerSignature));
                //    app.IsFinalize = true;
                //    app.FieldData = null;

                //    SetAuditFields(saleComplete, saleComplete.Id);
                //    SetAuditFields(app, app.Id);
                //}

                app.IsFinalize = true;
                app.FieldData = null;

                SetAuditFields(saleComplete, saleComplete.Id);
                SetAuditFields(app, app.Id);
            }
        }

        private string GetContractPDFPath(int appId, bool isGetExistFilePath, ref string physicalPath)
        {
            var res = ConfigValues.UPLOAD_DIRECTORY_TEMP + "/" + string.Format(Constant.ContractFileNameFormat, appId);
            physicalPath = HttpContext.Current.Server.MapPath(res);
            if (!isGetExistFilePath || !File.Exists(physicalPath))
            {
                if (string.IsNullOrEmpty(OrderPdfFeature.CreateContractFile(appId))) return "";
            }

            return this.ServerURL + res + "?t=" + DateTime.Now.Ticks;
        }

        private string GetDeliveryPDFPath(int appId, bool isGetExistFilePath, ref string physicalPath)
        {
            var fileName = string.Format(Constant.DeliveryFormFileNameFormat, appId);
            var res = ConfigValues.UPLOAD_DIRECTORY_TEMP + "/" + fileName;
            physicalPath = HttpContext.Current.Server.MapPath(res);
            if (!isGetExistFilePath || !File.Exists(physicalPath))
            {
                HtmlPdfConverter.ConvertWebpageToPdf(physicalPath, this.ServerURL + ConfigValues.DELIVERY_WEBFORM_URL + HttpUtility.UrlEncode(SecurityHelper.Encrypt(appId.ToString())));
            }
            return this.ServerURL + res + "?t=" + DateTime.Now.Ticks;
        }

        private string GetAcceptancePDFPath(int appId, bool isFinal, ref string physicalPath)
        {
            //check if the application is finalized.
            if (isFinal)
            {
                var order = SalesOrderBM.Instance.Single(x => x.Id == appId);
                isFinal = order.IsFinalize == true;
            }

            var fileName = string.Format(isFinal ? Constant.FinalNameFormat : Constant.AcceptFormFileNameFormat, appId);
            var res = ConfigValues.UPLOAD_DIRECTORY_TEMP + "/" + fileName;
            physicalPath = HttpContext.Current.Server.MapPath(res);

            if (!File.Exists(physicalPath))
            {
                HtmlPdfConverter.ConvertWebpageToPdf(physicalPath, this.ServerURL + ConfigValues.CUSTOMER_ACCEPTANCE_WEBFORM_URL + HttpUtility.UrlEncode(SecurityHelper.Encrypt(appId.ToString())));
            }
            return this.ServerURL + res + "?t=" + DateTime.Now.Ticks;
        }

        private bool TrackingReplaceOldAssignee(SalesOrder oldOrder, SalesOrder newOrder)
        {
            var result = false;
            var oldAssignees = !string.IsNullOrEmpty(oldOrder.ResponsibleUsers) ? oldOrder.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(x => newOrder.DeliveryUserIds.Contains(int.Parse(x))) : 
                                                                                 new string[] { };

            var newAssignees = !string.IsNullOrEmpty(newOrder.ResponsibleUsers) ? newOrder.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(x => newOrder.DeliveryUserIds.Contains(int.Parse(x))) :
                                                                                 new string[] { };
            result = (oldAssignees.Any() || newAssignees.Any()) && !oldAssignees.Intersect(newAssignees).Any();

            return result;
        }

        private bool NotifyToAssignee(int[] userIds, SalesOrder app, Eli_ListValues status)
        {
            try
            {
                var appId = app.Id;

                if (userIds != null && userIds.Any())
                {
                    var assignees = UserBM.Instance.Find(x => userIds.Contains(x.Id));
                    

                    var email = string.Join(",", assignees.Select(x => x.Email));
                    if (app.IsSold == true)
                    {
                        if(app.SalesCustomer == null)
                        {
                            var customerId = app.CustomerId;
                            app.SalesCustomer = SalesCustomerBM.Instance.Single(x => x.Id == customerId);
                        }                       
                    }
                    
                    //create mail server
                    var mailServer = RegistryBM.Instance.GetMailServerInfo();

                    //get mail template
                    var tmpName = MailTemplates.TEMPLATE_MAIL_NOTIFY_CHANGE_ASSIGNEE.ToString();
                    var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                    var content = template.TemplateContent;
                    content = content.Replace(Constant.ApplicantNumber, appId.ToString());
                    template.Subject = template.Subject.Replace(Constant.ApplicantNumber, appId.ToString());
                    content = content.Replace(Constant.ApplicantStatus, status.Description);
                    content = content.Replace(Constant.HomeLink, this.ServerURL);
                    
                    // replace [UserName] by sale customer name
                    content = content.Replace(Constant.UserName, app.SalesCustomer.Name);

                    // get first user by store
                    var firstUser = app.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(app.StoreNumber.Value) : null;
                    content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                    MailHelper.SendAsyncEmail(mailServer, "", mailServer.Username, email, app.SalesCustomer != null ? app.SalesCustomer.Email : null, template.Subject, content, false);                  
                }                

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }

        private bool SendCancelAppEmail(int appId)
        {
            try
            {
                var customerEmail = SalesCustomerBM.Instance.GetCustomerEmail(appId);
                var app = SalesOrderBM.Instance.GetById(appId);
                var ccEmail = UserBM.Instance.GetStoreEmailList(app.StoreNumber.Value, true);

                if (!string.IsNullOrEmpty(customerEmail))
                {
                    //create mail server
                    var mailServer = RegistryBM.Instance.GetMailServerInfo();

                    //get mail template
                    var tmpName = MailTemplates.TEMPLATE_MAIL_CANCEL_APPLICANT.ToString();
                    var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                    template.Subject = template.Subject.Replace(Constant.ApplicantNumber, appId.ToString());
                    template.TemplateContent = template.TemplateContent.Replace(Constant.ApplicantNumber, appId.ToString());
                    template.TemplateContent = template.TemplateContent.Replace(Constant.HomeLink, this.ServerURL);
                    
                    // get first user by store
                    var firstUser = app.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(app.StoreNumber.Value) : null;
                    template.TemplateContent = template.TemplateContent.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                    MailHelper.SendMailWithAttachments(mailServer, mailServer.Username,
                                                       customerEmail, ccEmail.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                                                       template.Subject, template.TemplateContent, new string[] { }, "", mailServer.Password);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }               

        private bool NotifyApprovedToCustomer(SalesOrder order)
        {
            try
            {
                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_APPROVED_APPLICANT.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);                

                //email & name
                string email, name;
                //if (order.SalesCustomer != null &&
                //    !string.IsNullOrEmpty(order.SalesCustomer.Email) &&
                //    !string.IsNullOrEmpty(order.SalesCustomer.Name))
                //{
                //    email = order.SalesCustomer.Email;
                //    name = order.SalesCustomer.Name;
                //}
                //else
                //{
                //    //get submmitter
                //    var user = UserBM.Instance.GetById(order.CreatedBy);
                //    email = user.Email;
                //    name = user.Name;
                //}

                //cc to store if need
                var storeEmails = UserBM.Instance.GetStoreEmailList(order.StoreNumber.Value, false);        

                //subject
                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());

                //build mail content
                var content = template.TemplateContent;
                content = content.Replace(Constant.UserName, "");
                content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());                               
                content = content.Replace(Constant.HomeLink, ServerURL);

                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                MailHelper.SendAsyncEmail(mailServer, "", mailServer.Username, storeEmails, null, template.Subject, content, false);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }

        #endregion
    }
}
