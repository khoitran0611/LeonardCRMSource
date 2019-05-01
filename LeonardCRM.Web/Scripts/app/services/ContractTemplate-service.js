(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("ContractTemplateService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/ContractTemplateApi/';
        return {
            CreateTemplate: function (contractTemplate, moduleId) {
                return serviceHelper.post(url + 'SaveContractTemplate/?moduleId=' + moduleId, contractTemplate);
            },
            DeleteContractTemplate: function (selectedTemplate) {
                return serviceHelper.post(url + 'DeleteContractTemplate', selectedTemplate);
            },
            LoadData: function(templateId) {
                return serviceHelper.get(url + 'GetContractTemplateById/?templateId=' + templateId);
            }
        };
    }

})(angular);