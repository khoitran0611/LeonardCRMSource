(function () {

    'use strict';

    angular.module("LeonardCRM").controller("CreateLanguageCtrl", ctrl);

    ctrl.$inject = ["$scope", "resourceService"];

    function ctrl($scope, resourceService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var saveLanguageCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                $scope.$emit('reloadLanguageData', data);
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.languageObj = {};

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('dialogLanguageDetail', function (event, args) {
            $scope.languageObj = {};
        });

        $scope.$on('dialogLanguageSave', function (event) {
            var msg = '';
            if (!angular.isDefined($scope.languageObj.LanguageName)
                || $scope.languageObj.LanguageName === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Language Name<br>';
            }
            else if (!resourceService.getRegexNameFormat().test($scope.languageObj.LanguageName)) {
                msg += $scope.languages.EDIT_LANGUAGE.WRONG_FORMAT + 'Language Name<br>';
            }

            if (!angular.isDefined($scope.languageObj.LanguageCode)
                || $scope.languageObj.LanguageCode === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Language Code<br>';
            }
            else if (!resourceService.getRegexNameFormat().test($scope.languageObj.LanguageCode)) {
                msg += $scope.languages.EDIT_LANGUAGE.WRONG_FORMAT + 'Language Code<br>';
            }

            if (!angular.isDefined($scope.languageObj.CultureCode)
                || $scope.languageObj.CultureCode === null) {
                msg += $scope.languages.EDIT_LANGUAGE.REQUIRED_INPUT + 'Culture Code<br>';
            }
            else if (!resourceService.getCultureCodeRegex().test($scope.languageObj.CultureCode)) {
                msg += $scope.languages.EDIT_LANGUAGE.WRONG_FORMAT + 'Culture Code<br>';
            }

            if (msg.length > 0) {
                NotifyError(msg);
            } else {
                resourceService.createLanguage($scope.languageObj).then(saveLanguageCallback);
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