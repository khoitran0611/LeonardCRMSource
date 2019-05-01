(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("WebformCtrl", ctrl);

    ctrl.$inject = ["$scope", "requestContext"];

    function ctrl($scope, requestContext) {

        // --- Define Controller Variables. ---------------------- //

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext('dashboard.webform');

        // --- Define Controller Method. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        $scope.subview = renderContext.getNextSection();

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

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

        // --- Initialize. ---------------------------------- //
    }

})(angular);