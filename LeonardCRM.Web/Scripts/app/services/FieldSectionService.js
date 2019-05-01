(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("fieldSectionService", ser);

    ser.$inject = ["serviceHelper"];

    function ser(serviceHelper) {

        // --- Define Controller Variables. ---------------------- //
        var url = '/api/FieldSectionApi/';
        // --- Define Controller Method. ---------------------- //

        var getFieldSectionByModuleId = function (moduleId) {
            return serviceHelper.get(url + 'GetFieldSectionByModuleId/' + moduleId);
        };

        var saveObjs = function (items, id, moduleId) {
            return serviceHelper.post(url + 'SaveObjs/' + id + '?moduleId=' + moduleId, items);
        };

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ------------------------ //
        return {
            getFieldSectionByModuleId: getFieldSectionByModuleId,
            saveObjs: saveObjs
        };
    }

})(angular);