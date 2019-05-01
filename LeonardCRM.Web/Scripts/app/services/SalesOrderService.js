(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("salesOrderService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/SalesOrderApi/';
        return {
            AddNewOrder: function (order, moduleId) {
                return serviceHelper.post(url + 'CreateOrder/?moduleId=' + moduleId, order);
            },
            GetSalesOrdersById: function (id) {
                return serviceHelper.get(url + 'GetSalesOrderById' + id);
            },
            GetAllSalesOrder: function (pagedInfo) {
                return serviceHelper.post(url + 'GetAllSalesOrder', pagedInfo);
            },
            DeleteSalesOrder: function (selectedSalesOrder) {
                return serviceHelper.post(url + 'DeleteSalesOrder', selectedSalesOrder);
            },
            LoadData: function (param) {
                return serviceHelper.post(url + 'GetSalesOrderById', param);
            },
            getRecentlyAddedOrders: function (onlyMe) {
                return serviceHelper.get(url + 'GetRecentlyAddedOrders/?onlyMe=' + onlyMe);
            },
            getChartData: function (onlyMe) {
                return serviceHelper.get(url + 'GetChartData/?onlyMe=' + onlyMe);
            },
            getOrderReportDashboard: function (reportObject) {
                return serviceHelper.post(url + 'GetOrderReportDashboard', reportObject);
            },
            getOwnApplication: function (assistMode) {
                return serviceHelper.get(url + 'GetOwnedApplication?assistMode=' + (assistMode ? assistMode : false));
            },
            cancelApplication: function (appId) {
                return serviceHelper.get(url + 'CancelApplication?appId=' + appId);
            },
            saveSignature: function (app, mode) {
            	return serviceHelper.post(url + 'SaveSignature' + (angular.isDefined(mode) && mode != null ? ('?t=' + mode) : ""), app);
            },
            saveDeliverySignature: function (app) {
                return serviceHelper.post(url + 'SaveDeliverySignature', app);
            },
            getPDFByForm: function (formType, appId, isGetExistFilePath, isFinal)
            {
            	return serviceHelper.get(url + 'GetPDFByForm?formType=' + formType + "&appId=" + appId + (angular.isDefined(isGetExistFilePath) ? ("&isGetExistFilePath=" + isGetExistFilePath) : "") + (angular.isDefined(isGetExistFilePath) ? ("&isFinal=" + isFinal) : ""));
            },
            PMT: function (ir, np, pv, fv, type) {
                /*
                 * ir   - interest rate per month
                 * np   - number of periods (months)
                 * pv   - present value
                 * fv   - future value
                 * type - when the payments are due:
                 *        0: end of the period, e.g. end of month (default)
                 *        1: beginning of period
                 */
                var pmt, pvif;

                fv || (fv = 0);
                type || (type = 0);

                if (ir === 0)
                    return -(pv + fv) / np;

                pvif = Math.pow(1 + ir, np);
                pmt = -ir * pv * (pvif + fv) / (pvif - 1);

                if (type === 1)
                    pmt /= (1 + ir);

                return pmt;
            },
            finalizeOrer: function (app) {
                return serviceHelper.post(url + 'FinalizeOrer', app);
            },
            getOverdueApp: function (pageIndex, pageSize, sortDesc, overdueMonth, filterParams) {
            	return serviceHelper.post(url + 'GetOverdueApp?pageIndex=' + pageIndex + "&pageSize=" + pageSize + "&sortDesc=" + sortDesc + "&overdueMonth=" + overdueMonth, filterParams);
            },
            deleteOverdueApps: function (overdueMonth, total) {
            	return serviceHelper.get(url + 'DeleteOverdueApps?overdueMonth=' + overdueMonth + "&total=" + total);
            },
            cloneApp: function (appId) {
                return serviceHelper.get(url + 'CloneApp?appId=' + appId);
            }
        };
    }

})(angular);