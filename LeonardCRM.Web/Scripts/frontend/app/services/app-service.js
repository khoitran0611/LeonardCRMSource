(function (ng) {

    "use strict";

    angular.module("LeonardCRMFrontEnd").service("appService", ser);

    ser.$inject = ["$cookieStore", "serviceHelper", "_"];

    // a repository for the app.
    function ser($cookieStore, serviceHelper, _) {
    	var url = '/api/ModuleApi/';
    	var modules = {};

        function getCurrentUser() {
            try {
                return parseInt($cookieStore.get('user'));
            }
            catch (e) {
                return 0;
            }
        }

        var loadModules = function () {
        	return serviceHelper.get(url + 'GetModules');
        };

        var md = localStorage.getItem('modules');
        if (md == null) {
        	loadModules();
        } else {
        	modules = angular.fromJson(md);
        }


        function getDefaultView(moduleName) {
        	try {
        		var module = _.findWithProperty(modules, 'Name', moduleName);
        		if (module && module.DefaultViewId)
        			return module.DefaultViewId;
        		return 0;
        	}
        	catch (e) {
        		return 0;
        	}
        }

        function getModuleId(name) {
            return 0;
        }

        function getPickListByModules(modules, needReferenceList) {
            return serviceHelper.post(url + 'GetPickListByModules/?needRef=' + needReferenceList, modules);
        }

        // Return the public API.
        return {
            getPickListByModules: getPickListByModules,
            getCurrentUser: getCurrentUser,
            getModuleId: getModuleId,
            getDefaultView: getDefaultView
        };
    }	

})(angular);