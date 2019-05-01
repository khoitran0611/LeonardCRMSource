(function () {

    "use strict";

    angular.module("LeonardCRM").controller("reportCtrl", ctrl);

    ctrl.$inject = ["$scope", "roleService", "requestContext", "$routeParams"];

    function ctrl($scope, roleService, requestContext, $routeParams) {

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = $routeParams.module;

        $scope.gridLines = 10;
        $scope.chartType = "AreaChart";
        // I handle changes to the request context.
        $scope.setWindowTitle($scope.languages.REPORT.REPORT_DASHBOARD_TITLE);
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
