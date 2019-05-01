(function (ng) {

    'use strict';

    ng.module("LeonardCRM").directive("ngCustomvalidateinput", dir);

    dir.$inject = [];

    function dir() {

        // --- Define directive Variables. ---------------------- //

        // --- Define directive Method. ---------------------- //

        var controller = ['$scope', function ($scope) {
        }];
        var link = function (scope, elem, attrs, ctrl) {

            scope.$watch('model', function (oldValue, newValue) {
                if (scope.model == null || scope.model.length == 0) {
                    ctrl.$setValidity('ngCustomvalidateinput', false);
                } else {
                    var regex = new RegExp(scope.pattern);
                    var result = regex.test(scope.model);
                    ctrl.$setValidity('ngCustomvalidateinput', result);
                }
            });
        };

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ---------------------------------- //
        return {
            restrict: 'A',
            require: 'ngModel',
            scope: {
                model: '=ngModel',
                pattern: '=ngCustomvalidateinput'
            },
            controller: controller,
            link: link
        };
    }

})(angular);