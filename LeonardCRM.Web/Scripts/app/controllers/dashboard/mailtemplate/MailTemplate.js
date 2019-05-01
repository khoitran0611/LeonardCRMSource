(function() {

    'use strict';

    angular.module("LeonardCRM").controller("MailTemplateCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', '$routeParams', 'requestContext'];

    function ctrl($scope, $http, $location, $routeParams, requestContext) {


        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("dashboard.mail_templates");


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

        $scope.setWindowTitle("Mail Template Management");

    }

})();