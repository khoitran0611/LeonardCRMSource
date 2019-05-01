(function () {

    'use strict';

    angular.module("LeonardCRM").controller("EditTaxCtrl", ctrl);

    ctrl.$inject = ["$scope", "taxService"];

    function ctrl($scope, taxService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var getTaxByTaxIdCallBack = function (data) {
            $scope.taxObj = angular.fromJson(data);
            $scope.taxObjShadow = angular.copy($scope.taxObj);
        };

        var saveTaxCallback = function (data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                if ($scope.taxObj.Id == 0) {
                    $scope.taxObj = angular.copy($scope.taxObjShadow);
                } else {
                    $scope.$emit('reloadData');
                }

            } else {
                NotifyError(data.Result, 5000);
            }
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.taxObj = {};
        $scope.taxObjShadow = {};

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('dialogDetail', function (event, args) {
            taxService.getTaxByTaxId(args).then(getTaxByTaxIdCallBack);
        });

        $scope.$on('dialogSave', function (event) {
            var msg = '';
            if (isNaN($scope.taxObj.TaxValue)) {
                msg += $scope.languages.TAX.TAX_VALUE_INVALID + '<br>';
            }
            if (msg.length > 0) {
                NotifyError(msg);
            } else {
                taxService.saveTax($scope.taxObj, $scope.CurrentModule).then(saveTaxCallback);
            }
                
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

            }
        );

        // --- Initialize. ---------------------------------- //
    }

})();