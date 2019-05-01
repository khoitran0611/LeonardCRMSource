(function (ng) {

    'use strict';

    angular.module("LeonardCRM").service("taxService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/TaxApi/';

        var getAll = function () {
            return serviceHelper.get(url + 'GetAll');
        };

        var getTaxByTaxId = function (taxId) {
            return serviceHelper.get(url + 'GetTaxByTaxId/' + taxId);
        };

        var saveTax = function (model, moduleId) {
            return serviceHelper.post(url + 'SaveTax/' + moduleId, model);
        };

        return {
            getAll: getAll,
            getTaxByTaxId: getTaxByTaxId,
            saveTax: saveTax
        };
    }

})(angular);