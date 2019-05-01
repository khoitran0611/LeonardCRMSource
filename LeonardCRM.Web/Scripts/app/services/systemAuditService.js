(function (ng) {

    'use strict';

    angular.module("LeonardCRM").service("systemAuditService", ser);

    ser.$inject = ['serviceHelper'];

    function ser(serviceHelper) {
        var url = '/api/SystemAuditApi/';

        var getAuditByRecordId = function (pageInfo) {
            return serviceHelper.post(url + 'GetByModuleIdnRecordId', pageInfo);
        };

        var serverFilter = function (condition, moduleId, recordId, pageIndex) {
            return serviceHelper.post(url + 'ServerFilter/?moduleId=' + moduleId + '&recordId=' + recordId + '&pageIndex=' + pageIndex, condition);
        };
        return {
            getAuditByRecordId: getAuditByRecordId,
            serverFilter: serverFilter
        };
    }

})(angular);