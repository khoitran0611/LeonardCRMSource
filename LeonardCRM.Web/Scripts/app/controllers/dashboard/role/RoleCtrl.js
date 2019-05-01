(function () {

    'use strict';

    angular.module("LeonardCRM").controller("RoleCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "roleService", "requestContext", "viewService", "toolbarService", "_"];

    function ctrl($scope, $http, $route, $location, $routeParams, roleService, requestContext, viewService, toolbarService, _) {

        // --- Define Controller Variables. ---------------------- //

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext('dashboard.roles');

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
        
    }

})();