(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("entityFieldService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/EntityFieldsApi/';
        var customData = [];

        function getCustomData() {
            return customData;
        }

        function setCustomData(data) {
            customData = data;
        }

        function getEntityFieldByModuleId(moduleId) {
            return serviceHelper.get(url + 'GetEntityFieldByModuleId/' + moduleId);
        }

        function getManageFieldsByModuleId(moduleId) {
            return serviceHelper.get(url + 'GetManageFieldsByModuleId/' + moduleId);
        }

        function updateSortOrder(fields) {
            return serviceHelper.post(url + 'UpdateSortOrder', fields);
        }

        function getEntityFieldById(fieldId) {
            return serviceHelper.get(url + 'GetEntityFieldById/' + fieldId);
        }

        function deleteEntityField(entityField) {
            return serviceHelper.post(url + 'DeleteEntityField', entityField);
        }

        function saveEntityField(entityField) {
            return serviceHelper.post(url + 'SaveEntityField', entityField);
        }

        function getFieldListType(id) {
            return serviceHelper.get(url + 'GetFieldListType/' + id);
        }

        var getAllCustFieldByModule = function (moduleId) {
            return serviceHelper.get(url + 'GetAllCustFieldByModule/' + moduleId);
        };

        return {
            GetEntityFieldByModuleId: getEntityFieldByModuleId,
            getManageFieldsByModuleId: getManageFieldsByModuleId,
            updateSortOrder: updateSortOrder,
            getEntityFieldById: getEntityFieldById,
            deleteEntityField: deleteEntityField,
            saveEntityField: saveEntityField,
            getCustomData: getCustomData,
            setCustomData: setCustomData,
            getFieldListType: getFieldListType,
            getAllCustFieldByModule: getAllCustFieldByModule
        };
    }

})(angular);