(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("webformService", ser);

    ser.$inject = ["$http", "serviceHelper"];

    function ser($http, serviceHelper) {

        // --- Define Controller Variables. ----------------- //

        var apiUrl = '/api/WebformApi/';

        // --- Define Controller Methods. ------------------- //

        var getAllWebform = function () {
            return serviceHelper.get(apiUrl + 'GetAllWebforms');
        };

        var getObjectById = function (objectId) {
            return serviceHelper.get(apiUrl + 'GetWebformById/' + objectId);
        };

        var saveObject = function (obj) {
            return serviceHelper.post(apiUrl + 'SaveWebform', obj);
        };

        var deleteObject = function (obj) {
            return serviceHelper.post(apiUrl + 'Delete', obj);
        };

        // --- Define Scope Methods. ------------------------ //

        // --- Define Scope Variables. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ---------------------------------- //

        return {
            getAllWebform: getAllWebform,
            getObjectById: getObjectById,
            saveObject: saveObject,
            deleteObject: deleteObject
        };
    }

})(angular);