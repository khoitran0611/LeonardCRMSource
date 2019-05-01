(function () {

    'use strict';

    angular.module("LeonardCRM").controller("PicklistDependencyCtrl", ctrl);

    ctrl.$inject = ['$scope', 'requestContext'];

    function ctrl($scope, requestContext) {

        // --- Define Controller Variables. ---------------------- //

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext('dashboard.picklist_dependency');

        // --- Define Controller Method. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        $scope.subview = renderContext.getNextSection();

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                $('#dialogDetail').remove();
                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {

                    return;

                }

                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();

            }
        );

    }

})();