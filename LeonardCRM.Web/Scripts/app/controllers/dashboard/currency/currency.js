(function() {

    "use strict";

    angular.module("LeonardCRM").controller("CurrencyCtrl", ctrl);

    ctrl.$inject = ['$scope', 'requestContext'];

    function ctrl($scope, requestContext) {

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("dashboard.currency");

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();

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
    
})();