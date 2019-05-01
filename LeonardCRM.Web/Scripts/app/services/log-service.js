(function( ng, _ ) {
	
    "use strict";

    angular.module("LeonardCRM").factory("logService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/LogApi/';
        return {
            DeleteLog: function (selectedRecord) {
                return serviceHelper.post(url + 'DeleteLog', selectedRecord);
            },
            GetLogById: function (id) {
                return serviceHelper.get(url + 'GetLogById/?Id=' + id);
            },
            ClearLog: function () {
                return serviceHelper.get(url + 'ClearLog');
            }
        };
    }

})(angular);