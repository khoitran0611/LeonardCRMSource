(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("picklistDependencyService", ser);

    ser.$inject = ["$rootScope", "serviceHelper"];

    function ser($rootScope, serviceHelper) {

        // --- Define Controller Variables. ---------------------- //

        var apiUrl = '/api/PicklistDependencyApi/';
        var sourceValues = [];

        // --- Define Controller Method. ---------------------- //

        var getObjById = function (id) {
            return serviceHelper.get(apiUrl + 'GetObjById/' + id);
        };

        var saveObject = function (item, moduleId) {
            return serviceHelper.post(apiUrl + 'SaveObject/' + moduleId, item);
        };

        var deleteObjects = function (items) {
            return serviceHelper.post(apiUrl + 'DeleteObjects/', items);
        };

        var setSourceValues = function (items) {
            sourceValues = items;
            $rootScope.$broadcast('SetSourceValuesEvent');
        };

        var getSourceValues = function () {
            return sourceValues;
        };

        var saveSelectedSourceValue = function () {
            $rootScope.$broadcast('SaveSelectedSourceValueEvent');
        };

        var getPicklistDependenciesByModuleId = function (moduleId) {
            return serviceHelper.get(apiUrl + 'GetPicklistDependenciesByModuleId/' + moduleId);
        };

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //
        return {
            getObjById: getObjById,
            setSourceValues: setSourceValues,
            getSourceValues: getSourceValues,
            saveSelectedSourceValue: saveSelectedSourceValue,
            saveObject: saveObject,
            deleteObjects: deleteObjects,
            getPicklistDependenciesByModuleId: getPicklistDependenciesByModuleId
        };
    }

})(angular);