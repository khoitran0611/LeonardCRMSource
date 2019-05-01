(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("resourceService", ser);

    ser.$inject = ["$http", "$rootScope", "$q", "serviceHelper"];

    // a resource repository for the app.
    function ser($http, $rootScope, $q, serviceHelper) {
        var url = '/api/LocalizeApi/';
        var resourceStrings = {};

        function loadResources() {
            serviceHelper.get(url + 'GetResources').then(function (data) {
                resourceStrings = angular.fromJson(data);
                localStorage.setItem('resourceStrings', angular.toJson(data));
                serviceHelper.get('/api/ModuleApi/GetModules').then(function (data1) {
                    localStorage.setItem('modules', angular.toJson(data1));

                    serviceHelper.get('/api/RegistryApi/GetRegistry').then(function (data2) {
                        localStorage.removeItem('settings');
                        localStorage.setItem('settings', JSON.stringify(data2));
                        $rootScope.$broadcast('loadingReourcesCompleted');
                    });
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

        function getRegexNameFormat() {
            return /^(\w)+$/;
        };

        function getCultureCodeRegex() {
            return /^[a-z]{2,3}(?:-[A-Z]{2,3}(?:-[a-zA-Z]{4})?)?$/;
        };

        function getResources() {
            if (resourceStrings == null) {
                var data = localStorage.getItem('resourceStrings');
                resourceStrings = angular.fromJson(data);
            }
            return resourceStrings;
        }

        function getListLanguages() {
            return serviceHelper.get(url + 'GetListLanguages');
        }

        function getDefaultLanguages() {
            return serviceHelper.get(url + 'GetDefaultLanguages');
        }

        function languageChanged(fileName) {
            return serviceHelper.get(url + 'LanguageChanged/?fileName=' + fileName);
        }

        function pageChanged(fileName, pageName) {
            return serviceHelper.get(url + 'PageChanged/?fileName=' + fileName + '&pageName=' + pageName);
        }

        function createLanguage(languageObj) {
            return serviceHelper.post(url + 'CreateLanguage', languageObj);
        }

        function createResource(resourceObj) {
            return serviceHelper.post(url + 'CreateResource', resourceObj);
        }

        function createTranslation(translationObj) {
            return serviceHelper.post(url + 'CreateTranslation', translationObj);
        }

        function saveLanguage(fileName, pageName, languages) {
            return serviceHelper.post(url + 'SaveLanguage/?fileName=' + fileName + '&pageName=' + pageName, languages);
        }

        function refreshCache() {
            return serviceHelper.get(url + 'RefreshCache');
        }

        // Return the public API.
        return {
            loadResources: getResources,
            getDefaultLanguages: getDefaultLanguages,
            getListLanguages: getListLanguages,
            languageChanged: languageChanged,
            pageChanged: pageChanged,
            createLanguage: createLanguage,
            createResource: createResource,
            createTranslation: createTranslation,
            saveLanguage: saveLanguage,
            getRegexNameFormat: getRegexNameFormat,
            getCultureCodeRegex: getCultureCodeRegex,
            refreshCache: refreshCache
        };
    }
	

})(angular);