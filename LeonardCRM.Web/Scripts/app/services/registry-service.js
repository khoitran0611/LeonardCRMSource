(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("registryService", ser);

    ser.$inject = ["$http", "$rootScope", "$q", "serviceHelper"];

    // a repository for the app.
    function ser($http, $rootScope, $q, serviceHelper) {
        var url = '/api/RegistryApi/';
        var siteSettings = {};

        function loadSettings() {
            serviceHelper.get(url + 'GetRegistry').then(function (data) {
                siteSettings = data;
                localStorage.removeItem('settings');
                localStorage.setItem('settings', JSON.stringify(data));
            });
        }

        function getSettings() {  
            var settings = localStorage.getItem('settings');      
            if (settings) {
                siteSettings = ng.fromJson(settings);
            } else
                loadSettings();
            return siteSettings;
        }

        function getRegistry() {
            return serviceHelper.get(url + 'GetSettings');
        }

        function saveSettings(regs) {
            var promise = serviceHelper.post(url + 'SaveSettings', regs);
            promise.then(function (data) {      
                localStorage.removeItem('settings');
                localStorage.setItem('settings', JSON.stringify(regs));
            });
            return promise;
        }

        //loadSettings();
        getSettings();

        // Return the public API.
        return {
            siteSettings: getSettings(),
            GetRegistry: getRegistry,
            SaveSettings: saveSettings
        };
    }

})(angular);