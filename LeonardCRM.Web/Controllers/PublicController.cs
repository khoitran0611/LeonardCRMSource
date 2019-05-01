using System.Web.Mvc;
using Eli.Common;

namespace LeonardCRM.Web.Controllers
{
    public class PublicController : Controller
    {
        readonly SessionContext _context = new SessionContext();
        //private string GetText(string key, string page = null)
        //{
        //    if (page == null)
        //        return LocalizeHelper.Instance.GetText("COMMON", key);
        //    return LocalizeHelper.Instance.GetText(key, page);
        //}
        [Authorize]
        public ActionResult Index()
        {
            //if (!Request.IsAuthenticated)
            //    return RedirectPermanent("/login");

            if (!ConfigValues.NEED_LANDING_PAGE)
                return Redirect("/admin");
            return View();
        }

        public ActionResult PublicLoginInfo()
        {
            return PartialView("Controls/LoginInfo", _context.GetUserData());
        }

        public ActionResult LeftMenu()
        {
            return PartialView("Controls/_LeftMenu", _context.GetUserData());
        }

        ///// <summary>
        ///// This action is used to receive submitted data from Webform module
        ///// </summary>
        ///// <param name="formCollection"></param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[HttpPost]
        //public object Webform(FormCollection formCollection)
        //{
        //    try
        //    {
        //        var publicId = formCollection["PublicId"];
        //        formCollection.Remove("PublicId");

        //        var returnUrl = formCollection["ReturnUrl"];
        //        formCollection.Remove("ReturnUrl");

        //        var webform = WebformBM.Instance.Single(w => w.PublicId == publicId);

        //        if (webform != null)
        //        {
        //            var moduleId = webform.ModuleId;
        //            var userId = webform.AssignedTo;

        //            var module = ModuleBM.Instance.Single(m => m.Id == moduleId);
        //            var objectName = !string.IsNullOrEmpty(module.DefaultTable) && module.DefaultTable.EndsWith("s")
        //                ? module.DefaultTable.Remove(module.DefaultTable.Length - 1)
        //                : module.DefaultTable;

        //            var assembly = Assembly.Load(@"LeonardCRM.DataLayer");
        //            var instance = assembly.CreateInstance("LeonardCRM.DataLayer.ModelEntities." + objectName, false,
        //                BindingFlags.Default, null, null, null, null);

        //            var custFields = EntityFieldBM.Instance.GetAllCustFieldByModule(moduleId);
        //            var lstCustomFields = new List<Eli_FieldData>();

        //            var lstWebformDetails = WebformBM.Instance.GetWebformDetailsByModuleId(webform.Id);
        //            foreach (string key in formCollection)
        //            {
        //                PropertyInfo propertyInfo = instance.GetType().GetProperty(key);
        //                var custField = custFields.SingleOrDefault(cf => cf.FieldName == key);
        //                var formDetails = lstWebformDetails.SingleOrDefault(d => d.Eli_EntityFields.FieldName == key);
        //                if (custField != null)
        //                {
        //                    var eliFieldData = new Eli_FieldData()
        //                    {
        //                        Id = 0,
        //                        CustFieldId = custField.Id,
        //                        FieldData =
        //                            formDetails == null || string.IsNullOrEmpty(formDetails.OverrideValue)
        //                                ? formCollection[key]
        //                                : formDetails.OverrideValue
        //                    };
        //                    SetDefaultAudit(eliFieldData, 0, userId);
        //                    lstCustomFields.Add(eliFieldData);
        //                }
        //                else
        //                {
        //                    if (propertyInfo != null)
        //                    {
        //                        if (propertyInfo.PropertyType.Name.StartsWith("Bool"))
        //                        {
        //                            propertyInfo.SetValue(instance,
        //                                Convert.ChangeType(formCollection[key] == "on", propertyInfo.PropertyType), null);
        //                        }
        //                        else
        //                        {
        //                            var fieldValue = formDetails == null ||
        //                                             string.IsNullOrEmpty(formDetails.OverrideValue)
        //                                ? formCollection[key]
        //                                : formDetails.OverrideValue;
        //                            propertyInfo.SetValue(instance, StringToType(fieldValue, propertyInfo.PropertyType),
        //                                null);
        //                        }
        //                    }
        //                }
        //            }

