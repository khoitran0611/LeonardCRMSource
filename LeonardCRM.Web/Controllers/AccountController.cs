using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Eli.Common;
using LeonardCRM.BusinessLayer;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.BusinessLayer.Feature;
using LeonardCRM.DataLayer.ModelEntities;
using System.Web;

namespace LeonardCRM.Web.Controllers
{
    public class AccountController : BasicController
    {
        readonly SessionContext _context = new SessionContext();
        private string GetText(string key, string page = null)
        {
            if (page == null)
                return LocalizeHelper.Instance.GetText("FORGOT_PASSWORD", key);
            return LocalizeHelper.Instance.GetText(key, page);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginResourceString();
            var cookie = Response.Cookies[Constant.CurrentLanguage];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            cookie = Response.Cookies["user"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            //remove this line in production-----
            new CacheManager().Remove(Constant.ViewColumnsCacheKey);
            //--------------------------------

            if (Request.IsAuthenticated)
            {
                _context.SignOut();
            }
            return View("Login", string.Empty);
        }

        public ActionResult LoginInfo()
        {
            return PartialView("_LoginInfo", _context.GetUserData());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Authenticate()
        {
            LoginResourceString();
            var username = Request["username"];
            var password = Request["password"];
            var remember = Request["remember"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var status = new AccountFeature().Login(username, password, remember == "on", true);
                if (status == LoginStatus.Success)
                {
                    var authenticatedUser = UserBM.Instance.SingleLoadWithReferences(u => u.Email == username, "Eli_Roles");
                    _context.SetAuthenticationToken(username, password, remember == "on", authenticatedUser);

                    if (authenticatedUser.RoleId != (int)UserRoles.Customer && authenticatedUser.RoleId != (int)UserRoles.DeliveryStaff)
                        return PartialView("LoadResource");
                    return PartialView("LoadFrontendResource");
                }
                return RedirectToAction("Login", "Account", new { st = status.ToString() });
            }
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult PasswordRecovery()
        {
            RecoverPasswordString();
            var model = new Eli_User();
            return View("PasswordRecoveryPartial", model);
        }

        //private void LoadSiteSettings()
        //{
        //    //var settings = new BusinessLayer.DataControllers.RegistryApiController().GetRegistry();

        //    if (String.IsNullOrEmpty(SessionManager.PreferLanguage))
        //        SessionManager.PreferLanguage = CurrentCulture;

        //    var currentCulture = SessionManager.PreferLanguage;
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(currentCulture);
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);

        //    var cookie = new HttpCookie(Constant.CurrentLanguage, currentCulture)
        //    {
        //        Expires = DateTime.Now.AddDays(365)
        //    };
        //    Response.Cookies.Add(cookie);

        //    //settings.SMTP_CREDENTIAL_EMAIL = "";
        //    //settings.SMTP_CREDENTIAL_PASSWORD = "";
        //    //cookie = new HttpCookie(Constant.SiteSettingsCookie, JsonConvert.SerializeObject(settings))
        //    //    {
        //    //        Expires = DateTime.Now.AddDays(365)
        //    //    };
        //    //Response.Cookies.Add(cookie);
        //}

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PasswordRecovery(Eli_User model)
        {
            RecoverPasswordString();
            try
            {
                if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Captcha))
                {
                    var email = model.Email.Trim();

                    var msg = ValidateFillForm(model.Email, model.Captcha);
                    if (string.IsNullOrEmpty(msg))
                    {
                        var hdUser = UserBM.Instance.GetUserByEmail(email);

                        msg = ValidateFillForm(hdUser);

                        if (string.IsNullOrEmpty(msg))
                        {
                            //var pwd = Membership.GeneratePassword(12, 1);
                            //hdUser.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "md5");
                            SendMail(hdUser.Name, hdUser.Email, this.ServerURL + ConfigValues.RESET_PASSWORD_LINK + HttpUtility.UrlEncode(SecurityHelper.Encrypt(hdUser.Email + "," + DateTime.Now.Ticks))); //send mail
                            //UserBM.Instance.Update(hdUser); //update password

                            IList<string> success = new List<string>();
                            success.Add(GetText("RESET_MSG"));
                            ViewBag.Success = success;
                            model.Email = string.Empty;
                        }
                    }
                    model.Captcha = string.Empty;
                    return View("PasswordRecoveryPartial", model);
                }
                return View("PasswordRecoveryPartial", new Eli_User());
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                IList<string> errors = new List<string>();
                errors.Add(GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"));
                ViewBag.Error = errors;
                return View("PasswordRecoveryPartial", new Eli_User());
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string p)
        {
            try
            {
                p = SecurityHelper.Decrypt(p);
                var param = p.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (CheckRecoveryLink(param))
                {
                    return View("Error");
                }

                var model = new Eli_User()
                {
                    Email = param[0].ToString()
                };

                ResetPassResourceString();

                return View("ConfirmResetPassword", model);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return Redirect("/error");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(string p, Eli_User model)
        {
            try
            {
                p = SecurityHelper.Decrypt(p);
                var param = p.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (CheckRecoveryLink(param))
                {
                    return View("Error");
                }

                var errorMsg = ValidateRecoveryPassword(model);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ViewBag.ErrorMsg = errorMsg;
                }
                else
                {
                    var hdUser = UserBM.Instance.GetUserByEmail(param[0]);
                    hdUser.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "md5");
                    var status = UserBM.Instance.Update(hdUser); //update password
                    if (status > 0)
                    {
                        var isSendSuccess = SendMailResetPassSuccess(param[0]);

                        IList<string> success = new List<string>();
                        success.Add(GetText(isSendSuccess ? "RESET_SUCCESS_MSG" : "RESET_SUCCESS_BUT_NOT_NOTIFY_MSG"));
                        success.Add(GetText("BACK_LOGIN"));
                        ViewBag.Success = success;
                    }
                    else
                    {
                        ViewBag.ErrorMsg = GetText("COMMON", "SAVE_FAIL_MESSAGE_USER"); 
                    }
                }

                //reset form
                model.Password = "";
                model.PasswordConfirm = "";

                //load resource
                ResetPassResourceString();

                return View("ConfirmResetPassword", model);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return Redirect("/error");
            }
        }

        private void RecoverPasswordString()
        {
            ViewBag.Errors = new List<string>();
            ViewBag.Success = new List<string>();
            ViewBag.Title = GetText("TITLE");
            ViewBag.Email = GetText("EMAIL");
            ViewBag.Email = GetText("EMAIL");
            ViewBag.InvalidEmail = GetText("EMAIL_INVALID");
            ViewBag.Captcha = GetText("CAPTCHA");
            ViewBag.ReqiredCaptcha = GetText("REQUIRED_CAPTCHA");
            ViewBag.ErrorTitle = GetText("ERROR_TITLE");
            ViewBag.SendButton = GetText("SEND");
        }

        private string ValidateFillForm(string email, string captcha)
        {
            IList<string> errors = new List<string>();

            var result = string.Empty;
            if (!Utilities.IsMail(email))
            {
                result = GetText("EMAIL_INVALID");
                errors.Add(result);
            }
            if (Session["CaptchaImageText"] == null)
            {
                result = GetText("COMMON", "CAPTCHA_EXPIRED");
                errors.Add(result);
            }
            else
            {
                var capchaStr = Session["CaptchaImageText"].ToString().ToLower();
                if (captcha.ToLower() != capchaStr)
                {
                    result = GetText("COMMON", "CAPTCHA_INVALID");
                    errors.Add(result);
                }
            }
            ViewBag.Errors = errors;
            return result;
        }

        private string ValidateFillForm(Eli_User hdUser)
        {
            IList<string> errors = ViewBag.Errors as List<string>;
            //Check List Name
            var result = string.Empty;
            if (hdUser == null)
            {
                result = GetText("LOGIN", "NO_ACCOUNT");
                errors.Add(result);
                return result;
            }
            return result;
        }

        private void SendMail(string fullname, string email, string recoveryLink)
        {
            var emailFrom = SiteSettings.SMTP_CREDENTIAL_EMAIL;
            var emailTo = email;
            var templateName = MailTemplates.TEMPLATE_MAIL_FORGOT_PASS.ToString();
            var template = MailTemplateBM.Instance.Single(
                t => t.TemplateName == templateName);
            var subject = template.Subject;
            var emailBody = template.TemplateContent;

            emailBody = emailBody.Replace(Constant.Fullname, fullname);
            emailBody = emailBody.Replace(Constant.EmailAddress, email);
            emailBody = emailBody.Replace(Constant.RecoveryPasswordLink, recoveryLink);
            emailBody = emailBody.Replace(Constant.HomeLink, this.ServerURL);
            var mailServer = RegistryBM.Instance.GetMailServerInfo();

            MailHelper.SendMail(mailServer, emailFrom, emailTo, subject, emailBody);
        }

        private void LoginResourceString()
        {
            ViewBag.NO_ACCOUNT = GetText("LOGIN", "NO_ACCOUNT");
            ViewBag.ACCOUNT_NOT_ACTIVATED = GetText("LOGIN", "ACCOUNT_NOT_ACTIVATED");
            ViewBag.ACCOUNT_BANNED = GetText("LOGIN", "ACCOUNT_BANNED");
            ViewBag.USER_NAME = GetText("LOGIN", "USER_NAME");
            ViewBag.PASSWORD_RES = GetText("LOGIN", "PASSWORD");
            ViewBag.BUTTON_LOGIN = GetText("LOGIN", "BUTTON_LOGIN");
            ViewBag.REMEMEBR = GetText("LOGIN", "REMEMEBR");
            ViewBag.FORGOT_PASSWORD = GetText("LOGIN", "FORGOT_PASSWORD");
            ViewBag.Title = GetText("LOGIN", "TITLE");
            ViewBag.ErrorTitle = GetText("ERROR_TITLE");
        }

        private void RegisterResourceString()
        {
            ViewBag.Errors = new List<string>();
            ViewBag.Success = new List<string>();
            ViewBag.Title = GetText("REGISTER", "TITLE");
            ViewBag.Name = GetText("REGISTER", "NAME");
            ViewBag.Email = GetText("REGISTER", "EMAIL");
            ViewBag.Password = GetText("REGISTER", "PASSWORD");
            ViewBag.PasswordConfirm = GetText("REGISTER", "PASSWORDCONFIRM");
            ViewBag.InvalidEmail = GetText("REGISTER", "EMAIL_INVALID");
            ViewBag.Captcha = GetText("REGISTER", "CAPTCHA");
            ViewBag.ReqiredCaptcha = GetText("REGISTER", "REQUIRED_CAPTCHA");
            ViewBag.ReqiredName = GetText("REGISTER", "REQUIRED_NAME");
            ViewBag.ReqiredPassword = GetText("REGISTER", "REQUIRED_PASSWORD");
            ViewBag.ReqiredPasswordConfirm = GetText("REGISTER", "REQUIRED_PASSWORDCONFIRM");
            ViewBag.ErrorTitle = GetText("REGISTER", "ERROR_TITLE");
            ViewBag.RegisterButton = GetText("REGISTER", "REGISTER_BUTTON");
            ViewBag.LoginButton = GetText("REGISTER", "BUTTON_LOGIN");
            ViewBag.FormHeader = GetText("REGISTER", "FORM_HEADER");

            ViewBag.CongratulationHeading = GetText("CONGRATULATION", "HEADING");
            ViewBag.CongratulationMessage = GetText("CONGRATULATION", "MSG");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterResourceString();
            var model = new Eli_User();
            return View("Register", model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(Eli_User model)
        {
            RegisterResourceString();
            try
            {
                if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.PasswordConfirm) && !string.IsNullOrEmpty(model.Captcha))
                {
                    var msg = ValidateRegisterForm(model);
                    if (string.IsNullOrEmpty(msg))
                    {
                        model.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "md5");
                        model.RoleId = (int)UserRoles.Customer;
                        model.ActivationCode = Guid.NewGuid();
                        model.Status = (int)UserStatus.Active;
                        model.CreatedDate = DateTime.Now;
                        model.ModifiedDate = DateTime.Now;

                        var res = UserBM.Instance.InsertUser(model);
                        if (res > 0)
                        {
                            //SendActiveAccountMail(model.Name, model.Email, model.ActivationCode.ToString());

                            var success = new List<string>();
                            success.Add(ViewBag.CongratulationMessage);
                            ViewBag.Success = success;
                            ViewBag.Title = GetText("CONGRATULATION", "TITLE");

                            model.Name = string.Empty;
                            model.Email = string.Empty;
                            model.Password = string.Empty;
                            model.PasswordConfirm = string.Empty;
                        }
                        else
                        {
                            var errors = new List<string>();
                            errors.Add(GetText("COMMON", "SAVE_FAIL_MESSAGE_ADMIN"));
                            ViewBag.Errors = errors;
                        }
                    }

                    model.Captcha = string.Empty;
                    return View("Register", model);
                }

                return View("Register", new Eli_User());
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                IList<string> errors = new List<string>();
                errors.Add(GetText("COMMON", "UNEXPECTED_ERROR_MESSAGE_USER"));
                ViewBag.Errors = errors;
                var success = new List<string>();
                ViewBag.Success = success;
                return View("Register", new Eli_User());
            }
        }

        private string ValidateRegisterForm(Eli_User hdUser)
        {
            IList<string> errors = new List<string>();

            var result = string.Empty;
            // validate email
            if (!Utilities.IsMail(hdUser.Email))
            {
                result = GetText("EMAIL_INVALID");
                errors.Add(result);
            }

            // validate password
            if (hdUser.Password != hdUser.PasswordConfirm)
            {
                result = GetText("PASSWORD_MATCH");
                errors.Add(result);
            }
            else
            {
                if (!Utilities.IsPasswordValid(hdUser.Password))
                {
                    result = GetText("PASSWORD_INVALID");
                    errors.Add(result);
                }
            }

            // validate captcha
            if (Session["CaptchaImageText"] == null)
            {
                result = GetText("COMMON", "CAPTCHA_EXPIRED");
                errors.Add(result);
            }
            else
            {
                var capchaStr = Session["CaptchaImageText"].ToString().ToLower();
                if (hdUser.Captcha.ToLower() != capchaStr)
                {
                    result = GetText("COMMON", "CAPTCHA_INVALID");
                    errors.Add(result);
                }
            }
            if (errors.Count == 0)
            {
                if (UserBM.Instance.ExistEmail(hdUser.Email))
                {
                    result = GetText("EMAIL_EXIST");
                    errors.Add(result);
                }
            }
            ViewBag.Errors = errors;
            return result;
        }

        private void SendActiveAccountMail(string fullname, string email, string activationCode)
        {
            var mailServer = RegistryBM.Instance.GetMailServerInfo();
            var emailFrom = mailServer.Username;
            var emailTo = email;
            var templateName = MailTemplates.TEMPLATE_MAIL_ACTIVATE.ToString();
            var template = MailTemplateBM.Instance.Single(
                t => t.TemplateName == templateName);
            var subject = template.Subject;
            var emailBody = template.TemplateContent;

            emailBody = emailBody.Replace(Constant.Fullname, fullname);
            emailBody = emailBody.Replace(Constant.EmailAddress, email);

            var activationLink = Request.Url.AbsoluteUri.Replace("Register", "Activation/?ActivationCode=" + activationCode);
            emailBody = emailBody.Replace(Constant.ActivationLink, activationLink);

            MailHelper.SendMail(mailServer, emailFrom, emailTo, subject, emailBody);
        }

        private void SendActiveAccountSuccessfulMail(string fullname, string email)
        {
            var mailServer = RegistryBM.Instance.GetMailServerInfo();
            var emailFrom = mailServer.Username;
            var templateName = MailTemplates.TEMPLATE_MAIL_ACTIVATE_SUCCESS.ToString();
            var template = MailTemplateBM.Instance.Single(
                t => t.TemplateName == templateName);
            var subject = template.Subject;
            var emailBody = template.TemplateContent;

            emailBody = emailBody.Replace(Constant.Fullname, fullname);
            emailBody = emailBody.Replace(Constant.HomeLink, this.ServerURL);

            MailHelper.SendMail(mailServer, emailFrom, email, subject, emailBody);
        }

        private void CompleteRegisterResourceString()
        {
            ViewBag.Success = false;
            ViewBag.Title = GetText("COMPLETE_REGISTRATION", "TITLE");
            ViewBag.CompleteRegistration = GetText("COMPLETE_REGISTRATION", "USER_IS_ACTIVED");
            ViewBag.CompleteRegistrationHeading = GetText("COMPLETE_REGISTRATION", "HEADING");
            ViewBag.LoginLink = GetText("COMPLETE_REGISTRATION", "LOGIN_LINK");
        }

        [AllowAnonymous]
        public ActionResult Activation(string activationCode)
        {
            CompleteRegisterResourceString();

            if (!string.IsNullOrEmpty(activationCode))
            {
                var user = UserBM.Instance.GetUserByCode(activationCode);
                if (user != null)
                {
                    switch (user.Status)
                    {
                        case (int)UserStatus.Active:
                            ViewBag.ErrorMessage = GetText("COMPLETE_REGISTRATION", "USER_ACTIVED");
                            break;
                        case (int)UserStatus.Suspended:
                            ViewBag.ErrorMessage = GetText("COMPLETE_REGISTRATION", "USER_BANNED");
                            break;
                        case (int)UserStatus.Not_Activated:
                            var res = UserBM.Instance.ActiveUser(user);
                            if (res > 0)
                            {
                                ViewBag.Success = true;
                                SendActiveAccountSuccessfulMail(user.Name, user.Email);
                            }
                            else
                            {
                                ViewBag.Success = false;
                                ViewBag.ErrorMessage = GetText("COMMON", "SAVE_FAIL_MESSAGE_ADMIN");
                            }
                            break;
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = GetText("COMPLETE_REGISTRATION", "CODE_INVALID");
                }
            }

            return View("Activation", string.Empty);
        }

        private void ResetPassResourceString()
        {
            ViewBag.PASSWORD_RES = GetText("LOGIN", "PASSWORD");
            ViewBag.CONFIRM_PASSWORD_RES = GetText("CONFIRM_PASSWORD");
            ViewBag.SUBMIT_LOGIN = GetText("SUBMIT_LOGIN");
            ViewBag.NEW_PASSWORD_ERROR_MSG = GetText("NEW_PASSWORD_ERROR_MSG");
            ViewBag.CONFIRM_NEW_PASSWORD_ERROR_MSG = GetText("CONFIRM_NEW_PASSWORD_ERROR_MSG");
            ViewBag.ErrorTitle = GetText("ERROR_TITLE");
            ViewBag.PASSWORD_CONFIRMATION_NO_MATCH_ERROR_MSG = GetText("PASSWORD_CONFIRMATION_NO_MATCH_ERROR_MSG");
            ViewBag.RESET_PASSWORD_TITLE = GetText("RESET_PASSWORD_TITLE");
            ViewBag.Title = GetText("RESET_PASSWORD_PAGE_HEADER");
        }

        private string ValidateRecoveryPassword(Eli_User model)
        {
            var resultMsg = "";

            if (string.IsNullOrEmpty(model.Password))
            {
                resultMsg += "* " + GetText("LOGIN", "NEW_PASSWORD_ERROR_MSG")  + "<br/>";
            }

            if (string.IsNullOrEmpty(model.PasswordConfirm))
            {
                resultMsg += "* " + GetText("LOGIN", "CONFIRM_NEW_PASSWORD_ERROR_MSG") + "<br/>";
            }

            if (!string.IsNullOrEmpty(model.Password) && !Utilities.IsPasswordValid(model.Password))
                resultMsg += "* " + GetText("USERS", "PASSWORD_INVALID") + "<br>";

            if (!string.IsNullOrEmpty(model.Password) &&
                !string.IsNullOrEmpty(model.PasswordConfirm) &&
                model.Password != model.PasswordConfirm)
            {
                resultMsg += "* " + GetText("LOGIN", "PASSWORD_CONFIRMATION_NO_MATCH_ERROR_MSG")  + "<br/>";
            }         

            return resultMsg;
        }

        private bool CheckRecoveryLink(string[] param)
        {
            var isValid = false;
            if (param.Length < 2)
            {
                ViewBag.Msg = GetText("LOGIN", "RECOVERY_PASS_LINK_INVALID_ERROR_MSG");
                isValid = true;
            }
            else if (DateTime.Now.Subtract(new DateTime(long.Parse(param[1]))).TotalHours >= 24)
            {
                ViewBag.Msg = GetText("LOGIN", "RECOVERY_PASS_LINK_EXPIRED_ERROR_MSG");
                isValid = true;
            }
            return isValid;
        }

        [AllowAnonymous]
        public string GetNewGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        private bool SendMailResetPassSuccess(string email)
        {
            try
            {
                var emailFrom = SiteSettings.SMTP_CREDENTIAL_EMAIL;
                var emailTo = email;
                var templateName = MailTemplates.TEMPLATE_MAIL_PASSWORD_CHANGE_NOTIFICATION.ToString();
                var template = MailTemplateBM.Instance.Single(
                    t => t.TemplateName == templateName);
                var subject = template.Subject;
                var emailBody = template.TemplateContent;

                emailBody = emailBody.Replace(Constant.EmailAddress, email);
                emailBody = emailBody.Replace(Constant.HomeLink, this.ServerURL);
                var mailServer = RegistryBM.Instance.GetMailServerInfo();

                MailHelper.SendMail(mailServer, emailFrom, emailTo, subject, emailBody);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return false;
            }
        }
    
    }
}