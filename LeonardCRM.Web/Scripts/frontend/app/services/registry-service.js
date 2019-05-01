(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("registryService", ser);

    ser.$inject = ["serviceHelper"];

    // a repository for the app.
    function ser() {
        var siteSettings = {};

        function getSettings() {  
            var settings = localStorage.getItem('settings');
            if (settings) {
                siteSettings = ng.fromJson(settings);
            }
            else
                window.location.href = '/login';
            return siteSettings;
        }

        // Return the public API.
        return {
            siteSettings: getSettings()
        };
    }

})(angular);