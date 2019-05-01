(function (ng) {
    
    'use strict';

    angular.module("LeonardCRM").service("previewDataService", ser);

    ser.$inject = ["$rootScope"];


    function ser($rootScope) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var loadData = function () {
            $rootScope.$broadcast('bindingDataEvent', $rootScope.dynamicObjs);
        }

        var bindingData = function(data) {
            $rootScope.dynamicObjs = angular.copy(data);
        };

        // --- Define Scope Variables. ---------------------- //
        $rootScope.isShowPreview = false;
        $rootScope.dynamicObjs = {};

        // --- Define Scope Method. ---------------------- //

        // --- Bind To Scope Events. ------------------------ //

        // --- Initialize. ---------------------------------- //
        return {
            bindingData: bindingData,
            loadData: loadData
        };
    }

})(angular);
