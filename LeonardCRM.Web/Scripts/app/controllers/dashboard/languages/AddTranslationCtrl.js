(function () {

    'use strict';

    angular.module("LeonardCRM").controller("AddTranslationCtrl", ctrl);

    ctrl.$inject = ["$scope", "resourceService"];

    function ctrl($scope, resourceService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var saveTranslationCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                $scope.$emit('reloadTranslationData');
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.translationObj = {};

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('dialogTranslationDetail', function (event, args) {
            $scope.translationObj = {
                FileName: args.FileName,
                ResourceName: args.ResourceName
            };
        });

        $scope.$on('dialogTranslationSave', function (event) {
            var msg = '';
            if (!angular.isDefined($scope.translationObj.Key)
                || $scope.translationObj.Key === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Key<br>';
            }
            else if (!resourceService.getRegexNameFormat().test($scope.translationObj.Key)) {
                msg += $scope.languages.EDIT_LANGUAGE.WRONG_FORMAT + 'Key<br>';
            }

            if (!angular.isDefined($scope.translationObj.Translation)
                || $scope.translationObj.Translation === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Translation<br>';
            }

            if (!angular.isDefined($scope.translationObj.TranslationDefault)
                || $scope.translationObj.TranslationDefault === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Translation Default<br>';
            }

            if (msg.length > 0) {
                NotifyError(msg);
            } else {
                $scope.translationObj.Key = $scope.translationObj.Key.toUpperCase();

                resourceService.createTranslation($scope.translationObj).then(saveTranslationCallback);
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