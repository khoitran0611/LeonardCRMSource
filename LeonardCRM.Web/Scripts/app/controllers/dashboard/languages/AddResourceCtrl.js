(function () {

    'use strict';

    angular.module("LeonardCRM").controller("AddResourceCtrl", ctrl);

    ctrl.$inject = ["$scope", "resourceService"];

    function ctrl($scope, resourceService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var saveResourceCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                $scope.$emit('reloadResourceData', data);
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.resourceObj = {};

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('dialogResourceDetail', function (event, args) {
            $scope.resourceObj = {};
        });

        $scope.$on('dialogResourceSave', function (event) {
            var msg = '';

            if (!angular.isDefined($scope.resourceObj.Name)
                || $scope.resourceObj.Name === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Name<br>';
            }

            else if (!resourceService.getRegexNameFormat().test($scope.resourceObj.Name)) {
                msg += $scope.languages.EDIT_LANGUAGE.WRONG_FORMAT + 'Name<br>';
            }

            if (msg.length > 0) {
                NotifyError(msg);
            } else {
                $scope.resourceObj.Name = $scope.resourceObj.Name.toUpperCase();

                resourceService.createResource($scope.resourceObj).then(saveResourceCallback);
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