        //            //set default source for salecustomer module
        //            if (moduleId == Constant.ModuleCustomer)
        //            {
        //                int sourceId = GetSourceId(moduleId);
        //                if (sourceId > 0)
        //                {
        //                    PropertyInfo sourceInfo = instance.GetType().GetProperty("Source");
        //                    if (sourceInfo != null)
        //                    {
        //                        sourceInfo.SetValue(instance, StringToType(sourceId.ToString(), sourceInfo.PropertyType),
        //                            null);
        //                    }
        //                }
        //            }

        //            //set audit field
        //            SetDefaultAudit(instance, 0, userId);
        //            //set field "ResponsibleUsers"
        //            PropertyInfo propertyAssignTo = instance.GetType().GetProperty("ResponsibleUsers");
        //            if (propertyAssignTo != null)
        //            {
        //                propertyAssignTo.SetValue(instance,
        //                    Convert.ChangeType(userId + ",", propertyAssignTo.PropertyType), null);
        //            }


        //            string msg = new ObjectValidator(moduleId).ValidateObject(instance);

        //            if (string.IsNullOrEmpty(msg))
        //            {
        //                var status = WebformBM.Instance.SaveWebformObj(moduleId, instance, lstCustomFields);
        //                if (status > 0)
        //                {
        //                    if (!string.IsNullOrEmpty(returnUrl))
        //                    {
        //                        return Redirect(returnUrl);
        //                    }
        //                    else
        //                    {
        //                        return
        //                            Json(new ResultObj(ResultCodes.Success,
        //                                GetText("COMMON", "SAVE_SUCCESS_MESSAGE"), 0));
        //                    }
        //                }
        //            }

        //            return Json(new ResultObj(ResultCodes.ValidationError, msg, 0));
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Log(exception.Message, exception);
        //    }
        //    return
        //        Json(new ResultObj(ResultCodes.SavingFailed, GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"),
        //            0));
        //}

        //private static object StringToType(string value, Type propertyType)
        //{
        //    var underlyingType = Nullable.GetUnderlyingType(propertyType);
        //    if (underlyingType == null)
        //        return Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
        //    return String.IsNullOrEmpty(value)
        //        ? null
        //        : Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
        //}

        //private void SetDefaultAudit(object src, long srcKeyValue, int userId)
        //{
        //    if (srcKeyValue <= 0)
        //    {
        //        TrySetProperty(src, "CreatedDate", DateTime.Now);
        //        TrySetProperty(src, "CreatedBy", userId);
        //    }
        //    TrySetProperty(src, "ModifiedDate", DateTime.Now);
        //    TrySetProperty(src, "ModifiedBy", userId);
        //}

        //private void TrySetProperty(object obj, string property, object value)
        //{
        //    var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
        //    if (prop != null && prop.CanWrite)
        //        prop.SetValue(obj, value, null);
        //}

        //private int GetSourceId(int moduleId)
        //{
        //    var picklist = ListNameBM.Instance.GetListNameValuesByModuleId(moduleId);
        //    var source = picklist.Where(p => p.FieldName.ToLower() == "source").ToList();
        //    if (source.Any())
        //    {
        //        var webform = source.SingleOrDefault(s => s.Description.ToLower() == "web form");
        //        if (webform != null)
        //            return webform.Id;
        //        return source.First().Id;
        //    }
        //    return -1;
        //}

        ///// <summary>
        ///// External login using access token based
        ///// </summary>
        ///// <param name="loginName"></param>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Login([System.Web.Http.FromBody] string loginName, [System.Web.Http.FromBody] string password)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(loginName) && !string.IsNullOrEmpty(password))
        //        {
        //            string accessToken;
        //            var status = AuthenticationModule.AuthenticateUser(loginName, password, out accessToken);
        //            return Json(new AuthenTicket(accessToken, status.ToString(), ""));
        //        }
        //        return
        //            Json(new AuthenTicket("", LoginStatus.LoginFail.ToString(),
        //                GetText("LOGIN", "NO_ACCOUNT")));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Log(ex.Message, ex);
        //        return
        //            Json(new AuthenTicket("", LoginStatus.LoginFail.ToString(),
        //                GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER")));
        //    }
        //}

    }

    //public class AuthenTicket
    //{
    //    public AuthenTicket(string accessToken, string status, string errorMessage)
    //    {
    //        AccessToken = accessToken;
    //        Status = status;
    //        ErrorMessage = errorMessage;
    //    }
    //    public string AccessToken { get; set; }
    //    public string Status { get; set; }
    //    public string ErrorMessage { get; set; }
    //}
}
