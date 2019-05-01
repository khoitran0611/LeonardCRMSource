using System;
using System.Linq;
using System.IO;
using System.Web;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System.Web.Http;
using Eli.Common;
using System.Collections.Generic;
using LeonardCRM.BusinessLayer.Helper;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SalesCompleteApiController : BaseApiController
    {
        #region Action Methods

        //Save the customer signature
        [HttpPost]
        public ResultObj SaveCustomerSignature([FromBody]SalesOrderComplete saleComplete, [FromUri] int appId)
        {
            try
            {
                var recheckMsg = PreCheckOrder(saleComplete.SalesOrder);
                if(!string.IsNullOrEmpty(recheckMsg))
                {
                    return new ResultObj(ResultCodes.ValidationError, recheckMsg);
                }

                var isDeliverySign = !string.IsNullOrEmpty(saleComplete.DeliverySignatureUrl) && string.IsNullOrEmpty(saleComplete.DeliverSignature);

                SetSignature(saleComplete, appId);

                if (!isDeliverySign)
                {
                    //store the signature image
                    if (!string.IsNullOrEmpty(saleComplete.CustomerSignatureUrl))
                    {
                        ImageHelper.SaveImageFromBase64(saleComplete.CustomerSignatureUrl.ToString(), HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.Signature));
                    }
                }
                else
                {
                    ImageHelper.SaveImageFromBase64(saleComplete.DeliverySignatureUrl.ToString(), HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" + saleComplete.DeliverSignature));
                }

                return new ResultObj(ResultCodes.Success, saleComplete, 0);

            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        [HttpPost]
        public ResultObj SaveSaleComplete([FromBody]SalesOrderComplete saleComplete, [FromUri] int appId)
        {
            try
            {
                SetObject(saleComplete, appId);
                var validateMsg = ValidateFillForm(saleComplete);
                if (string.IsNullOrEmpty(validateMsg))
                {
                    //remove delivery PDF file
                    var deliveryFile = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP + "/" + string.Format(Constant.DeliveryFormFileNameFormat, appId));
                    if (File.Exists(deliveryFile)) File.Delete(deliveryFile);

                    int status = SalesOrderCompleteBM.Instance.UpdateSaleComplete(saleComplete);
                    if (status > 0)
                    {
                        var statusId = saleComplete.SalesOrder.Status;
                        var statusObj = ListValueBM.Instance.First(s => s.Id == statusId);

                        if (statusObj != null)
                        {
                            var isSendSuccess = saleComplete.SalesOrder.Status == OrderStatus.Completed.GetHashCode() ? NotifyCompletedAcceptanceToCustomer(appId, saleComplete.SalesOrder, statusObj.Description) :
                                                                                                                        NotifyDeliveredNotSignEmail(appId, saleComplete.SalesOrder);
                                                                                                                         

                            return new ResultObj(ResultCodes.Success, new
                            {
                                Message = isSendSuccess ? GetText("COMMON", "SAVE_SUCCESS_MESSAGE") : GetText("APPLICANT_FORM", "SAVE_SUCCESS_SEND_MAIL_FAIL_MESSAGE"),
                                AppStatus = statusId,
                                StatusName =
                                    string.Format(
                                        "<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                                        statusObj.Color, statusObj.Description),
                                StatusDescription = statusObj.AdditionalInfo
                            }, saleComplete.Id);
                        }
                    }

                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);
                }
                return new ResultObj(ResultCodes.ValidationError, validateMsg, saleComplete.Id);
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

        //set the signature data
        private void SetSignature(SalesOrderComplete saleComplete, int appId)
        {
            if (!string.IsNullOrEmpty(saleComplete.CustomerSignatureUrl) && string.IsNullOrEmpty(saleComplete.Signature))
            {
                saleComplete.SignIP = Utilities.GetClientIpAddress();
                saleComplete.SignDate = DateTime.Now;
                saleComplete.Signature = "CusAccept_" + appId + ".png";
            }

            if (!string.IsNullOrEmpty(saleComplete.DeliverySignatureUrl) && string.IsNullOrEmpty(saleComplete.DeliverSignature))
            {
                saleComplete.DeliverSignIP = Utilities.GetClientIpAddress();
                saleComplete.DeliverSignDate = DateTime.Now;
                saleComplete.DeliverSignature = "DelAccept_" + appId + ".png";
            }
        }

        private void SetObject(SalesOrderComplete saleComplete, int appId)
        {
            saleComplete.OrderId = appId;
            if (saleComplete.SalesOrder != null &&
                (!string.IsNullOrEmpty(saleComplete.CustomerSignatureUrl) || !string.IsNullOrEmpty(saleComplete.DeliverySignatureUrl)))
            {
                saleComplete.SalesOrder.Status = !string.IsNullOrEmpty(saleComplete.CustomerSignatureUrl) ? OrderStatus.Completed.GetHashCode() :
                                                                                                            OrderStatus.DeliveredNotSigned.GetHashCode();
                SetAuditFields(saleComplete.SalesOrder, saleComplete.SalesOrder.Id);
            }
            SetAuditFields(saleComplete, saleComplete.Id);            
        }

        private string ValidateFillForm(SalesOrderComplete entity)
        {
            //validate customer
            string msg = new ObjectValidator(Constant.ModuleSaleComplete).ValidateObject(entity);

            if (entity.NeedRepair == true && string.IsNullOrEmpty(entity.RepairNote))
            {
                msg += GetText("NOT_INPUT_REPAIR_NOTE_ERROR_MSG") + " <br/>";
            }

            if (entity.Satisfy == false && string.IsNullOrEmpty(entity.NoSatisfyComment))
            {
                msg += GetText("NOT_NO_SATISFY_ERROR_MSG") + " <br/>";
            }

            if (entity.Answer5 == true && entity.ActBlocksUsed == null && entity.ActHoursOfLabor == null && entity.ActMilesToSite == null)
            {
                msg += GetText("NOT_ACTUAL_VALUE_ERROR_MSG") + " <br/>";
            }

            if (entity.SalesOrder != null)
            {
                if (string.IsNullOrEmpty(entity.SalesOrder.SerialNumber))
                {
                    msg += GetText("CONFIRM_INPUT_SERIAL_NUMBER_MSG") + " <br/>";
                }
                else if (!string.IsNullOrEmpty(entity.DeliverSignature) && entity.SalesOrder.SerialNumber.Replace(" ", "").Trim().Length < 6)
                {
                    msg += GetText("SERIAL_NUMBER_AT_LEAST_MSG") + " <br/>";
                }               
            }

            return msg;
        }

        private bool NotifyCompletedAcceptanceToCustomer(int appId, SalesOrder order, string status)
        {
            var isSuccess = false;
            try
            {
                //customer email & name
                string customerEmail, customerName;
                var customer = SalesCustomerBM.Instance.GetById(order.CustomerId);

                if (!string.IsNullOrEmpty(customer.Email) &&
                    !string.IsNullOrEmpty(customer.Name))
                {
                    customerEmail = customer.Email;
                    customerName = customer.Name;
                }
                else
                {
                    //get submmitter
                    var user = UserBM.Instance.GetById(order.CreatedBy);
                    customerEmail = user.Email;
                    customerName = user.Name;
                }

                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //get the manager email
                //var managerEmails = order.IsSold == true ? "" : UserBM.Instance.GetManagerEmail(!string.IsNullOrEmpty(order.ResponsibleUsers) ?
                //                                                                           order.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray() : null);
                var managerEmails = UserBM.Instance.GetManagerEmail(!string.IsNullOrEmpty(order.ResponsibleUsers) ?
                                                                                           order.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray() : null);

                //get the driver email
                var driverEmails = UserBM.Instance.GetDeliveryEmail(!string.IsNullOrEmpty(order.ResponsibleUsers) ?
                                                                                           order.ResponsibleUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray() : null);

                var ccEmails = new List<string>();
                ccEmails.Add(customerEmail);
                if (!string.IsNullOrEmpty(driverEmails) && driverEmails.Trim() != "")
                {
                    ccEmails.AddRange(driverEmails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_COMPLETED_ACCEPTANCE.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, appId.ToString());

                //build mail content
                var content = template.TemplateContent.Replace(Constant.UserName, customer.Name);
                content = content.Replace(Constant.ApplicantNumber, appId.ToString());
                content = content.Replace(Constant.ApplicantStatus, status);
                content = content.Replace(Constant.HomeLink, this.ServerURL);
                
                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                var folder = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP);
                var fileName = string.Format(Constant.AcceptFormFileNameFormat, appId);
                var pdfFile = folder + "\\" + fileName;
                HtmlPdfConverter.ConvertWebpageToPdf(pdfFile, this.ServerURL + ConfigValues.CUSTOMER_ACCEPTANCE_WEBFORM_URL + HttpUtility.UrlEncode(SecurityHelper.Encrypt(appId.ToString())));

                //join with the accounting user email
                var toEmail = managerEmails.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(SiteSettings.ACCOUNTING_USER_EMAIL))
                {
                    toEmail = toEmail.Concat(SiteSettings.ACCOUNTING_USER_EMAIL.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                }

                //join with the repaired user email
                var complete = order.SalesOrderCompletes.SingleOrDefault();
                if (complete.NeedRepair.HasValue && complete.NeedRepair.Value && !string.IsNullOrEmpty(SiteSettings.REPAIR_USER_EMAIL))
                {
                    toEmail = toEmail.Concat(SiteSettings.REPAIR_USER_EMAIL.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                }

                MailHelper.SendMailWithAttachments(mailServer, mailServer.Username, string.Join(",", toEmail), ccEmails.ToArray(), template.Subject, content, new string[] { fileName }, folder, mailServer.Password);

                isSuccess = true;

            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        private bool NotifyDeliveredNotSignEmail(int appId, SalesOrder salesOrder)
        {
            var isSuccess = false;
            try
            {
                var relatedCustomer = salesOrder.SalesCustomer;
                if (relatedCustomer == null)
                {
                    relatedCustomer = SalesCustomerBM.Instance.GetById(salesOrder.CustomerId);
                }
                var isHasCustomerEmail = !(relatedCustomer.Email.Trim().ToLower() == SiteSettings.NO_USER_EMAIL.Trim().ToLower());
                var sentToEmails = UserBM.Instance.GetStoreEmailList(salesOrder.StoreNumber.Value);
                sentToEmails += isHasCustomerEmail ?  "," + relatedCustomer.Email : "";

                var tmplName = isHasCustomerEmail ? MailTemplates.TEMPLATE_MAIL_DIRVER_COMPLETE_WITH_CUSTOMER_EMAIL.ToString() :
                                                    MailTemplates.TEMPLATE_MAIL_DIRVER_COMPLETE_NO_CUSTOMER_EMAIL.ToString();
                var tmplMail = MailTemplateBM.Instance.Single(x => x.TemplateName == tmplName);

                tmplMail.Subject = tmplMail.Subject.Replace(Constant.ApplicantNumber, appId.ToString());

                var content = tmplMail.TemplateContent;
                content = content.Replace(Constant.HomeLink, this.ServerURL);
                
                // get first user by store
                var firstUser = salesOrder.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(salesOrder.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                //create mail server
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                //send mail
                MailHelper.SendMail(mailServer, mailServer.Username, string.Join(",", sentToEmails), tmplMail.Subject, content);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                isSuccess = false;
            }
            return isSuccess;
        }

        private string PreCheckOrder(SalesOrder order)
        {
            var resultMsg = "";
            if (order != null)
            {
                bool isExistOrder;

                var isExpectedStatus = SalesOrderBM.Instance.CheckExpectedStatus(order.Status, order.Id, out isExistOrder);

                if (!isExistOrder)
                {
                    resultMsg = GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG");
                }
                else if (isExistOrder && !isExpectedStatus)
                {
                    resultMsg = GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG");
                }
            }
            else
            {
                resultMsg = GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG");
            }

            return resultMsg;
        }
        #endregion
    }
}
