(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("InvTemplateService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/InvoiceTemplateApi/';
        return {
            CreateInvTemplate: function (invtemplate, moduleId) {
                return serviceHelper.post(url + 'SaveInvTemplate/?moduleId=' + moduleId, invtemplate);
            },
            DeleteInvTemplate: function (selectedTemplate) {
                return serviceHelper.post(url + 'DeleteInvTemplate', selectedTemplate);
            },
            LoadData: function(invTemplateId) {
                return serviceHelper.get(url + 'GetInvTemplateById/?invTemplateId=' + invTemplateId);
            }
        };
    }

})(angular);