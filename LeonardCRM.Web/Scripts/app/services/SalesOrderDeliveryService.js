(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("salesDeliveryService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
    	var url = '/api/SalesDeliveryApi/';
        return {            
        	saveSaleDelivery: function (saleDelivery, mode) {
        		return serviceHelper.post(url + 'SaveDelivery' + (angular.isDefined(mode) && mode != null ? ('?t=' + mode) : ""), saleDelivery);
            }
        }
    };   

})(angular);