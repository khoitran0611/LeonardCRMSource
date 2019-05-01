(function( ng ) {
	
    "use strict";

    angular.module("LeonardCRM").factory("mailTemplateService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/MailTemplateApi/';
        return {
            GetMailTemplateById: function (Id) {
                return serviceHelper.get(url + 'GetMailTemplateById/?Id=' + Id);
            },
            Save: function (mailtemplate, moduleId) {
                return serviceHelper.post(url + 'Save/?moduleId=' + moduleId, mailtemplate);
            },
            LoadData: function () {
                return serviceHelper.get(url + 'GetAllMailTemplate');
            }
        };
    }

})(angular);