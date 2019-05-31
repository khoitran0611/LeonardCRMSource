using System;

namespace Eli.Common
{
    [Flags]
    public enum UserStatus
    {
        Active = 1,
        Suspended = 2,
        Not_Activated = 3,
        //Add new Status
    }

    [Flags]
    public enum UserRoles
    {
        Administrator = 1,
        ContractManager = 2,
        Store = 3,
        Customer = 20,
        DeliveryStaff = 22,
        Collector = 26,
        ClientAdmin = 23
    }

    [Flags]
    public enum EmailFlag
    {
        Show = 0,
        Hide = 1
    }

    [Flags]
    public enum Permission
    {
        Read = 1,
        CreateEdit = 2,
        Delete = 3
    }
    [Flags]
    public enum Priority
    {
        Low = 1,
        Normal = 2,
        Hight = 3,
        Urgent = 4
    }
    [Flags]
    public enum MaritalStatus
    {
        None = 0,
        Single = 1,
        NotLooking = 2,
        InRelationship = 3,
        Married = 4
    }

    [Flags]
    public enum LoginStatus
    {
        Success = 0,
        NoAccount = 1,
        BanStatus = 2,
        NotActivated = 3,
        LoginFail = 4,
    }

    [Flags]
    public enum LanguageCodes
    {
        en = 1,
        vi = 2,
        ru = 3,
    }

    [Flags]
    public enum MatchCase
    {
        MatchAll = 1,
        MatchEvery = 2,
        MatchAny = 3,
    }

    [Flags]
    public enum LinkTarget
    {
        _blank = 1,
        _parent = 2,
        _search = 3,
        _self = 4,
        _top = 5,
    }

    [Flags]
    public enum ReturnStatus
    {
        Success = 200,
        Error = 500,
        UnAutherized = 401
    }

    public enum UserSort
    {
        Name,
        Email,
        Status,
        CreatedDate,
        Flags
    }

    [Flags]
    public enum AuditEvent
    {
        Delete = 2,
        Assign = 4,
        ChangeStatus = 6
    }
    [Flags]
    public enum PageMode
    {
        Edit = 1,
        Create = 2,
        Update = 3
    }

    public enum MailTemplates
    {
        TEMPLATE_MAIL_ACTIVATE = 1,
        TEMPLATE_MAIL_FORGOT_PASS = 2,
        TEMPLATE_MAIL_ACTIVATE_SUCCESS = 3,
        TEMPLATE_MAIL_PASSWORD_CHANGE_NOTIFICATION = 4,
        TEMPLATE_MAIL_RESEND_ACTIVATION_NOTIFICATION = 5,
        TEMPLATE_MAIL_NOTIFY_SUBMIT_APPLICANT = 6,
        TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT = 7,
        TEMPLATE_MAIL_NOTIFY_COMPLETE_DELIVERY_REQUEST = 8,
        TEMPLATE_MAIL_SEND_CONTRACT_COPY = 9,
        TEMPLATE_MAIL_COMPLETED_DELIVERY_REQUEST = 10,
        TEMPLATE_MAIL_COMPLETED_ACCEPTANCE = 11,
        TEMPLATE_MAIL_CHANGES_APPLICATION = 12,
        TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT_TO_CUSTOMER = 13,
        TEMPLATE_MAIL_NOTIFY_CHANGE_ASSIGNEE = 14,
        TEMPLATE_MAIL_CANCEL_APPLICANT = 15,
        TEMPLATE_MAIL_DISAPPOVE_APPLICANT = 16,
        TEMPLATE_MAIL_WELCOME_NEW_USER = 17,
        TEMPLATE_MAIL_NOTIFY_UPDATE_APPLICANT_TO_MANAGER = 18,
        TEMPLATE_MAIL_APPROVED_APPLICANT = 19,
        TEMPLATE_MAIL_DIRVER_COMPLETE_NO_CUSTOMER_EMAIL = 20,
        TEMPLATE_MAIL_DIRVER_COMPLETE_WITH_CUSTOMER_EMAIL = 21
    }

    public enum EliSortDirection
    {
        Asc,
        Desc
    }

    public enum DataTypes
    {
        Integer = 1,
        Date = 2,
        Decimal = 3,
        Text = 4,
        List = 5,
        TextArea = 6,
        Time = 7,
        Email = 8,
        Url = 9,
        MultiSelectBox = 10,
        CheckBox = 11,
        Currency = 14
    }

    public enum SearchOperators
    {
        Equals,
        Contains,
        Between,
        StartsWith,
        GreaterThan,
        LessThan,
        GreaterEqual,
        LessThanEqual,
        Within
    }

    public enum OverviewReportOptions
    {
        RecentlyAdded = 0,
        Last7Days = 7,
        Last30Days = 30,
        Last365Days = 365
    }

    #region ImageHelper enums
    public enum ThumbnailType
    {
        Crop = 1,
        Percent = 2,
        FixedSize = 3,
        ConstrainProportions = 4,
        Default = 5
    }

    public enum Dimensions
    {
        Width = 1,
        Height = 2
    }

    public enum AnchorPosition
    {
        Top = 1,
        Center = 2,
        Bottom = 3,
        Left = 4,
        Right = 5
    }
    #endregion

    public enum SaleCustomerSource
    {
        OnForm = 1,
        Import = 2,
        DataInject = 3,
        Duplication = 4
    }
    public enum RollBackStatus
    {
        Success = 1,
        Failure = 2
    }

    public enum OrderStatus
    {
        Pending = 4,
        PreApproved = 5,
        //InProgress = 6,
        PendingCusAccept = 6,
        //ContractSigned = 7,
        //PaidFull = 8,
        InProgress = 8,
        PendingDelivery = 244,
        DeliveredNotSigned = 245,
        Completed = 246,
        Rejected = 256
    }

    public enum DeliveryType
    {
        StandardDelivery = 258,
        MoveJob = 259,
        TileDown = 383,
        Other = 388
    }

    public enum InvoiceStatus
    {        
        Paid = 9,
        NotFullPaid = 172,
        Rejected = 10,
        Accepted = 350
    }

    public enum ResidenceType
    {
        Own = 237,
        Rent = 238
    }

    public enum ResidenceTypePoint
    {
        Own = 4,
        Rent = 2
    }

    public enum LandType
    {
        Own = 239,
        Rent = 240
    }

    public enum LandTypePoint
    {
        Own = 4,
        Rent = 2
    }

    public enum CapitalizationPeriod
    {
        Perioid18Cmos = 253,
        Perioid36Cmos = 254,
        Perioid48Cmos = 255,
        Perioid60Cmos = 387,
    }
}
