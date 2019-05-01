(function () {

    'use strict';

    angular.module("LeonardCRM").controller("SettingsCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "roleService", "registryService", "requestContext", "viewService", "toolbarService", "_"];

    function ctrl($scope, $http, $route, $location, $routeParams, roleService, registryService, requestContext, viewService, toolbarService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var getRegistryCallback = function (data) {
            var hashTableObj = angular.fromJson(data);
            $scope.regs = angular.copy(hashTableObj.reg);
            $scope.langs = angular.copy(hashTableObj.languages);
            $scope.themes = angular.copy(hashTableObj.themes);
            $scope.currencies = angular.copy(hashTableObj.currencies);

            roleService.getRoles().then(function (data) {                
                $scope.roles = data;
            });
        };

        var getRegistry = function () {
            registryService.GetRegistry()
                .then(getRegistryCallback);
        };

        var saveSettings = function () {
            registryService.SaveSettings($scope.regs)
                .then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
        };

        var init = function () {
            toolbarService.NeedSaveCommand(true);
            toolbarService.NeedCanCelCommand(false);
            getRegistry();
            var i = 0;
            //for (i = 1; i <= 20; i++) {
            //    $scope.items_per_page.push(i * 5);
            //}
            $scope.items_per_page = [10,25,50,100];
            for (i = 1; i <= 30; i++) {
                $scope.max_pages.push(i);
            }
            $scope.setWindowTitle($scope.languages.HOST_SETTING.TITLE);
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.regs = {};
        $scope.items_per_page = [];
        $scope.max_pages = [];
        $scope.langs = [];
        $scope.themes = [];
        $scope.currencies = [];
        $scope.roles = [];

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('saveEvent', function (event) {
            saveSettings();
            event.preventDefault();
        });
        $scope.$on('cancelEvent', function (event) {
            if ($scope.previousUrl == '') {
                $location.path('/dashboard');
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

            }
        );

        $scope.$on('$destroy', function () {
            toolbarService.NeedCanCelCommand(true);
        });

        init();
    }

})();