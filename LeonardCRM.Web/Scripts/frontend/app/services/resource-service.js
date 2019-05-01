(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("resourceService", ser);

    ser.$inject = ["$rootScope", "serviceHelper"];

    // a resource repository for the app.
    function ser($rootScope, serviceHelper) {
        var url = '/api/LocalizeApi/';
        var resourceStrings = {};

        function loadResources() {
            serviceHelper.get(url + 'GetResources').then(function (data) {
                resourceStrings = angular.fromJson(data);
                localStorage.setItem('resourceStrings', angular.toJson(data));

                    serviceHelper.get('/api/RegistryApi/GetRegistry').then(function (data2) {
                        localStorage.removeItem('settings');
                        localStorage.setItem('settings', JSON.stringify(data2));
                        $rootScope.$broadcast('loadingReourcesCompleted');
                    });
            });
        };

        function initResources() {
            var data = localStorage.getItem('resourceStrings');
            if (data == null) {
                loadResources();
            } else {
                resourceStrings = angular.fromJson(data);
                $rootScope.$broadcast('loadingReourcesCompleted');
            }
        };

        initResources();

        function getResources() {
            if (resourceStrings == null) {
                var data = localStorage.getItem('resourceStrings');
                resourceStrings = angular.fromJson(data);
            }
            return resourceStrings;
        }

        // Return the public API.
        return {
            loadResources: getResources
        };
    }
	

})(angular);