(function () {

    'use strict';

    angular.module("LeonardCRM").controller("ModuleCtrl", ctrl);

    ctrl.$inject = ["$scope"];

    function ctrl($scope) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                
            }
        );
        
        // --- Initialize. ---------------------------------- //
    }

})();