using System;
using System.IO;
using System.Web;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Web.Http;
using Eli.Common;
using System.Linq;
using LeonardCRM.BusinessLayer.Feature;
using LeonardCRM.BusinessLayer.Helper;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SalesDeliveryApiController : BaseApiController
    {
        #region Action Methods

        public virtual ResultObj SaveDelivery([FromBody] SalesOrderDelivery delivery, [FromUri] string t = null)
        {
            try
            {
                var order = delivery.SalesOrder;
                bool isExistOrder;
                var oldStatus = order.Status;

                var isExpectedStatus = SalesOrderBM.Instance.CheckExpectedStatus(order.Status, delivery.OrderId, out isExistOrder);

                if (!isExistOrder)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG"));
                }
                else if (isExistOrder && !isExpectedStatus)
                {
                    return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG"));
                }

                SetObject(delivery);
                var msg = ValidateObject(delivery, t);
                if (string.IsNullOrEmpty(msg))
                {
                    var status = SalesOrderDeliveryBM.Instance.UpdateSaleDelivery(delivery);
                    if (status > 0)
                    {
                        //delete the cached file if have
                        var fileName = string.Format(Constant.DeliveryFormFileNameFormat, delivery.OrderId);
                        var path = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP + "/" + fileName);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        //return success response
                        return ProcessSuccess(delivery, oldStatus);
                    }
                    else
                    {
                        return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), delivery.Id); ;
                    }
                }

                return new ResultObj(ResultCodes.ValidationError, msg, delivery.Id); ;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        #endregion

        #region Internal Methods

        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("CUSTOMER_ACCEPTANCE_FORM", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        private string ValidateObject(SalesOrderDelivery delivery, string mode)
        {
            var msg = "";
            var validator = new ObjectValidator(Constant.ModuleOrderDelivery);
            msg += validator.ValidateObject(delivery);

            // Validate for Delivery Request
            if (delivery.SalesOrder.Status >= OrderStatus.PendingDelivery.GetHashCode())
            {
                if (delivery.DeliveryType == DeliveryType.MoveJob.GetHashCode())
                {
                    if (string.IsNullOrWhiteSpace(delivery.MoveFromAddress))
                        msg += GetText("APPLICANT_FORM", "MOVE_FROM_ADDRESS_REQUIRE_ERROR_MSG") + "<br>";
                    if (string.IsNullOrWhiteSpace(delivery.MoveToAddress))
                        msg += GetText("APPLICANT_FORM", "MOVE_TO_ADDRESS_REQUIRE_ERROR_MSG") + "<br>";
                }
            }

            if (delivery.SalesOrder != null && delivery.SalesOrder.SalesCustomer != null)
            {
                validator = new ObjectValidator(Constant.ModuleCustomer);

                if (delivery.SalesOrder.IsSold == true) //"Sold" process
                {
                    msg += validator.ValidateObjectWithFields(delivery.SalesOrder.SalesCustomer, new string[] {
                        "StoreNumber", "Name", "Email",
                        "MailingStreet", "MailingCity", "MailingZip", "MailingState",
                        "PhysicalStreet", "PhysicalCity", "PhysicalZip", "PhysicalState" });
                }
                else //"Rent" process
                {                    
                    msg += validator.ValidateObject(delivery.SalesOrder.SalesCustomer);
                }                
            }

            if (!string.IsNullOrEmpty(mode))
            {
                var parts = SecurityHelper.Decrypt(mode).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || (int.Parse(parts[0]) != CurrentUserID) || (int.Parse(parts[1]) != CurrentUserRole.Id))
                {
                    msg += LocalizeHelper.Instance.GetText("COMMON", "REQUEST_INVALID_ERROR_MSG");
                }
            }

            return msg;
        }

        private void SetObject(SalesOrderDelivery delivery)
        {
            SetAuditFields(delivery, delivery.Id);

            var orderId= delivery.OrderId;
            var order = SalesOrderBM.Instance.SingleLoadWithReferences(x => x.Id == orderId, "SalesCustomer", "SalesDocuments");

            if (order != null && order.SalesCustomer != null)
            {
                var customer = order.SalesCustomer;
                if (delivery.SalesOrder != null && delivery.SalesOrder.SalesCustomer != null)
                {
                    customer.PhysicalStreet = delivery.SalesOrder.SalesCustomer.PhysicalStreet;
                    customer.HomePhone = delivery.SalesOrder.SalesCustomer.HomePhone;
                    customer.CellPhone = delivery.SalesOrder.SalesCustomer.CellPhone;
                    SetAuditFields(customer, customer.Id);
                }

                delivery.SalesOrder = order;
                SetOrderStatus(delivery.SalesOrder.SalesCustomer, delivery.SalesOrder, delivery);
            }
            else
            {
                delivery.SalesOrder = null;
            }
        }

        private void SetOrderStatus(SalesCustomer entity, SalesOrder order, SalesOrderDelivery orderDelivery)
        {
            if (order.Status == OrderStatus.PendingCusAccept.GetHashCode())
            {
                var isRequireWaiver = entity.ResidenceType == ResidenceType.Rent.GetHashCode() || entity.LandType == LandType.Rent.GetHashCode();
                var docsRequired = 1;
                docsRequired += (isRequireWaiver ? 1 : 0) + (!string.IsNullOrWhiteSpace(entity.CoName) ? 1 : 0);

                //Check if Delivery Request Form is completed
                if (!string.IsNullOrEmpty(order.LesseeSignature) && //require customer's signature
                    (string.IsNullOrEmpty(entity.CoName) || !string.IsNullOrEmpty(order.CoSignature)) && //require the siganture if have the co-customer
                    order.SalesDocuments.Count >= docsRequired && 
                    orderDelivery != null && 
                    !string.IsNullOrWhiteSpace(orderDelivery.CustomerSignature))
                {
                    //up to the "InProgress"
                    order.Status = OrderStatus.InProgress.GetHashCode();
                    SetAuditFields(order, order.Id);
                }
            } 
        }

        private bool NotifyCompletedDeliveryRequestStore(SalesCustomer customer, SalesOrder order, Eli_ListValues statusObj)
        {
            try
            {
                var isSuccess = false;

                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_COMPLETED_DELIVERY_REQUEST.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                //build mail content
                var content = template.TemplateContent.Replace(Constant.UserName, customer.Name);
                var emailList = order.StoreNumber.HasValue ? UserBM.Instance.GetStoreEmailList(order.StoreNumber.Value) : "";
                var deliveryEmail = UserBM.Instance.GetDeliveryEmailList(order.Id);
                var senderEmailArray = (!string.IsNullOrEmpty(emailList) ? emailList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray() : 
                                                                           new string[] { }) //get store email 
                                        .Concat(!string.IsNullOrEmpty(deliveryEmail) ? deliveryEmail.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray() : 
                                                                                       new string[] { }); //join with delivery email
                emailList = string.Join(",", senderEmailArray);

                content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());
                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());
                content = content.Replace(Constant.ApplicantStatus, statusObj.Description);
                content = content.Replace(Constant.HomeLink, this.ServerURL);

                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                //send mail to all submitted store users so that they all receive notification
                if (!string.IsNullOrEmpty(emailList))
                {
                    var appId = order.Id;
                    var fullPath = OrderPdfFeature.CreateDeliveryFormFile(appId, this.ServerURL);
                    var folder = Path.GetDirectoryName(fullPath);
                    var fileName = Path.GetFileName(fullPath);

                    MailHelper.SendAsyncEmail(mailServer, "", mailServer.Username, emailList, template.Subject, content, new[] { fileName }, folder);
                    isSuccess = true;
                }

                return isSuccess;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }

        private ResultObj ProcessSuccess(SalesOrderDelivery delivery, int oldStatus)
        {
            var order = delivery.SalesOrder;
            var customer = delivery.SalesOrder.SalesCustomer;

            //Get the new status
            var statusId = delivery.SalesOrder.Status;
            var statusObj = ListValueBM.Instance.First(s => s.Id == statusId);

            if (order.Status == OrderStatus.PendingCusAccept.GetHashCode())
            {
                bool isRequireWaiver;
                int docsCount;
                SalesOrderBM.Instance.SetRequireWaiverAndDocs(customer, out isRequireWaiver, out docsCount);

                if (order.SalesDocuments.Count == 0 || 
                    string.IsNullOrWhiteSpace(delivery.CustomerSignature) || 
                    string.IsNullOrEmpty(order.LesseeSignature) ||  //miss customer's signature
                    (!string.IsNullOrEmpty(customer.CoName) && string.IsNullOrEmpty(order.CoSignature))) //miss co-customer's signature
                {
                    var message = GetText("APPLICANT_FORM", "DRIVER_LISCENSE_REQUIRE_MESSAGE");
                    message = string.Format(message, isRequireWaiver ? GetText("APPLICANT_FORM", "LANDLORD_WAIVER_FORM_ACCEPT") : "");

                    return new ResultObj(ResultCodes.Success,
                        new
                        {
                            Message = message,
                            AppStatus = statusId,
                            StatusName =
                                string.Format(
                                    "<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                                    statusObj.Color, statusObj.Description),
                            StatusDescription = statusObj.AdditionalInfo,
                        }, delivery.Id);
                }
                else if (order.SalesDocuments.Count < docsCount)
                {
                    if (!string.IsNullOrWhiteSpace(customer.CoName))
                    {
                        var message = GetText("APPLICANT_FORM", "DRIVER_LISCENSE_REQUIRE_FOR_CONAME");
                        message = string.Format(message, isRequireWaiver ? GetText("APPLICANT_FORM", "LANDLORD_WAIVER_FORM_ACCEPT") : "");

                        return new ResultObj(ResultCodes.Success,
                            new
                            {
                                Message = message,
                                AppStatus = statusId,
                                StatusName =
                                    string.Format(
                                        "<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                                        statusObj.Color, statusObj.Description),
                                StatusDescription = statusObj.AdditionalInfo
                            }, delivery.Id);
                    }
                    else
                    {
                        var message = GetText("APPLICANT_FORM", "LANDLORD_WAIVER_FORM_REQUIRE_ERROR_MSG");

                        return new ResultObj(ResultCodes.Success,
                            new
                            {
                                Message = message,
                                AppStatus = statusId,
                                StatusName =
                                    string.Format(
                                        "<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                                        statusObj.Color, statusObj.Description),
                                StatusDescription = statusObj.AdditionalInfo
                            }, delivery.Id);
                    }
                }
            }


            if (delivery.SalesOrder.Status == OrderStatus.InProgress.GetHashCode())
            {
                var isSendSuccess = NotifyChangesToManager(delivery.SalesOrder) &&
                                    (oldStatus == OrderStatus.PendingCusAccept.GetHashCode() ? //up from "PendingCusAccept" to "InProgress"
                                                    OrderSendMailFeature.SendContractCopyToCustomer(delivery.SalesOrder, ServerURL, CurrentUser) :
                                                    NotifyCompletedDeliveryRequestStore(delivery.SalesOrder.SalesCustomer, delivery.SalesOrder, statusObj));

                return new ResultObj(ResultCodes.Success,
                    new
                    {
                        Message = isSendSuccess ? GetText("DELIVERY_REQUEST_FORM", "APPLICATION_IS_IN_PROGRESS") : GetText("DELIVERY_REQUEST_FORM", "APPLICATION_IS_IN_PROGRESS_WITHOUT_SEND_MAIL"),
                        AppStatus = statusId,
                        StatusName =
                            string.Format(
                                "<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                                statusObj.Color, statusObj.Description),
                        StatusDescription = statusObj.AdditionalInfo,
                    }, delivery.Id);
            }

            return new ResultObj(ResultCodes.Success, new
            {
                Message = GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),
                AppStatus = statusId,
                StatusName =
                    string.Format("<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                        statusObj.Color, statusObj.Description),
                StatusDescription = statusObj.AdditionalInfo,
            }, delivery.Id);
        }

        private bool NotifyChangesToManager(SalesOrder order)
        {
            try
            {
                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT_TO_MANAGER.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                var managerEmail = UserBM.Instance.GetManagerEmail(!string.IsNullOrEmpty(order.ResponsibleUsers) ?
                                                                                         order.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray() : null).Trim();
                var status = ListValueBM.Instance.GetById(order.Status);

                //build mail content
                var content = template.TemplateContent;
                content = content.Replace(Constant.UserName, "");
                content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());
                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());
                content = content.Replace(Constant.ApplicantStatus, status.Description);
                content = content.Replace(Constant.HomeLink, ServerURL);
                
                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                MailHelper.SendAsyncEmail(mailServer, "", mailServer.Username, managerEmail, null, template.Subject, content, false);

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
