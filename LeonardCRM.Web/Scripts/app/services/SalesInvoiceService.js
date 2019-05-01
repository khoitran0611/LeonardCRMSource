(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("salesInvoiceService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/SalesInvoiceApi/';

        this.GetInvoiceByOrderId = function (params) {
            return serviceHelper.post(url + 'GetInvoiceByOrderId', params);
        };

        this.AddSalesInvoice = function (params, moduleId) {
            return serviceHelper.post(url + 'AddSalesInvoice/?moduleId=' + moduleId, params);
        };

        this.DeleteSalesInvoice = function (selectedInvoices) {
            return serviceHelper.post(url + 'DeleteSalesInvoice', selectedInvoices);
        };

        this.LoadData = function (param) {
            return serviceHelper.post(url + 'GetSalesInvoicesById', param);
        };

        this.ExportPDF = function (param) {
            return serviceHelper.post(url + 'ExportToPdf', param);
        };

        this.getRecentlyAddedInvoices = function (onlyMe) {
            return serviceHelper.get(url + 'GetRecentlyAddedInvoices/?onlyMe=' + onlyMe);
        }
        ;
        this.getChartData = function (onlyMe) {
            return serviceHelper.get(url + 'GetChartData/?onlyMe=' + onlyMe);
        };
        this.getChartInvoicePaidData = function (onlyMe, currencyId) {
            return serviceHelper.get(url + 'GetChartInvoicePaidData/?onlyMe=' + onlyMe + '&currencyId=' + currencyId);
        };

        this.getInvoiceReportDashboard = function (reportObject) {
            return serviceHelper.post(url + 'GetInvoiceReportDashboard', reportObject);
        };
    }
})(angular);