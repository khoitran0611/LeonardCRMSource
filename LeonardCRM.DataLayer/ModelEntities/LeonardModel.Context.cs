﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeonardCRM.DataLayer.ModelEntities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;

    using System.Linq;
    
    public partial class LeonardUSAEntities : DbContext
    {
        public LeonardUSAEntities()
            : base("name=LeonardUSAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Eli_Currency> Eli_Currency { get; set; }
        public DbSet<Eli_CurrencyNames> Eli_CurrencyNames { get; set; }
        public DbSet<Eli_DataTypes> Eli_DataTypes { get; set; }
        public DbSet<Eli_EntityFields> Eli_EntityFields { get; set; }
        public DbSet<Eli_FieldData> Eli_FieldData { get; set; }
        public DbSet<Eli_FieldsSection> Eli_FieldsSection { get; set; }
        public DbSet<Eli_FieldsSectionDetail> Eli_FieldsSectionDetail { get; set; }
        public DbSet<Eli_ListDependency> Eli_ListDependency { get; set; }
        public DbSet<Eli_ListDependencyDetail> Eli_ListDependencyDetail { get; set; }
        public DbSet<Eli_ListNames> Eli_ListNames { get; set; }
        public DbSet<Eli_ListValues> Eli_ListValues { get; set; }
        public DbSet<Eli_Log> Eli_Log { get; set; }
        public DbSet<Eli_LoginLog> Eli_LoginLog { get; set; }
        public DbSet<Eli_MailTemplates> Eli_MailTemplates { get; set; }
        public DbSet<Eli_ModuleRelationship> Eli_ModuleRelationship { get; set; }
        public DbSet<Eli_Modules> Eli_Modules { get; set; }
        public DbSet<Eli_Notes> Eli_Notes { get; set; }
        public DbSet<Eli_OnlineUsers> Eli_OnlineUsers { get; set; }
        public DbSet<Eli_Registry> Eli_Registry { get; set; }
        public DbSet<Eli_RolesFields> Eli_RolesFields { get; set; }
        public DbSet<Eli_RolesPermissions> Eli_RolesPermissions { get; set; }
        public DbSet<Eli_SysAudit> Eli_SysAudit { get; set; }
        public DbSet<Eli_Tax> Eli_Tax { get; set; }
        public DbSet<Eli_TempViews> Eli_TempViews { get; set; }
        public DbSet<Eli_ViewColumns> Eli_ViewColumns { get; set; }
        public DbSet<Eli_ViewConditions> Eli_ViewConditions { get; set; }
        public DbSet<Eli_ViewCustom> Eli_ViewCustom { get; set; }
        public DbSet<Eli_ViewCustomColumns> Eli_ViewCustomColumns { get; set; }
        public DbSet<Eli_ViewCustomConditions> Eli_ViewCustomConditions { get; set; }
        public DbSet<Eli_ViewGroupBy> Eli_ViewGroupBy { get; set; }
        public DbSet<Eli_ViewOrderBy> Eli_ViewOrderBy { get; set; }
        public DbSet<Eli_Views> Eli_Views { get; set; }
        public DbSet<SalesContractState> SalesContractStates { get; set; }
        public DbSet<SalesContractTemplate> SalesContractTemplates { get; set; }
        public DbSet<SalesCustomerUser> SalesCustomerUsers { get; set; }
        public DbSet<SalesDocument> SalesDocuments { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvService> SalesInvServices { get; set; }
        public DbSet<SalesInvTax> SalesInvTaxes { get; set; }
        public DbSet<SalesInvTemplate> SalesInvTemplates { get; set; }
        public DbSet<SalesOrdersUser> SalesOrdersUsers { get; set; }
        public DbSet<vwAllCustomer> vwAllCustomers { get; set; }
        public DbSet<vwClient> vwClients { get; set; }
        public DbSet<vwCustomViewColumn> vwCustomViewColumns { get; set; }
        public DbSet<vwEntityFieldData> vwEntityFieldDatas { get; set; }
        public DbSet<vwFieldNameDataType> vwFieldNameDataTypes { get; set; }
        public DbSet<vwField> vwFields { get; set; }
        public DbSet<vwFieldsDataType> vwFieldsDataTypes { get; set; }
        public DbSet<vwListNameValue> vwListNameValues { get; set; }
        public DbSet<vwModuleEnittyRelationship> vwModuleEnittyRelationships { get; set; }
        public DbSet<vwModuleEntityField> vwModuleEntityFields { get; set; }
        public DbSet<vwModuleHasEntityField> vwModuleHasEntityFields { get; set; }
        public DbSet<vwModule> vwModules { get; set; }
        public DbSet<vwOrderCustomer> vwOrderCustomers { get; set; }
        public DbSet<vwPicklistDependency> vwPicklistDependencies { get; set; }
        public DbSet<vwRegistry> vwRegistries { get; set; }
        public DbSet<vwSystmAudit> vwSystmAudits { get; set; }
        public DbSet<vwUserRole> vwUserRoles { get; set; }
        public DbSet<SalesCustReference> SalesCustReferences { get; set; }
        public DbSet<vwViewMenu> vwViewMenus { get; set; }
        public DbSet<SalesCustomer> SalesCustomers { get; set; }
        public DbSet<vwViewColumn> vwViewColumns { get; set; }
        public DbSet<vwApplication> vwApplications { get; set; }
        public DbSet<Eli_Roles> Eli_Roles { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderComplete> SalesOrderCompletes { get; set; }
        public DbSet<SalesOrderDelivery> SalesOrderDeliveries { get; set; }
        public DbSet<Eli_User> Eli_User { get; set; }
    
        [EdmFunction("LeonardUSAEntities", "SPLIT")]
        public virtual IQueryable<SPLIT_Result> SPLIT(string dELIMITER, string lIST)
        {
            var dELIMITERParameter = dELIMITER != null ?
                new ObjectParameter("DELIMITER", dELIMITER) :
                new ObjectParameter("DELIMITER", typeof(string));
    
            var lISTParameter = lIST != null ?
                new ObjectParameter("LIST", lIST) :
                new ObjectParameter("LIST", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<SPLIT_Result>("[LeonardUSAEntities].[SPLIT](@DELIMITER, @LIST)", dELIMITERParameter, lISTParameter);
        }
    
        public virtual ObjectResult<GetReferenceListValues_Result> GetReferenceListValues(Nullable<int> moduleId)
        {
            var moduleIdParameter = moduleId.HasValue ?
                new ObjectParameter("moduleId", moduleId) :
                new ObjectParameter("moduleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetReferenceListValues_Result>("GetReferenceListValues", moduleIdParameter);
        }
    
        public virtual int sp_AdvanceSearch(Nullable<int> moduleId, Nullable<int> viewId, Nullable<int> id, string sqlScripts, Nullable<int> userId, Nullable<int> roleId, Nullable<int> pageIndex, Nullable<int> pageSize, string sortDirection, ObjectParameter totalRow, Nullable<bool> defaultOderBy, string columnGroup, ObjectParameter groupJsonStr)
        {
            var moduleIdParameter = moduleId.HasValue ?
                new ObjectParameter("ModuleId", moduleId) :
                new ObjectParameter("ModuleId", typeof(int));
    
            var viewIdParameter = viewId.HasValue ?
                new ObjectParameter("ViewId", viewId) :
                new ObjectParameter("ViewId", typeof(int));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var sqlScriptsParameter = sqlScripts != null ?
                new ObjectParameter("sqlScripts", sqlScripts) :
                new ObjectParameter("sqlScripts", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortDirectionParameter = sortDirection != null ?
                new ObjectParameter("sortDirection", sortDirection) :
                new ObjectParameter("sortDirection", typeof(string));
    
            var defaultOderByParameter = defaultOderBy.HasValue ?
                new ObjectParameter("defaultOderBy", defaultOderBy) :
                new ObjectParameter("defaultOderBy", typeof(bool));
    
            var columnGroupParameter = columnGroup != null ?
                new ObjectParameter("columnGroup", columnGroup) :
                new ObjectParameter("columnGroup", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AdvanceSearch", moduleIdParameter, viewIdParameter, idParameter, sqlScriptsParameter, userIdParameter, roleIdParameter, pageIndexParameter, pageSizeParameter, sortDirectionParameter, totalRow, defaultOderByParameter, columnGroupParameter, groupJsonStr);
        }
    
        public virtual int sp_CreateOrder(string orderName, Nullable<int> status, Nullable<decimal> budget, string description, Nullable<int> createdBy, Nullable<int> modifiedBy, string useridlist, string customeridlist, string filenamelist, string foldername, ObjectParameter addedOrderId)
        {
            var orderNameParameter = orderName != null ?
                new ObjectParameter("OrderName", orderName) :
                new ObjectParameter("OrderName", typeof(string));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(int));
    
            var budgetParameter = budget.HasValue ?
                new ObjectParameter("Budget", budget) :
                new ObjectParameter("Budget", typeof(decimal));
    
            var descriptionParameter = description != null ?
                new ObjectParameter("Description", description) :
                new ObjectParameter("Description", typeof(string));
    
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("CreatedBy", createdBy) :
                new ObjectParameter("CreatedBy", typeof(int));
    
            var modifiedByParameter = modifiedBy.HasValue ?
                new ObjectParameter("ModifiedBy", modifiedBy) :
                new ObjectParameter("ModifiedBy", typeof(int));
    
            var useridlistParameter = useridlist != null ?
                new ObjectParameter("useridlist", useridlist) :
                new ObjectParameter("useridlist", typeof(string));
    
            var customeridlistParameter = customeridlist != null ?
                new ObjectParameter("customeridlist", customeridlist) :
                new ObjectParameter("customeridlist", typeof(string));
    
            var filenamelistParameter = filenamelist != null ?
                new ObjectParameter("filenamelist", filenamelist) :
                new ObjectParameter("filenamelist", typeof(string));
    
            var foldernameParameter = foldername != null ?
                new ObjectParameter("foldername", foldername) :
                new ObjectParameter("foldername", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CreateOrder", orderNameParameter, statusParameter, budgetParameter, descriptionParameter, createdByParameter, modifiedByParameter, useridlistParameter, customeridlistParameter, filenamelistParameter, foldernameParameter, addedOrderId);
        }
    
        public virtual int sp_CreateUser(string name, string email, string loginName, string password, Nullable<int> status, string phone, Nullable<long> createdby, Nullable<long> modifiedby, Nullable<int> roleid, Nullable<System.Guid> acivecode, Nullable<System.DateTime> dateOfBirth)
        {
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var loginNameParameter = loginName != null ?
                new ObjectParameter("loginName", loginName) :
                new ObjectParameter("loginName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("status", status) :
                new ObjectParameter("status", typeof(int));
    
            var phoneParameter = phone != null ?
                new ObjectParameter("phone", phone) :
                new ObjectParameter("phone", typeof(string));
    
            var createdbyParameter = createdby.HasValue ?
                new ObjectParameter("createdby", createdby) :
                new ObjectParameter("createdby", typeof(long));
    
            var modifiedbyParameter = modifiedby.HasValue ?
                new ObjectParameter("modifiedby", modifiedby) :
                new ObjectParameter("modifiedby", typeof(long));
    
            var roleidParameter = roleid.HasValue ?
                new ObjectParameter("roleid", roleid) :
                new ObjectParameter("roleid", typeof(int));
    
            var acivecodeParameter = acivecode.HasValue ?
                new ObjectParameter("acivecode", acivecode) :
                new ObjectParameter("acivecode", typeof(System.Guid));
    
            var dateOfBirthParameter = dateOfBirth.HasValue ?
                new ObjectParameter("dateOfBirth", dateOfBirth) :
                new ObjectParameter("dateOfBirth", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_CreateUser", nameParameter, emailParameter, loginNameParameter, passwordParameter, statusParameter, phoneParameter, createdbyParameter, modifiedbyParameter, roleidParameter, acivecodeParameter, dateOfBirthParameter);
        }
    
        public virtual ObjectResult<sp_CustomerReportByDays_Result> sp_CustomerReportByDays(Nullable<int> createdBy, Nullable<int> numberOfDays)
        {
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            var numberOfDaysParameter = numberOfDays.HasValue ?
                new ObjectParameter("numberOfDays", numberOfDays) :
                new ObjectParameter("numberOfDays", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_CustomerReportByDays_Result>("sp_CustomerReportByDays", createdByParameter, numberOfDaysParameter);
        }
    
        public virtual ObjectResult<sp_CustomerReportDashboard_Result> sp_CustomerReportDashboard(string responsibleUsers)
        {
            var responsibleUsersParameter = responsibleUsers != null ?
                new ObjectParameter("responsibleUsers", responsibleUsers) :
                new ObjectParameter("responsibleUsers", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_CustomerReportDashboard_Result>("sp_CustomerReportDashboard", responsibleUsersParameter);
        }
    
        public virtual int sp_DeleteUsers(string ids, ObjectParameter denyStr)
        {
            var idsParameter = ids != null ?
                new ObjectParameter("Ids", ids) :
                new ObjectParameter("Ids", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_DeleteUsers", idsParameter, denyStr);
        }
    
        public virtual int sp_DeleteViews(string idArray, ObjectParameter nameArray)
        {
            var idArrayParameter = idArray != null ?
                new ObjectParameter("IdArray", idArray) :
                new ObjectParameter("IdArray", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_DeleteViews", idArrayParameter, nameArray);
        }
    
        public virtual ObjectResult<vwSystmAudit> sp_FilterSystemAudit(Nullable<int> moduleId, Nullable<int> recordId, Nullable<System.DateTime> dateModified, string operation, string columnName, Nullable<int> createdBy, Nullable<int> pageIndex, Nullable<int> pageSize, ObjectParameter totalRow)
        {
            var moduleIdParameter = moduleId.HasValue ?
                new ObjectParameter("moduleId", moduleId) :
                new ObjectParameter("moduleId", typeof(int));
    
            var recordIdParameter = recordId.HasValue ?
                new ObjectParameter("recordId", recordId) :
                new ObjectParameter("recordId", typeof(int));
    
            var dateModifiedParameter = dateModified.HasValue ?
                new ObjectParameter("dateModified", dateModified) :
                new ObjectParameter("dateModified", typeof(System.DateTime));
    
            var operationParameter = operation != null ?
                new ObjectParameter("operation", operation) :
                new ObjectParameter("operation", typeof(string));
    
            var columnNameParameter = columnName != null ?
                new ObjectParameter("columnName", columnName) :
                new ObjectParameter("columnName", typeof(string));
    
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<vwSystmAudit>("sp_FilterSystemAudit", moduleIdParameter, recordIdParameter, dateModifiedParameter, operationParameter, columnNameParameter, createdByParameter, pageIndexParameter, pageSizeParameter, totalRow);
        }
    
        public virtual ObjectResult<vwSystmAudit> sp_FilterSystemAudit(Nullable<int> moduleId, Nullable<int> recordId, Nullable<System.DateTime> dateModified, string operation, string columnName, Nullable<int> createdBy, Nullable<int> pageIndex, Nullable<int> pageSize, ObjectParameter totalRow, MergeOption mergeOption)
        {
            var moduleIdParameter = moduleId.HasValue ?
                new ObjectParameter("moduleId", moduleId) :
                new ObjectParameter("moduleId", typeof(int));
    
            var recordIdParameter = recordId.HasValue ?
                new ObjectParameter("recordId", recordId) :
                new ObjectParameter("recordId", typeof(int));
    
            var dateModifiedParameter = dateModified.HasValue ?
                new ObjectParameter("dateModified", dateModified) :
                new ObjectParameter("dateModified", typeof(System.DateTime));
    
            var operationParameter = operation != null ?
                new ObjectParameter("operation", operation) :
                new ObjectParameter("operation", typeof(string));
    
            var columnNameParameter = columnName != null ?
                new ObjectParameter("columnName", columnName) :
                new ObjectParameter("columnName", typeof(string));
    
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<vwSystmAudit>("sp_FilterSystemAudit", mergeOption, moduleIdParameter, recordIdParameter, dateModifiedParameter, operationParameter, columnNameParameter, createdByParameter, pageIndexParameter, pageSizeParameter, totalRow);
        }
    
        public virtual ObjectResult<sp_GetCustomersByUserId_Result> sp_GetCustomersByUserId(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetCustomersByUserId_Result>("sp_GetCustomersByUserId", userIdParameter);
        }
    
        public virtual ObjectResult<sp_GetModulePermisstion_Result> sp_GetModulePermisstion(Nullable<int> roleId)
        {
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetModulePermisstion_Result>("sp_GetModulePermisstion", roleIdParameter);
        }
    
        public virtual ObjectResult<vwModule> sp_GetModulesByRole(Nullable<int> roleId)
        {
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<vwModule>("sp_GetModulesByRole", roleIdParameter);
        }
    
        public virtual ObjectResult<vwModule> sp_GetModulesByRole(Nullable<int> roleId, MergeOption mergeOption)
        {
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<vwModule>("sp_GetModulesByRole", mergeOption, roleIdParameter);
        }
    
        public virtual ObjectResult<sp_GetRoles_Result> sp_GetRoles(Nullable<int> currentRoleId)
        {
            var currentRoleIdParameter = currentRoleId.HasValue ?
                new ObjectParameter("currentRoleId", currentRoleId) :
                new ObjectParameter("currentRoleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetRoles_Result>("sp_GetRoles", currentRoleIdParameter);
        }
    
        public virtual ObjectResult<sp_GetRolesFieldsByRoleId_Result> sp_GetRolesFieldsByRoleId(Nullable<int> roleId)
        {
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetRolesFieldsByRoleId_Result>("sp_GetRolesFieldsByRoleId", roleIdParameter);
        }
    
        public virtual int sp_GetSubView(Nullable<int> moduleId, Nullable<int> viewId, Nullable<int> id, Nullable<int> userId, Nullable<int> roleId, Nullable<int> pageIndex, Nullable<int> pageSize, string sortDirection, ObjectParameter totalRow)
        {
            var moduleIdParameter = moduleId.HasValue ?
                new ObjectParameter("ModuleId", moduleId) :
                new ObjectParameter("ModuleId", typeof(int));
    
            var viewIdParameter = viewId.HasValue ?
                new ObjectParameter("ViewId", viewId) :
                new ObjectParameter("ViewId", typeof(int));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortDirectionParameter = sortDirection != null ?
                new ObjectParameter("sortDirection", sortDirection) :
                new ObjectParameter("sortDirection", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_GetSubView", moduleIdParameter, viewIdParameter, idParameter, userIdParameter, roleIdParameter, pageIndexParameter, pageSizeParameter, sortDirectionParameter, totalRow);
        }
    
        public virtual int sp_GetTotalForView(Nullable<int> viewId, Nullable<int> roleId, Nullable<int> userId, ObjectParameter @return)
        {
            var viewIdParameter = viewId.HasValue ?
                new ObjectParameter("viewId", viewId) :
                new ObjectParameter("viewId", typeof(int));
    
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_GetTotalForView", viewIdParameter, roleIdParameter, userIdParameter, @return);
        }
    
        public virtual int sp_GetView(Nullable<int> moduleId, Nullable<int> viewId, Nullable<int> id, Nullable<int> userId, Nullable<int> roleId, Nullable<int> pageIndex, Nullable<int> pageSize, string sortDirection, ObjectParameter totalRow, Nullable<bool> defaultOderBy, string columnGroup, ObjectParameter groupJsonStr)
        {
            var moduleIdParameter = moduleId.HasValue ?
                new ObjectParameter("ModuleId", moduleId) :
                new ObjectParameter("ModuleId", typeof(int));
    
            var viewIdParameter = viewId.HasValue ?
                new ObjectParameter("ViewId", viewId) :
                new ObjectParameter("ViewId", typeof(int));
    
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortDirectionParameter = sortDirection != null ?
                new ObjectParameter("sortDirection", sortDirection) :
                new ObjectParameter("sortDirection", typeof(string));
    
            var defaultOderByParameter = defaultOderBy.HasValue ?
                new ObjectParameter("defaultOderBy", defaultOderBy) :
                new ObjectParameter("defaultOderBy", typeof(bool));
    
            var columnGroupParameter = columnGroup != null ?
                new ObjectParameter("columnGroup", columnGroup) :
                new ObjectParameter("columnGroup", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_GetView", moduleIdParameter, viewIdParameter, idParameter, userIdParameter, roleIdParameter, pageIndexParameter, pageSizeParameter, sortDirectionParameter, totalRow, defaultOderByParameter, columnGroupParameter, groupJsonStr);
        }
    
        public virtual int sp_GetViewCustom(Nullable<int> viewId, Nullable<int> userId, Nullable<int> roleId, Nullable<int> pageIndex, Nullable<int> pageSize, string sortExpression, string filterStr, ObjectParameter totalRow, string columnGroup, ObjectParameter groupJsonStr)
        {
            var viewIdParameter = viewId.HasValue ?
                new ObjectParameter("ViewId", viewId) :
                new ObjectParameter("ViewId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpressionParameter = sortExpression != null ?
                new ObjectParameter("sortExpression", sortExpression) :
                new ObjectParameter("sortExpression", typeof(string));
    
            var filterStrParameter = filterStr != null ?
                new ObjectParameter("filterStr", filterStr) :
                new ObjectParameter("filterStr", typeof(string));
    
            var columnGroupParameter = columnGroup != null ?
                new ObjectParameter("columnGroup", columnGroup) :
                new ObjectParameter("columnGroup", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_GetViewCustom", viewIdParameter, userIdParameter, roleIdParameter, pageIndexParameter, pageSizeParameter, sortExpressionParameter, filterStrParameter, totalRow, columnGroupParameter, groupJsonStr);
        }
    
        public virtual ObjectResult<sp_InvoiceReportByDays_Result> sp_InvoiceReportByDays(Nullable<int> createdBy, Nullable<int> numberOfDays)
        {
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            var numberOfDaysParameter = numberOfDays.HasValue ?
                new ObjectParameter("numberOfDays", numberOfDays) :
                new ObjectParameter("numberOfDays", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_InvoiceReportByDays_Result>("sp_InvoiceReportByDays", createdByParameter, numberOfDaysParameter);
        }
    
        public virtual ObjectResult<sp_InvoiceReportDashboard_Result> sp_InvoiceReportDashboard(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> status, string users, Nullable<bool> byClient, Nullable<int> currencyId)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(int));
    
            var usersParameter = users != null ?
                new ObjectParameter("users", users) :
                new ObjectParameter("users", typeof(string));
    
            var byClientParameter = byClient.HasValue ?
                new ObjectParameter("byClient", byClient) :
                new ObjectParameter("byClient", typeof(bool));
    
            var currencyIdParameter = currencyId.HasValue ?
                new ObjectParameter("currencyId", currencyId) :
                new ObjectParameter("currencyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_InvoiceReportDashboard_Result>("sp_InvoiceReportDashboard", fromDateParameter, toDateParameter, statusParameter, usersParameter, byClientParameter, currencyIdParameter);
        }
    
        public virtual ObjectResult<sp_InvoiceReportPerDays_Result> sp_InvoiceReportPerDays(Nullable<int> createdBy, Nullable<int> numberOfDays, Nullable<int> currencyId)
        {
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            var numberOfDaysParameter = numberOfDays.HasValue ?
                new ObjectParameter("numberOfDays", numberOfDays) :
                new ObjectParameter("numberOfDays", typeof(int));
    
            var currencyIdParameter = currencyId.HasValue ?
                new ObjectParameter("currencyId", currencyId) :
                new ObjectParameter("currencyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_InvoiceReportPerDays_Result>("sp_InvoiceReportPerDays", createdByParameter, numberOfDaysParameter, currencyIdParameter);
        }
    
        public virtual ObjectResult<sp_OrderReportByDays_Result> sp_OrderReportByDays(Nullable<int> createdBy, Nullable<int> numberOfDays)
        {
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            var numberOfDaysParameter = numberOfDays.HasValue ?
                new ObjectParameter("numberOfDays", numberOfDays) :
                new ObjectParameter("numberOfDays", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_OrderReportByDays_Result>("sp_OrderReportByDays", createdByParameter, numberOfDaysParameter);
        }
    
        public virtual ObjectResult<sp_OrderReportDashboard_Result> sp_OrderReportDashboard(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> status, string users)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(int));
    
            var usersParameter = users != null ?
                new ObjectParameter("users", users) :
                new ObjectParameter("users", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_OrderReportDashboard_Result>("sp_OrderReportDashboard", fromDateParameter, toDateParameter, statusParameter, usersParameter);
        }
    
        [EdmFunction("LeonardUSAEntities", "fn_GetRolesHierachy")]
        public virtual IQueryable<Nullable<int>> fn_GetRolesHierachy(Nullable<int> roleId)
        {
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("roleId", roleId) :
                new ObjectParameter("roleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Nullable<int>>("[LeonardUSAEntities].[fn_GetRolesHierachy](@roleId)", roleIdParameter);
        }
    
        public virtual ObjectResult<sp_GetAllOverdueApps_Result> sp_GetAllOverdueApps(string id, string status, string customerName, string partNumber, string capitalizationPeriod, Nullable<System.DateTime> modifiedDate, Nullable<int> pageIndex, Nullable<int> pageSize, string sortExpression, Nullable<int> expectedMonth, ObjectParameter totalRow)
        {
            var idParameter = id != null ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(string));
    
            var statusParameter = status != null ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(string));
    
            var customerNameParameter = customerName != null ?
                new ObjectParameter("CustomerName", customerName) :
                new ObjectParameter("CustomerName", typeof(string));
    
            var partNumberParameter = partNumber != null ?
                new ObjectParameter("PartNumber", partNumber) :
                new ObjectParameter("PartNumber", typeof(string));
    
            var capitalizationPeriodParameter = capitalizationPeriod != null ?
                new ObjectParameter("CapitalizationPeriod", capitalizationPeriod) :
                new ObjectParameter("CapitalizationPeriod", typeof(string));
    
            var modifiedDateParameter = modifiedDate.HasValue ?
                new ObjectParameter("ModifiedDate", modifiedDate) :
                new ObjectParameter("ModifiedDate", typeof(System.DateTime));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("pageIndex", pageIndex) :
                new ObjectParameter("pageIndex", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("pageSize", pageSize) :
                new ObjectParameter("pageSize", typeof(int));
    
            var sortExpressionParameter = sortExpression != null ?
                new ObjectParameter("sortExpression", sortExpression) :
                new ObjectParameter("sortExpression", typeof(string));
    
            var expectedMonthParameter = expectedMonth.HasValue ?
                new ObjectParameter("expectedMonth", expectedMonth) :
                new ObjectParameter("expectedMonth", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetAllOverdueApps_Result>("sp_GetAllOverdueApps", idParameter, statusParameter, customerNameParameter, partNumberParameter, capitalizationPeriodParameter, modifiedDateParameter, pageIndexParameter, pageSizeParameter, sortExpressionParameter, expectedMonthParameter, totalRow);
        }
    }
}
