(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("orderAttachmentService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/SalesDocumentApi/';
        return {
            GetAttachmentsByItemId: function (applicantId) {
                return serviceHelper.get(url + "GetAttachmentsByItemId?appId=" + applicantId);
            },
            SaveAttachment: function (applicantId, attachments, isOnlyAdd) {
                return serviceHelper.post(url + "SaveAttachment?appId=" + applicantId + "&isOnlyAdd=" + (isOnlyAdd ? isOnlyAdd : "false"), attachments);
            },

            uploadBase64PNG: function (base64Data) {
            	return serviceHelper.post(url + "UploadBase64PNG", { "data": base64Data });
            }
        };
    }

})(angular);