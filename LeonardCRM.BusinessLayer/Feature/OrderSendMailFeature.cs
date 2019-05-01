using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using System;
using System.IO;
using System.Web;
using LeonardCRM.BusinessLayer.Helper;

namespace LeonardCRM.BusinessLayer.Feature
{
    public class OrderSendMailFeature
    {
        //send the contract copy to customer
        public static bool SendContractCopyToCustomer(SalesOrder app, string serverUrl, Eli_User currentUser)
        {
            var result = false;
            try
            {

                //create the pdf file           
                string tempPath = HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_TEMP);
                string contractFullPath = OrderPdfFeature.CreateContractFile(app.Id);
                string contractFileName = Path.GetFileName(contractFullPath);
                string deliveryFullPath = OrderPdfFeature.CreateDeliveryFormFile(app.Id, serverUrl);
                string deliveryFileName = Path.GetFileName(deliveryFullPath);

                //get mail template
                var tmpName = MailTemplates.TEMPLATE_MAIL_SEND_CONTRACT_COPY.ToString();
                var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

                if (template != null && !string.IsNullOrEmpty(currentUser.Email))
                {
                    var customerEmail = app.SalesCustomer != null && !string.IsNullOrEmpty(app.SalesCustomer.Email) ? app.SalesCustomer.Email : SalesCustomerBM.Instance.GetCustomerEmail(app.Id);

                    //build the mail subject
                    template.Subject = template.Subject.Replace(Constant.ApplicantNumber, app.Id.ToString());

                    var emailList = app.StoreNumber.HasValue ? UserBM.Instance.GetStoreEmailList(app.StoreNumber.Value, false) : null;


                    //build the mail content
                    template.TemplateContent = template.TemplateContent.Replace(Constant.ApplicantNumber, app.Id.ToString());
                    template.TemplateContent = template.TemplateContent.Replace(Constant.UserName, currentUser.Name);
                    template.TemplateContent = template.TemplateContent.Replace(Constant.HomeLink, serverUrl);
                    
                    // get first user by store
                    var firstUser = app.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(app.StoreNumber.Value) : null;
                    template.TemplateContent = template.TemplateContent.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                    //create mail server
                    var mailServer = RegistryBM.Instance.GetMailServerInfo();

                    //send the mail with contract
                    MailHelper.SendMailWithAttachments(mailServer, mailServer.Username, customerEmail,
                                                                   !string.IsNullOrEmpty(emailList) ? emailList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null,
                                                                   template.Subject, template.TemplateContent,
                                                                   new string[] { contractFileName, deliveryFileName }, tempPath, mailServer.Password);
                    result = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }
    }
}
