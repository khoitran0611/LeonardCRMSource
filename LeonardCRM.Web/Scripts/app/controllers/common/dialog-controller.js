(function () {

    "use strict";

    angular.module("LeonardCRM").factory("dialogService", srv);

    srv.$inject = ["$rootScope"];

    function srv($rootScope) {
        var dialogService = {};
        dialogService.hideAllButtons = function (isShow) {
            $rootScope.$broadcast('setAllButtonsEvent', isShow);
        };
        dialogService.hideSaveButton = function (isShow) {
            $rootScope.$broadcast('setSaveButtonsEvent', isShow);
        };
        return dialogService;
    }
})();

(function () {

    "use strict";

    angular.module("LeonardCRM").controller("dialogCtrl", ctrl);

    ctrl.$inject = ["$scope", "appService"];

    function ctrl($scope, appService) {
        var moduleId = $scope.CurrentModule;

        if (angular.isDefined(moduleId) && !moduleId) {
            moduleId = $scope.ModuleId;
            $scope.hasPermission = appService.hasPermission(moduleId);
        }
        else {
            $scope.hasPermission = true;
        }

        $scope.NeedFooterCommand = true;
        $scope.NeedSaveCommand = true;
        $scope.$on('setAllButtonsEvent', function (event, isShow) {
            $scope.NeedFooterCommand = isShow;
        });
        $scope.$on('setSaveButtonsEvent', function (event, isShow) {
            $scope.NeedSaveCommand = isShow;
        });
    }

})();
