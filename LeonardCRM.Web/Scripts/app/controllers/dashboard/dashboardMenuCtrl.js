(function (ng) {
    'use strict';

    angular.module("LeonardCRM").controller("dashboardMenuCtrl", ctrl);

    ctrl.$inject = ["$scope", "requestContext", "appService"];

    function ctrl($scope, requestContext, appService) {

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("dashboard");

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();

        $scope.dashboardModules = appService.getModulesByParent($scope.ModuleId);

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {
                    return;
                }

                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();
            }
        );
    }
})(angular);