(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("modulesRelationshipService", ser);

    ser.$inject = ["$http", "$q", "serviceHelper"];

    function ser($http, $q, serviceHelper) {
        var url = '/api/ModulesRelationshipApi/';

        function getModules() {
            return serviceHelper.get(url + 'GetModules');
        }

        var getRelationshipByModules = function (masterModuleId, childModuleId) {
            return serviceHelper.get(url + 'GetRelationshipByModules/?masterModuleId=' + masterModuleId + '&childModuleId=' + childModuleId);
        };

        var createRelationship = function (masterModuleId, entities) {
            return serviceHelper.post(url + 'CreateRelationship/?masterModuleId=' + masterModuleId, entities);
        };

        var deleteRelationship = function (id) {
            return serviceHelper.post(url + 'DeleteRelationship', id);
        };

        return {
            getModules: getModules,
            getRelationshipByModules: getRelationshipByModules,
            createRelationship: createRelationship,
            deleteRelationship: deleteRelationship
        };
    }

})(angular);