(function() {

    'use strict';

    angular.module("LeonardCRM").controller("ManageEntityFieldCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "$compile", "$timeout", "roleService", "appService", "entityFieldService", "requestContext", "viewService", "toolbarService", "windowService", "_"];

    function ctrl($scope, $http, $route, $location, $routeParams, $compile, $timeout, roleService, appService, entityFieldService, requestContext, viewService, toolbarService, windowService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var setOrder = function() {
            angular.forEach($scope.entityFieldSources, function(item, index) {
                item.SortOrder = index + 1;
            });
        };

        var updateSortOrderCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var getManageFieldsByModuleIdCallback = function(data) {
            $scope.entityFieldSources = angular.fromJson(data);
        };

        var loadFieldByModulue = function() {
            entityFieldService.getManageFieldsByModuleId($scope.fieldModuleId)
                .then(getManageFieldsByModuleIdCallback);
        };

        var getModuleForEntityFieldsCallback = function(data) {
            $scope.moduleSources = angular.fromJson(data);
            if ($scope.moduleSources.length > 0) {
                $scope.fieldModuleId = $scope.moduleSources[0].Id;
                loadFieldByModulue();
                toolbarService.ShowAdvanceSearch(false);
            }
        };

        var init = function () {
            appService.getModuleForEntityFields().
                then(getModuleForEntityFieldsCallback);

            $scope.setWindowTitle($scope.languages.ENTITY_FIELD.TITLE);
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.moduleSources = [];
        $scope.fieldModuleId = 0;
        $scope.entityFieldSources = [];
        $scope.reverse = true;
        $scope.predicate = 'SortOrder Asc';
        $scope.sortableOptions1 = {
            placeholder: 'field',
            connectWith: '.fields-container',
            update: function(e, ui) {

            }
        };

        $scope.fieldSelected = {};
        $scope.fieldId = 0;

        //$scope.detailFormUrl = '/appviews/dashboard/entityfields/editentityfield.html';
        //$scope.FormTitle = $scope.languages.ENTITY_FIELD.EDIT_HEADING;

        // --- Define Scope Method. ---------------------- //

        $scope.ModuleChanged = function() {
            loadFieldByModulue();
        };

        $scope.updateOrder = function() {
            setOrder();
            entityFieldService.updateSortOrder($scope.entityFieldSources)
                .then(updateSortOrderCallback);
        };

        $scope.sortGrid = function(columnName) {
            $scope.reverse = !$scope.reverse;
            $scope.entityFieldSources = _.sortBy($scope.entityFieldSources, columnName);
            if ($scope.reverse) {
                $scope.predicate = columnName + ' Asc';
            } else {
                $scope.predicate = columnName + ' Desc';
                $scope.entityFieldSources.reverse();
            }
        };

        $scope.editRow = function(item) {
            var param = {
                Url: '/appviews/dashboard/entityfields/editentityfield.html',
                Id: item.Id,
                ParentId: 0,
                ViewId: 0,
                ModuleId: $scope.fieldModuleId,
                Key: 'entityfield' + item.Id.toString()
            };
            windowService.openWindow(param);
        };

        $scope.deleteRow = function(item) {
            $scope.fieldSelected = item;
            $scope.SetConfirmMsg($scope.languages.ENTITY_FIELD.CONFIRM_DELETE_MSG,'manageentity');
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('yesEvent', function (event, args) {
            if (args == 'manageentity') {
                entityFieldService.deleteEntityField($scope.fieldSelected).then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        $scope.fieldSelected = {};
                        loadFieldByModulue();
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
            }
        });
        $scope.$on('noEvent', function(event) {
            event.preventDefault();
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            loadFieldByModulue();
        });

        $scope.$on('refreshEvent', function(event) {
            loadFieldByModulue();
            event.preventDefault();
        });

        $scope.$on('addEvent', function (event) {
            var param = {
                Url: '/appviews/dashboard/entityfields/editentityfield.html',
                Id: 0,
                ParentId: 0,
                ViewId: 0,
                ModuleId: $scope.fieldModuleId,
                Key: 'entityfield0'
            };
            windowService.openWindow(param);
        });



        // --- Initialize. ---------------------------------- //
        init();
    }

})();