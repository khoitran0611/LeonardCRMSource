(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("salesOrderCompleteService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/SalesCompleteApi/';
        return {
            saveCustomerSignature: function (saleComplete, appId) {
                return serviceHelper.post(url + 'SaveCustomerSignature/?appId=' + appId, saleComplete);
            },
            saveSaleComplete: function (saleComplete, appId) {
                return serviceHelper.post(url + 'SaveSaleComplete/?appId=' + appId, saleComplete);
            }
        }
    };   

})(angular);