(function () {

    'use strict';

    angular.module("LeonardCRM").controller("SourceValuesCtrl", ctrl);

    ctrl.$inject = ['$scope', 'picklistDependencyService'];
    
    function ctrl($scope, picklistDependencyService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var initForm = function () {
            var items = picklistDependencyService.getSourceValues();
            $scope.SourceValues = angular.copy(items);
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.SourceValues = [];

        // --- Define Scope Method. ---------------------- //

        $scope.CheckAll = function() {
            angular.forEach($scope.SourceValues, function (item, index) {
                item.Selected = true;
            });
        };

        $scope.UncheckAll = function () {
            angular.forEach($scope.SourceValues, function (item, index) {
                item.Selected = false;
            });
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('SetSourceValuesEvent', function() {
            var items = picklistDependencyService.getSourceValues();
            $scope.SourceValues = angular.copy(items);
        });
        $scope.$on('SaveSelectedSourceValueEvent', function () {
            picklistDependencyService.setSourceValues($scope.SourceValues);
        });

        // --- Initialize. ------------------------ //
        initForm();

    }

})();