(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("currencyService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/CurrencyApi/';
        return {
            GetCurrencyById: function (id) {
                return serviceHelper.get(url + 'GetCurrencyById/' + id.toString());
            },
            LoadData: function () {
                return serviceHelper.get(url + 'LoadData');
            },
            GetAllCurrencyName: function () {
                return serviceHelper.get(url + 'GetAllCurrencyName');
            },
            GetAllCurrency: function () {
                return serviceHelper.get(url + 'GetAll');
            },
            Save: function (currency, moduleid) {
                return serviceHelper.post(url + 'Save/?moduleid=' + moduleid, currency);
            }
        };
    }

})(angular);