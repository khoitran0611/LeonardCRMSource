(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("OrdersCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "salesInvoiceService", "salesOrderService", "toolbarService", "$routeParams", "requestContext"];

    function ctrl($scope, $http, $location, salesInvoiceService, salesOrderService, toolbarService, $routeParams, requestContext) {

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("order");


        // --- Define Scope Variables. ---------------------- //


        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();


        // --- Bind To Scope Events. ------------------------ //


        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
            	$scope.orderid = $routeParams.orderid;
              
                // Make sure this change is relevant to this controller.
            	if (!renderContext.isChangeRelevant()) {
                	$('#dialogDetail').remove();
                    return;
                }

                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();

            }
        );
        // --- Initialize. ---------------------------------- //


    }

})(angular);