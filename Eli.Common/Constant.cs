using System.Collections.Generic;

namespace Eli.Common
{
    public class Constant
    {
        public const string DateFormat = "MMM dd yyyy";
        public const string NumberFormat = "{0:n0}";
        public const string UserID = "[UserID]";
        public const string UserName = "[UserName]";
        public const string EmailAddress = "[email]";
        public const string Password="[password]";
        public const string ContactMessage = "[contact_message]";
        public const string Fullname = "[fullname]";
        public const string ActivationLink = "[link]";
        public const string ActivationCode = "[ActivationCode]";
        public const string Time = "[time]";
        public const string SurveyLink = "[link]";
        public const string UserStatus = "[user_status]";
        public const string LoginLink = "[login_link]";
        public const string Subject = "[subject]";
        public const string StartDate = "[start_date]";
        public const string DueDate = "[due_date]";
        public const string Description = "[description]";
        public const string RecoveryPasswordLink = "[recovery_link]";

        public const string InvOrder = "[inv_order]";
        public const string InvCreatedDate = "[inv_created_date]";
        public const string InvDuedDate = "[inv_due_date]";
        public const string InvTotal = "[inv_total]";
        public const string InvStatus = "[inv_status]";

        public const string ContactSender = "[contact_sender]";

        public const string OrderType = "[order_type]";
        public const string POLink = "[po_link]";
        public const string AcceptLink = "[accept_link]";
        public const string RejectLink = "[reject_link]";

        public const string ApplicantNumber = "[applicant_number]";
        public const string ApplicantStatus = "[applicant_status]";
        public const string HomeLink = "[home_link]";
        public const string NoteForRent = "[note_rent]";
        public const string DisapproveReason = "[disapprove_reason]";
        public const string RepairNote = "[repaired_note]";

        public const string CurrentDayExtField = "CurrentDay";
        public const string CurrentMonthExtField = "CurrentMonth";
        public const string CurrentYearExtField = "CurrentYear";
        public const string CurrentDateExtField = "CurrentDate";
        public const string CurrentDateTimeExtField = "CurrentDateTime";
        public const string PercentOfCapitializationPeriod = "PercentOfCapitializationPeriod";
        public const string OrderPromotion = "Order_Promotion";
        public const string CapitializationPeriodConfigFormat = "CAPITAL_PERIOD_{0}";

        public const int ModuleUser = 7;
        public const int ModuleCustomer = 2;
        public const int ModuleOrder = 3;
        public const int ModuleInvoice = 4;
        public const int ModuleAppointment = 39;
        public const int ModuleOrderDelivery = 29;
        public const int ModuleSaleComplete = 30;
        public static int ModuleCustomerReferences = 28;
        public const string AuthorizeToken = "AccessToken";
        public const string ApplicationName = "AppID";
        public const string ApplicationKey = "EliCRM@2014";
        public const int ListType = 5;
        public const int NumOfTransactionItem = 5;
        public const int OrderAttachmentListId = 117;
        public const int NumberOfSearchedItems = 100;
        public const int CountryPickListId = 98;
        public const string RegAppParamLink = "Register_Appointment_Webform_Link";
        public const double DefaultDurationWebformApp = 30;//minutes
        public const string ANGULAR_APP_ROOT = "~/Scripts/";
        public const string ContentStyleRoot = "~/Content/";
        public const int MaxFileNameLength = 25;
        public const string DecimalFormat = "#,###.00";
        public const string ContractFileNameFormat = "Contract_{0}.pdf";
        public const string DeliveryFormFileNameFormat = "Delivery_{0}.pdf";
        public const string AcceptFormFileNameFormat = "Acceptance_{0}.pdf";
        public const string FinalNameFormat = "Final_{0}.pdf";
        public const string SnapShotNameFormat = "Snapshot_{0}.png";

        public const string ReplaceSignatureField = "@order_LesseeSignature@";
        public const string ReplaceCoSignatureField = "@order_CoSignature@";
        public static string BlankSignature = "__________________________";
        public const string DeliveryCustomerSignName = "CusSignature_{0}.png";

        public const string EmailSignature = "[email_signature]";

        #region Cache keys
        public const string NavigationCacheKey = "navigations";
        public const string CurrentUserKey = "CurrentUser";
        public const string SiteSettings = "SiteSettings";
        public const string CurrentLanguage = "CurrentLanguage";
        public const string ResourceStrings = "Resource";
        public const string ModulesCacheKey = "Modules";
        public const string ViewColumnsCacheKey = "ViewColumns";
        public const string AllEntityFieldsCacheKey = "AllEntityFields";
        public const string OnlyActiveSurveyCacheKey = "traceone_OnlyActiveSurvey";
        #endregion

        #region QueryString parameters
        public const string USERID = "uid";
        public const string CONTENTID = "cid";
        public const string ACTIVEID = "acid";
        public const string LogId = "lid";
        public const string PAGEMODE = "pm";
        public const string UPLOADFILE = "file";
        public const string DefaultHtmlFont = @"<style>* {        font-size:23px;        font-family: ""times new roman"", times;    }</style>";
        public const string BodyStandardWrapper = "<div style=\"width : 1137px\">{0}</div>";
        #endregion
        
        #region Cookies names
        public const string PreferLanguage = "preferLanguage";
        public const string SiteSettingsCookie = "settings"; 
        public const string ModulesCookie = "modules";
        #endregion

        #region Session keys

        public const string ss_UserId = "userID";


        #endregion

        #region Fields require for load dynamic view

        public static readonly List<string> RequireFields = new List<string>() {"id", "responsibleusers", "createdby"};        



        #endregion



    }
}
