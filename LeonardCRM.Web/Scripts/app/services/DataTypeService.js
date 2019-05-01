(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("dataTypeService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/DataTypeApi/';
        function getAllDataType() {
            return serviceHelper.get(url + 'GetAllDataType');
        }
        return {
            getAllDataType: getAllDataType
        };
    }

})(angular);