(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("picklistService", ser);
     
    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/ListNamesApi/';

        this.LoadData = function (param) {
            return serviceHelper.post(url + 'GetSalesInvoicesById', param);
        };

        this.GetAllModule = function (moduleId) {
            return serviceHelper.get(url + 'GetAllModule/?moduleId=' + moduleId);
        };

        this.GetListNameById = function (id) {
            return serviceHelper.get(url + 'GetListNameById/?id=' + id);
        };

        this.SaveListName = function (params, moduleId) {
            return serviceHelper.post(url + 'SaveListName/?moduleId=' + moduleId, params);
        };

        this.DeleteListName = function (params) {
            return serviceHelper.post(url + 'DeleteListName', params);
        };

        this.GetListNameByModuleId = function (moduleId) {
            return serviceHelper.get(url + 'GetListNameByModuleId/' + moduleId.toString());
        };
        this.GetListValueByModuleListName = function (moduleId, listName) {
            return serviceHelper.get(url + 'GetListValueByModuleListName/?moduleId=' + moduleId.toString() + '&listName=' + listName);
        };

        this.getListValueByFieldIds = function (masterFieldId, childFieldId) {
            return serviceHelper.get(url + 'GetListValueByFieldIds/?masterFieldId=' + masterFieldId + '&childFieldId=' + childFieldId);
        }
    }

})(angular);