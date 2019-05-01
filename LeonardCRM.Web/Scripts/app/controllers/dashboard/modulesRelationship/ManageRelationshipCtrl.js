(function () {

    'use strict';

    angular.module("LeonardCRM").controller("ManageRelationshipCtrl", ctrl);

    ctrl.$inject = ["$scope", "$window", "appService", "entityFieldService", "toolbarService", "modulesRelationshipService", "_"];

    function ctrl($scope, $window, appService, entityFieldService, toolbarService, modulesRelationshipService, _) {

        // --- Define Controller Variables. ---------------------- //

        var currentModuleId = 27;

        // --- Define Controller Method. ---------------------- //

        var loadAllRelationshipByModuleCallback = function (data) {
            if (!angular.isUndefined(data)
                && data !== null
                && data.length > 0) {
                $scope.modulesRelationship = data;
            } else {
                $scope.modulesRelationship = null;
            }
        }

        var loadAllRelationshipByModule = function(masterModuleId, childModuleId) {
            if (!angular.isUndefined(masterModuleId)
                && masterModuleId !== null
                && !angular.isUndefined(childModuleId)
                && childModuleId !== null) {

                modulesRelationshipService.getRelationshipByModules(masterModuleId, childModuleId)
                    .then(loadAllRelationshipByModuleCallback);
            }
        }

        var getFieldsByMasterModuleIdCallback = function (data) {
            $scope.entityFieldsMasterModule = angular.fromJson(data);

            loadAllRelationshipByModule($scope.masterModuleId, $scope.childModuleId);
        };

        var loadFieldsByMasterModule = function () {
            $scope.selectMasterRow = {};

            entityFieldService.getManageFieldsByModuleId($scope.masterModuleId)
                .then(getFieldsByMasterModuleIdCallback);
        };

        var getFieldsByChildModuleIdCallback = function (data) {
            $scope.entityFieldsChildModule = angular.fromJson(data);

            loadAllRelationshipByModule($scope.masterModuleId, $scope.childModuleId);
        };

        var loadFieldsByChildModule = function () {
            $scope.selectChildRow = {};

            entityFieldService.getManageFieldsByModuleId($scope.childModuleId)
                .then(getFieldsByChildModuleIdCallback);
        };

        var getModulesCallback = function (data) {
            $scope.masterModules = angular.fromJson(data);
            $scope.childModules = angular.fromJson(data);

            if ($scope.masterModules.length > 0) {
                $scope.masterModuleId = $scope.masterModules[0].Id;
                loadFieldsByMasterModule();
            }

            if ($scope.childModules.length > 0) {
                $scope.childModuleId = $scope.childModules[0].Id;
                loadFieldsByChildModule();
            }
        };

        var init = function () {
            modulesRelationshipService.getModules().
                then(getModulesCallback);
            
            $scope.setWindowTitle($scope.languages.ENTITY_FIELD.TITLE);

            toolbarService.ShowAdvanceSearch(false);

            appService.hasPermission(currentModuleId).then(function (data) {
                $scope.hasPermission = data;
            });
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.selectMasterRow = {};
        $scope.selectDisplayName = {};
        $scope.selectChildRow = {};
        
        $scope.deletedId = -1;

        // --- Define Scope Method. ---------------------- //

        $scope.CanNotCreateRelationship = function() {
            return _.isEmpty($scope.selectMasterRow) || _.isEmpty($scope.selectChildRow) || _.isEmpty($scope.selectDisplayName);
        }

        $scope.MasterModuleChanged = function () {
            loadFieldsByMasterModule();
        };

        $scope.ChildModuleChanged = function () {
            loadFieldsByChildModule();
        };

        $scope.CreateRelationship = function () {
            if (!angular.isUndefined($scope.selectMasterRow.entity)
                && $scope.selectMasterRow.entity !== null
                && !angular.isUndefined($scope.selectChildRow.entity)
                && $scope.selectChildRow.entity !== null
                && !angular.isUndefined($scope.selectDisplayName.entity)
                && $scope.selectDisplayName.entity !== null) {
                modulesRelationshipService.createRelationship($scope.masterModuleId,
                [$scope.selectMasterRow.entity, $scope.selectChildRow.entity, $scope.selectDisplayName.entity]).then(function(data) {
                    if (data.ReturnCode == 200) {

                        loadAllRelationshipByModule($scope.masterModuleId, $scope.childModuleId);

                        NotifySuccess(data.Result, 5000);
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
            }
        };

        $scope.DeleteRelationship = function (id) {
            $scope.deletedId = id;
            $scope.SetConfirmMsg($scope.languages.RELATIONSHIP.DELETE_MSG, 'deleterelationship');
        }

        $scope.$on('yesEvent', function (event, args) {
            if (args == 'deleterelationship') {
                if (!angular.isUndefined($scope.deletedId) && $scope.deletedId !== null) {
                    modulesRelationshipService.deleteRelationship($scope.deletedId).then(function (data) {
                        if (data.ReturnCode == 200) {

                            loadAllRelationshipByModule($scope.masterModuleId, $scope.childModuleId);

                            NotifySuccess(data.Result, 5000);
                        } else {
                            NotifyError(data.Result, 5000);
                        }
                    });
                }
            }
        });

        $scope.isEntityMasterChecked = function (id) {
            if (!angular.isUndefined($scope.selectMasterRow.entity) && $scope.selectMasterRow.entity !== null) {
                return id === $scope.selectMasterRow.entity.Id;
            }
            return false;
        };

        $scope.isEntityChildChecked = function (id) {
            if (!angular.isUndefined($scope.selectChildRow.entity) && $scope.selectChildRow.entity !== null) {
                return id === $scope.selectChildRow.entity.Id;
            }
            return false;
        };

        $scope.isDisplayNameChecked = function (id) {
            if (!angular.isUndefined($scope.selectDisplayName.entity) && $scope.selectDisplayName.entity !== null) {
                return id === $scope.selectDisplayName.entity.Id;
            }
            return false;
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('refreshEvent', function (event) {
            modulesRelationshipService.getModules().
                 then(getModulesCallback);
            event.preventDefault();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

            }
        );

        // --- Initialize. ---------------------------------- //
        init();
    }

})();