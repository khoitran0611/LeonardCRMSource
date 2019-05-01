(function() {

    'use strict';

    angular.module("LeonardCRM").controller("ManageModuleCtrl", ctrl);

    ctrl.$inject = ["$scope", "$compile", "appService", "toolbarService", "windowService", "_"];

    function ctrl($scope, $compile, appService, toolbarService, windowService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var updateSortOrderCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var setOrder = function() {
            angular.forEach($scope.moduleDataSource, function(item, index) {
                item.SortOrder = index + 1;
            });
        };

        var getModule = function() {
            appService.getModules().then(function(data) {
                $scope.moduleDataSource = angular.fromJson(data);
            });
        };

        var init = function() {
            getModule();

            $scope.setWindowTitle($scope.languages.MODULES.TITLE);
            toolbarService.NeedSaveCommand(true);
            toolbarService.NeedCanCelCommand(false);
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.moduleDataSource = [];
        $scope.myid = 1;


        // --- Define Scope Method. ---------------------- //

        $scope.sortGrid = function(columnName) {
            $scope.reverse = !$scope.reverse;
            $scope.moduleDataSource = _.sortBy($scope.moduleDataSource, columnName);
            if ($scope.reverse) {
                $scope.predicate = columnName + ' Asc';
            } else {
                $scope.predicate = columnName + ' Desc';
                $scope.moduleDataSource.reverse();
            }
        };

        $scope.updateOrder = function() {

        };

        $scope.editRow = function(item) {
            var param = {
                Url: '/appviews/dashboard/modules/editmodule.html',
                Id: item.Id,
                ParentId: 0,
                ViewId: 0,
                ModuleId: $scope.CurrentModule,
                Key: 'module' + item.Id.toString()
            };
            windowService.openWindow(param);
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('saveEvent', function(event) {
            setOrder();
            appService.updateSortOrder($scope.moduleDataSource)
                .then(updateSortOrderCallback);
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            toolbarService.NeedSaveCommand(true);
            toolbarService.NeedCanCelCommand(false);
            getModule();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function() {

            }
        );
        
        $scope.$on('$destroy', function () {
            toolbarService.NeedCanCelCommand(true);
        });

        // --- Initialize. ---------------------------------- //
        init();
    }

})();