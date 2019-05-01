(function (ng) {

    'use strict';

    angular.module("LeonardCRM").service("windowService", ser);

    ser.$inject = ["$rootScope"];

    function ser($rootScope) {

        // --- Define Controller Variables. ----------------- //

        var windowFactory = {};
        // --- Define Controller Methods. ------------------- //

        windowFactory.openWindow = function (param) {
            $rootScope.$broadcast('openWindow', param);
        };

        windowFactory.closeWindow = function (count) {
            // Xu lý Advance Search khi tat mo nhieu window
            $rootScope.$broadcast('windowClosed', count);
        };
        windowFactory.closeCurrentWindow = function () {
            $rootScope.$broadcast('currentWindowClosed');
        };
        windowFactory.refresh = function (key) {
            $rootScope.$broadcast('refreshDataEvent', key);
        };
        // --- Define Scope Methods. ------------------------ //

        // --- Define Scope Variables. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ---------------------------------- //
        return windowFactory;
    }

})(angular);