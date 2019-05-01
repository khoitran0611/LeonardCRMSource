(function() {

    'use strict';

    angular.module("LeonardCRM").controller("InvoiceTemplateCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'salesInvoiceService', 'salesOrderService', 'toolbarService', '$routeParams', 'requestContext'];

    function ctrl($scope, $http, $location, salesInvoiceService, salesOrderService, toolbarService, $routeParams, requestContext) {


        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("dashboard.invoice_templates");


        // --- Define Scope Variables. ---------------------- //


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
                //alert($scope.subview);
            }
        );
        // --- Initialize. ---------------------------------- //
        $scope.test = 'dddd';

        $scope.setWindowTitle("Invoice Template Management");

    }   

})();