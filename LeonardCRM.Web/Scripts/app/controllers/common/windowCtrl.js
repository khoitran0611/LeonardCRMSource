(function () {

    "use strict";

    angular.module("LeonardCRM").controller("windowCtrl", ctrl);

    ctrl.$inject = ["$scope"];


    function ctrl($scope) {

        // --- Define Controller Methods. ------------------- //

        var closeWindow = function() {
            if (angular.isDefined($scope.CurrentParam)
                && angular.isDefined($scope.CurrentParam.isPristine)
                && !$scope.CurrentParam.isPristine()) {
                var canContinue = confirm($scope.languages.COMMON.FORM_CHANGED_MSG);
                if (!canContinue) {
                    return;
                }
            }

            $scope.$emit('closeWindowEvent', $scope.CurrentParam.Key);
            $scope.CurrentParam = {
                Url: '',
                Id: 0,
                ParentId: 0,
                ViewId: 0,
                ModuleId: 0,
                Key: ''
            };

        };

        var closeAndOpenWindow = function (args) {
            $scope.CurrentParam.Id = args;
            $scope.$emit('CloseAndOpenWindowEvent', $scope.CurrentParam);
            $scope.CurrentParam = {
                Url: '',
                Id: 0,
                ParentId: 0,
                ViewId: 0,
                ModuleId: 0,
                Key: ''
            };
        };

        var init = function () {
            
        };

        // --- Define Scope Methods. ------------------------ //

        $scope.saveData = function () {
            if (angular.isDefined($scope.CurrentParam)
                && angular.isDefined($scope.CurrentParam.isPristine)
                && !$scope.CurrentParam.isPristine()) {
                $scope.CurrentParam.resetPristine();
            }

            $scope.$broadcast('saveDataEvent');
        };

        $scope.closeWindow = function () {
            closeWindow();
        };

        $scope.pageOnLoad = function () {
            
        };

        $scope.setTitle = function (titleStr) {
            $scope.CurrentParam.Title = titleStr;
        };
        $scope.setSaveButtonVisible = function(isShow) {
            $scope.hasEditPermissions = isShow;
        };
        // --- Define Controller Variables. ----------------- //

        // --- Define Scope Variables. ---------------------- //

        $scope.CurrentKey = '';
        $scope.CurrentParam = {
            Id: 0,
            Url: '',
            ParentId: 0,
            ViewId: 0,
            ModuleId:0,
            Key: ''
        };
        $scope.hasEditPermissions = true;

        $scope.loadBodyCompleted = function () {
            $scope.$broadcast('loadBodyCompletedEvent', $scope.CurrentParam);
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('init', function (event, args) {
            if ($scope.CurrentKey == '') {
                $scope.CurrentKey = angular.copy(args.Key);
                $scope.CurrentParam = angular.copy(args);
            }
        });

        $scope.$on('closeWindow', function (event) {
            closeWindow();           
        });
        
        $scope.$on('CloseAndOpenWindow', function (eventa,args) {
            closeAndOpenWindow(args);
        });

        // --- Initialize. ---------------------------------- //
        init();
    }

})();