using System;
using System.Collections.ObjectModel;
using System.Net;
using Eli.Common;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using System.Web.Security;
using LeonardCRM.BusinessLayer.Helper;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class SalesCustomerApiController : BaseApiController
    {
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("SALES_CUSTOMER", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [HttpPost]
        public ResultObj SaveCustomer([FromBody]JObject jsonObject, [FromUri] string t = null)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesCustomer>(jsonObject.ToString(), new JsonSerializerSettings
                {
                    Error = HandleDeserializationError //ignore invalid data of properties so that we can serialize without any errors
                });

                //New applicant submit check
                if (model.Id == 0 && (model.SalesOrders == null || model.SalesOrders.Count == 0))
                    return new ResultObj(ResultCodes.ValidationError, "Invalid submitted data. Please refresh browser and try again!");
                               
                if (model.Id > 0)
                {
                    var orderModel = model.SalesOrders.First();
                    bool isExistOrder;
                    var isExpectedStatus = SalesOrderBM.Instance.CheckExpectedStatus(orderModel.Status, orderModel.Id, out isExistOrder);

                    if (!isExistOrder)
                    {
                        return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "ORDER_NOT_EXIST_ERROR_MSG"));
                    }
                    else if (isExistOrder && !isExpectedStatus)
                    {
                        return new ResultObj(ResultCodes.ValidationError, GetText("ORDERS", "CHANGED_STATUS_ERROR_MSG"));
                    }
                }

                var entity = SetObject(model);
                
                if (entity == null)
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), 0);

                var msg = ValidateFillForm(entity, t);
                if (string.IsNullOrEmpty(msg))
                {
                    var isAssistantMode = !string.IsNullOrEmpty(t);                    

                    bool isNew = entity.Id == 0;
                    var status = SalesCustomerBM.Instance.SaveSalesCustomer(entity, HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SALE_DOCUMENT));
                    if (status > 0)
                    {
                        return ProcessSuccess(entity, isNew, isAssistantMode);
                    }
                    return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"), entity.Id);
                }
                return new ResultObj(ResultCodes.ValidationError, msg, entity.Id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }
        }

        private ResultObj ProcessSuccess(SalesCustomer entity, bool isNew, bool isAssistantMode)
        {
            //Get the new status
            var statusId = entity.SalesOrders.First().Status;
            var statusObj = ListValueBM.Instance.First(s => s.Id == statusId);

            NotifyToStore(entity, isNew, statusObj);

            if (isNew && //submit app
                isAssistantMode && //create on assistant mode             
                !string.IsNullOrEmpty(entity.Email) &&
                entity.Email.Trim().ToLower() != SiteSettings.NO_USER_EMAIL.Trim().ToLower())
            {
                var user = UserBM.Instance.GetUserByEmail(entity.Email);
                if (user == null) //is new user
                {
                    CreateNewUserFromCustomer(entity);
                }
                else
                {
                    SalesOrderBM.Instance.AssignAppToNewUser(user, entity.SalesOrders.First());
                }
            }
            

            if (isNew) //add customer
            {
                //with the "Sold" process: should store the customer signature in delivery form
                var relatedOrder = entity.SalesOrders.FirstOrDefault();
                if(relatedOrder != null && 
                   relatedOrder.IsSold == true && 
                   relatedOrder.SalesOrderDeliveries != null && relatedOrder.SalesOrderDeliveries.Any())
                {
                    var relatedDelivery = relatedOrder.SalesOrderDeliveries.FirstOrDefault();
                    if(relatedDelivery != null)
                    {
                        if (!string.IsNullOrEmpty(relatedDelivery.CustomerSignImage))
                        {
                            //save the signature image
                            ImageHelper.SaveImageFromBase64(relatedDelivery.CustomerSignImage,
                                HttpContext.Current.Server.MapPath(ConfigValues.UPLOAD_DIRECTORY_SIGNATURE + "/" +
                                                                   relatedDelivery.CustomerSignature));
                        }
                    }
                }

                //response to client
                return new ResultObj(ResultCodes.Success, GetText("APPLICANT_FORM", "SUBMIT_SUCCESS_APPLICATION_MESSAGE"),
                    entity.Id);
            }                
            
            //update customer
            return new ResultObj(ResultCodes.Success, new
            {
                Message = GetText("COMMON", "SAVE_SUCCESS_MESSAGE"),
                AppStatus = entity.SalesOrders.First().Status,
                StatusName =
                    string.Format("<span class=\"badge application-status\" style=\"background-color:{0};\">{1}</span>",
                        statusObj.Color, statusObj.Description),
                StatusDescription = statusObj.AdditionalInfo,
            }, entity.Id);
        }

        private void CreateNewUserFromCustomer(SalesCustomer entity)
        {
            var pwd = Membership.GeneratePassword(12, 1);
            var user = new Eli_User()
            {
                Name = entity.Name,
                Password = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "md5"),
                Email = entity.Email,
                DateOfBirth = entity.DateOfBirth,
                RoleId = UserRoles.Customer.GetHashCode(),
                Status = UserStatus.Active.GetHashCode(),
                Phone = !string.IsNullOrEmpty(entity.CellPhone) ? entity.CellPhone : entity.HomePhone               
            };

            SetAuditFields(user, user.Id);
            SalesOrderBM.Instance.AssignAppToNewUser(user, entity.SalesOrders.First());

            NotifyWellcomeMailToCustomer(user, pwd);
        }

        private void NotifyWellcomeMailToCustomer(Eli_User user, string pwd)
        {
            //create mail server
            var mailServer = RegistryBM.Instance.GetMailServerInfo();

            //get mail template
            var tmpName = MailTemplates.TEMPLATE_MAIL_WELCOME_NEW_USER.ToString();
            var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

            var content = template.TemplateContent.Replace(Constant.EmailAddress, user.Email);
            content = content.Replace(Constant.Password, pwd);
            content = content.Replace(Constant.HomeLink, this.ServerURL);
            content = content.Replace(Constant.UserName, user.Name);

            MailHelper.SendMail(mailServer, mailServer.Username, user.Email, template.Subject, content);
        }

        private void SetOrderStatus(SalesCustomer entity, SalesOrder order)
        {
            var orderStatus = GetOrderStatus(entity);           
            order.Status = (int)orderStatus;
        }

        private void SetAuditForChildren(SalesCustomer entity)
        {
            foreach (var field in entity.FieldData)
            {
                SetAuditFields(field, field.Id);
            }

            entity.Notes = entity.Notes ?? new List<Eli_Notes>();
            foreach (var note in entity.Notes)
            {
                SetAuditFields(note, note.Id);
            }

            entity.SalesCustReferences = entity.SalesCustReferences.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToList();
            foreach (var salesRef in entity.SalesCustReferences)
            {
                SetAuditFields(salesRef, salesRef.Id);
            }
        }

        [HttpGet]
        public SalesCustomer GetApplicantById(int appId)
        {
            try
            {
                if (appId <= 0)
                    return new SalesCustomer() { Email = this.CurrentUser.Email, Name = this.CurrentUser.Name, SelfEmployed = true, ClientIP =  Utilities.GetClientIpAddress()};

                var result = SalesCustomerBM.Instance.GetApplicantById(appId);
                var currentUserId = CurrentUserID;
                if (result != null && (CurrentUserRole.IsHostAdmin || 
                                       result.CreatedBy == currentUserId || 
                                       ("," + result.SalesOrders.First().ResponsibleUsers).Contains("," + currentUserId + ",") || 
                                       CurrentUserRole.Id == (int)UserRoles.ContractManager)) //Do not allow customer to view the other's applications
                {
                    result.IsCustomer = CurrentUserRole.Id == UserRoles.Customer.GetHashCode();
                    result.IsDeliveryPerson = CurrentUserRole.Id == UserRoles.DeliveryStaff.GetHashCode() && result.CreatedBy != currentUserId;
                    result.ClientIP = Utilities.GetClientIpAddress();

                    if (result.IsDeliveryPerson)
                    {
                        result.SalesCustReferences.Clear();
                        result.SalesOrders.First().SalesDocuments.Clear();
                        result.CoName = result.Email = result.CoEmail = result.DriverLicense = result.CoDriverLicense = string.Empty;
                        result.MailingState = result.CoSocialNum = result.SocialNum = null;
                        result.MailingCity = result.MailingStreet = result.MailingZip = string.Empty;
                        result.Employer = result.CoEmployer = result.Phone = result.Supervisor = result.CoPhone = result.CoSupervisor = string.Empty;
                        result.CheckingAccount = result.SavingAccount = result.EnrollAutoPay = null;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        //Calculate points for auto approval and set order status accordingly
        public OrderStatus GetOrderStatus(SalesCustomer customer)
        {
            var status = customer.SalesOrders.First().Status;
            if (status != (int)OrderStatus.PreApproved && status != (int)OrderStatus.Pending && status > 0)
            {
                return (OrderStatus)Enum.Parse(typeof(OrderStatus), status.ToString());
            }
            // Get Customer Entity Field
            var customerModule = ModuleBM.Instance.Single(m => m.Name.ToLower() == "customer");
            var customerEntityFields = EntityFieldBM.Instance.GetViewColumnsByModule(customerModule.Id, true,
                CurrentUserRole.Id);

            int totalEntityPoints = 0;

            // Point From Customer
            foreach (var cusField in customerEntityFields)
            {
                var property = customer.GetType()
                    .GetProperty(cusField.ColumnName, BindingFlags.Public | BindingFlags.Instance);
                var propertyValue = property.GetValue(customer);
                var compareValue = propertyValue == null ? "" : propertyValue.ToString();

                if (!string.IsNullOrEmpty(compareValue))
                {
                    switch ((cusField.ColumnName))
                    {
                        case "AtAddressSince":
                            if (DateTime.Now.Year - Convert.ToDateTime(compareValue).Year >= 1)
                                totalEntityPoints += Utilities.ToInt(cusField.Point);
                            break;

                        case "ResidenceType":
                            var resId = int.Parse(compareValue);
                            var residenceTypeValue =
                                ListValueBM.Instance.Find(x => x.Id == resId).FirstOrDefault();
                            if (residenceTypeValue != null)
                            {
                                if (residenceTypeValue.Id == (int)ResidenceType.Own)
                                    totalEntityPoints += (int)ResidenceTypePoint.Own;
                                else
                                    totalEntityPoints += (int)ResidenceTypePoint.Rent;
                            }
                            break;

                        case "LandType":
                            var landId = int.Parse(compareValue);
                            var landTypeValue =
                                ListValueBM.Instance.Find(x => x.Id == landId).FirstOrDefault();
                            if (landTypeValue != null)
                            {
                                if (landTypeValue.Id == (int)LandType.Own)
                                    totalEntityPoints += (int)LandTypePoint.Own;
                                else
                                    totalEntityPoints += (int)LandTypePoint.Rent;
                            }
                            break;

                        case "Since":
                        case "CoSince":
                            if (TimeManager.GetTotalMonths(DateTime.Now) - TimeManager.GetTotalMonths(Convert.ToDateTime(compareValue)) >= 6)
                                totalEntityPoints += Utilities.ToInt(cusField.Point);
                            break;

                        case "BusinessSince":
                            if (DateTime.Now.Year - Convert.ToDateTime(compareValue).Year >= 2)
                                totalEntityPoints += Utilities.ToInt(cusField.Point);
                            break;

                        case "CheckingAccount":
                        case "SavingAccount":
                        case "EnrollAutoPay":
                            if (Convert.ToBoolean(compareValue))
                                totalEntityPoints += Utilities.ToInt(cusField.Point);
                            break;

                        default:
                            totalEntityPoints += Utilities.ToInt(cusField.Point);
                            break;
                    }
                }
            }

            // Point From References
            if (customer.SalesCustReferences.Count == 3)
                totalEntityPoints += 2;

            if (customer.SalesOrders != null && customer.SalesOrders.Any())
            {
                customer.SalesOrders.First().TotalPoint = totalEntityPoints;
            }

            if (totalEntityPoints >= SiteSettings.TOTAL_POINTS)
            {
                return OrderStatus.PreApproved;
            }

            return OrderStatus.Pending;
        }

        [HttpPost]
        public SalesCustomer GetCustomerById([FromBody] PageInfo pagedInfo)
        {
            try
            {
                SalesCustomer entity;
                if (pagedInfo.Id > 0)
                {
                    entity = SalesCustomerBM.Instance.SingleLoadWithReferences(record => record.Id == pagedInfo.Id,
                        "SalesOrders", "SalesCustReferences");
                    entity.RelateViews = ViewBM.Instance.GetRelateViews(pagedInfo.ModuleId, pagedInfo.ViewId);
                }
                else
                {
                    entity = new SalesCustomer { RelateViews = new List<RelateView>(), SalesOrders = new Collection<SalesOrder>(), SalesCustReferences = new Collection<SalesCustReference>() };
                }

                if (string.IsNullOrEmpty(entity.ResponsibleUsers) && CurrentUserRole.Id != (int)UserRoles.Customer)
                {
                    entity.UserIds = new[] { CurrentUserID };
                    entity.ResponsibleUsers = CurrentUserID + ",";
                }
                else
                {
                    entity.ResponsibleUsers = entity.ResponsibleUsers.Substring(0, entity.ResponsibleUsers.Length - 1);
                    int n;
                    entity.UserIds = entity.ResponsibleUsers.Split(',').Select(s => int.TryParse(s, out n) ? n : 0).ToArray();
                }
                entity.CustomFields = EntityFieldBM.Instance.GetCustomFieldByModuleId(pagedInfo.ModuleId, entity.Id, CurrentUserRole.Id);

                var notes = NoteBM.Instance.GetNoteByRecordId(Constant.ModuleCustomer, entity.Id, null);
                entity.Notes = notes;

                return entity;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new SalesCustomer();
            }
        }

        [HttpPost]
        public ResultObj DeleteCustomers([FromBody] JArray jsonObject)
        {
            try
            {
                var entities = JsonConvert.DeserializeObject<IList<SalesCustomer>>(jsonObject.ToString());
                int status = SalesCustomerBM.Instance.Delete(entities);
                if (status > 0)
                    return new ResultObj(ResultCodes.Success, GetText("COMMON", "DELETE_SUCCESS"), 0);
                return new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "DELETE_ERROR"), 0);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new ResultObj(ResultCodes.UnkownError, GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"), 0);
            }

        }

        private string ValidateFillForm(SalesCustomer entity, string mode)
        {
            //validate customer
            var isMarkSold = entity.SalesOrders != null && entity.SalesOrders.Any() ? entity.SalesOrders.First().IsSold : false;

            string msg = isMarkSold != true ? new ObjectValidator(Constant.ModuleCustomer).ValidateObject(entity) : "";

            //validate order
            if (entity.SalesOrders != null && entity.SalesOrders.Any())
            {
                msg += new ObjectValidator(Constant.ModuleOrder).ValidateObject(entity.SalesOrders.First());
            }

            if (!string.IsNullOrEmpty(mode))
            {
                var parts = SecurityHelper.Decrypt(mode).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Count() < 2 || (int.Parse(parts[0]) != CurrentUserID) || (int.Parse(parts[1]) != CurrentUserRole.Id))
                {
                    msg += LocalizeHelper.Instance.GetText("COMMON", "REQUEST_INVALID_ERROR_MSG");
                }
            }
            return msg;
        }

        private SalesCustomer SetObject(SalesCustomer model)
        {
            SalesCustomer entity;

            if (model.Id > 0)
            {
                entity = SalesCustomerBM.Instance.GetById(model.Id);
                if (entity != null)
                {
                    entity.Name = model.Name;
                    entity.Email = model.Email;
                    entity.CellPhone = model.CellPhone;
                    entity.HomePhone = model.HomePhone;
                    entity.ModuleId = model.ModuleId;
                    entity.FieldData = model.FieldData;
                    entity.CustomFields = model.CustomFields;
                    entity.ResponsibleUsers = model.ResponsibleUsers;
                    entity.UserIds = model.UserIds;
                    entity.AtAddressSince = model.AtAddressSince;
                    entity.BusinessSince = model.BusinessSince;
                    entity.CellPhone = model.CellPhone;
                    entity.CheckingAccount = model.CheckingAccount;
                    entity.CoCellPhone = model.CoCellPhone;
                    entity.CoDateOfBirth = model.CoDateOfBirth;
                    entity.CoDriverLicense = model.CoDriverLicense;
                    entity.CoEmail = model.CoEmail;
                    entity.CoEmployer = model.CoEmployer;
                    entity.CoName = model.CoName;
                    entity.CoPhone = model.CoPhone;
                    entity.CoSince = model.CoSince;
                    entity.CoSocialNum = model.CoSocialNum;
                    entity.CoSupervisor = model.CoSupervisor;
                    entity.DateOfBirth = model.DateOfBirth;
                    entity.DriverLicense = model.DriverLicense;
                    entity.Employer = model.Employer;
                    entity.EnrollAutoPay = model.EnrollAutoPay;
                    entity.LandlordName = model.LandlordName;
                    entity.LandlordPhone = model.LandlordPhone;
                    entity.LandType = model.LandType;
                    entity.MailingCity = model.MailingCity;
                    entity.MailingState = model.MailingState;
                    entity.MailingStreet = model.MailingStreet;
                    entity.MailingZip = model.MailingZip;
                    entity.IncomeExplanation = model.IncomeExplanation;
                    entity.Phone = model.Phone;
                    entity.PhysicalCity = model.PhysicalCity;
                    entity.PhysicalState = model.PhysicalState;
                    entity.PhysicalStreet = model.PhysicalStreet;
                    entity.PhysicalZip = model.PhysicalZip;
                    entity.ResidenceType = model.ResidenceType;
                    entity.SavingAccount = model.SavingAccount;
                    entity.Since = model.Since;
                    entity.SocialNum = model.SocialNum;
                    entity.SelfEmployed = model.SelfEmployed;
                    entity.Supervisor = model.Supervisor;
                    entity.TypeOfBusiness = model.TypeOfBusiness;
                    entity.SalesCustReferences = model.SalesCustReferences.ToList();
                    entity.SalesOrders = model.SalesOrders;

                    //case: update
                    //we must guarantee the creator in the responsible users list
                    if (entity.UserIds != null && !entity.UserIds.Contains(entity.CreatedBy.Value) && CurrentUserRole.Id != (int)UserRoles.Customer)
                    {
                        entity.UserIds = entity.UserIds.Union(new[] { entity.CreatedBy.Value }).ToArray();
                    }
                }
            }
            else
            {
                entity = model;
                if (model.IsSameMailingAddress == true )
                {
                    entity.PhysicalCity = model.MailingCity;
                    entity.PhysicalState = model.MailingState;
                    entity.PhysicalStreet = model.MailingStreet;
                    entity.PhysicalZip = model.MailingZip;
                }
                entity.IsActive = true;
                var relatedOrder = entity.SalesOrders.First();
                if(relatedOrder != null)
                {
                    entity.UserIds = UserBM.Instance.GetResponsibleListForStore(relatedOrder.StoreNumber, relatedOrder.IsSold.GetValueOrDefault(false));
                }                
            }

            if (entity != null)
            {
                if (entity.UserIds != null)
                {
                    entity.ResponsibleUsers = string.Join(",", entity.UserIds) + ",";
                    entity.SalesOrders.First().ResponsibleUsers = entity.ResponsibleUsers;
                }
                SetAuditFields(entity, entity.Id);
                entity.Notes = model.Notes;

                SetRelatedChildren(entity);
            }

            return entity;
        }

        [HttpGet]
        public IList<vwClient> GetAllClients()
        {
            try
            {
                return SalesCustomerBM.Instance.GetAllClients(CurrentUserRole.Id, CurrentUserID);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpGet]
        public IList<vwAllCustomer> GetAllCustomers()
        {
            try
            {
                return SalesCustomerBM.Instance.GetAllCustomers(CurrentUserRole.Id, CurrentUserID);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return null;
            }
        }

        [HttpGet]
        public virtual ResultObj GetContractContent(int appId)
        {
            try
            {
                return new ResultObj(ResultCodes.Success, SalesCustomerBM.Instance.GetContractContent(appId, GetText("ORDERS", "PROMOTION_TEXT"), true, false, ""));
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception.Message, exception);
                return new ResultObj(ResultCodes.UnkownError, "");
            }
        }

        //Notify applicant to store by mail
        private void NotifyToStore(SalesCustomer customer, bool isNew, Eli_ListValues statusObj)
        {
            //create mail server
            var mailServer = RegistryBM.Instance.GetMailServerInfo();

            //get mail template
            var tmpName = isNew ? MailTemplates.TEMPLATE_MAIL_NOTIFY_SUBMIT_APPLICANT.ToString() : MailTemplates.TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT.ToString();
            var template = MailTemplateBM.Instance.Single(x => x.TemplateName == tmpName);

            //build mail content
            var content = template.TemplateContent.Replace(Constant.UserName, customer.Name);
            var order = customer.SalesOrders.FirstOrDefault();
            
            if (order != null)
            {
                var emailList = order.StoreNumber.HasValue ? UserBM.Instance.GetStoreEmailList(order.StoreNumber.Value) : "";
                content = content.Replace(Constant.ApplicantNumber, order.Id.ToString());
                template.Subject = template.Subject.Replace(Constant.ApplicantNumber, order.Id.ToString());
                content = content.Replace(Constant.ApplicantStatus, statusObj.Description);
                content = content.Replace(Constant.NoteForRent, (customer.ResidenceType == ResidenceType.Rent.GetHashCode() || customer.LandType == LandType.Rent.GetHashCode()) ? GetText("APPLICANT_FORM", "NOTE_FOR_RENT_MESSAGE") : "");
                content = content.Replace(Constant.HomeLink, this.ServerURL);

                // get first user by store
                var firstUser = order.StoreNumber.HasValue ? UserBM.Instance.GetFirstUserByStore(order.StoreNumber.Value) : null;
                content = content.Replace(Constant.EmailSignature, firstUser != null ? firstUser.Signature.ConvertSignature() : "");

                //send mail to all submitted store users so that they all receive notification
                if (order.IsSold == true) //with "Sold" process
                {
                    if(!string.IsNullOrEmpty(emailList))
                    {
                        MailHelper.SendAsyncEmail(mailServer, "", //mail server
                                         mailServer.Username, //sender
                                         emailList, // to emails
                                         null, //cc
                                         template.Subject, content, //subject & content email
                                         false);
                    }                   
                }
                else //with "Rent" process
                {
                    MailHelper.SendAsyncEmail(mailServer, "", //mail server
                                          mailServer.Username, //sender
                                          !string.IsNullOrEmpty(emailList) ? emailList : customer.Email, // to emails
                                          !string.IsNullOrEmpty(emailList) ? customer.Email : null, //cc
                                          template.Subject, content, //subject & content email
                                          false);
                }               
            }
        }

        private void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            errorArgs.ErrorContext.Handled = true;
        }

        private void SetRelatedChildren(SalesCustomer entity)
        {
            SetAuditForChildren(entity);

            var order = entity.SalesOrders.FirstOrDefault();
            if (order != null)
            {
                // Get Order Status
                if (!order.IsSold.GetValueOrDefault(false))
                {
                    SetOrderStatus(entity, order);
                }

                if (entity.Id == 0)//adding new customer
                {
                    //with the "Sold" process: init status is pending delivery
                    var pendingDeliveryStatus = OrderStatus.PendingDelivery.GetHashCode();
                    if (order.IsSold == true && 
                        order.Status < pendingDeliveryStatus)//app isn't form Pending Delivery status
                    {
                        order.Status = pendingDeliveryStatus;
                    }

                    order.IsActive = true;//active order
                    order.IsNewPart = true; //set default for new part

                    if (order.IsSold.GetValueOrDefault(false) &&
                       order.SalesOrderDeliveries != null &&
                       order.SalesOrderDeliveries.Any())//belong to sold process
                    {
                        var relatedDelivery = order.SalesOrderDeliveries.First();
                        if (relatedDelivery != null)
                        {
                            relatedDelivery.CustomerSignIP = Utilities.GetClientIpAddress();
                            relatedDelivery.CustomerSignDate = DateTime.Now;
                            relatedDelivery.CustomerSignature = Constant.DeliveryCustomerSignName;
                            relatedDelivery.IsActive = true;
                        }

                        SetAuditFields(relatedDelivery, relatedDelivery.Id);
                    }
                }
                else //updating customer
                {
                    if (!string.IsNullOrEmpty(order.SignatureIP) && string.IsNullOrEmpty(order.LesseeSignature))
                    {
                        order.SignatureIP = null;
                    }

                    if (order.SalesOrderCompletes != null && order.SalesOrderCompletes.Any())
                    {
                        order.SalesOrderCompletes.Clear();
                    }
                }

                SetAuditFields(order, order.Id);
            }
        }
    }
}
