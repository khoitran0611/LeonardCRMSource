(function () {

    "use strict";

    angular.module("LeonardCRM").controller("reportMenuCtrl", ctrl);

    ctrl.$inject = ["$scope", "$routeParams", "requestContext", "appService"];

    function ctrl($scope, $routeParams, requestContext, appService) {
        $scope.subview = $routeParams.module;
        $scope.dashboardModules = appService.getReportModules();

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

                if (requestContext.haveParamsChanged(["module"])) {
                    $scope.subview = $routeParams.module;
                }
            }
        );
    }
})();