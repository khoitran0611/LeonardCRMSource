(function (ng) {

    "use strict";

    angular.module("LeonardCRM").service("appService", ser);

    ser.$inject = ["$http", "_", "$cookieStore", "$q", "serviceHelper"];

    // a repository for the app.
    function ser($http, _, $cookieStore, $q, serviceHelper) {
        var url = '/api/ModuleApi/';
        var roleUrl = '/api/RoleApi/';
        var modules = {};
        var reportModules = {};

        var loadModules = function () {
            return serviceHelper.get(url + 'GetModules');
        };
        var loadReportModules = function () {
            reportModules = _.where(modules, { 'ReportModule': true });
        };

        var md = localStorage.getItem('modules');
        if (md == null) {
            loadModules();
        } else {
            modules = angular.fromJson(md);
        }
        var md1 = localStorage.getItem('reportModules');
        if (md1 == null) {
            loadReportModules();
        } else {
            reportModules = angular.fromJson(md1);
        }
        function getModuleId(moduleName) {
            try {
                var module = _.findWithProperty(modules, 'Name', moduleName);
                if (module)
                    return module.Id;
                return 0;
            }
            catch (e) {
                return 0;
            }
        }

        function getReportModuleId(moduleName) {
            try {
                var module = _.findWithProperty(reportModules, 'Name', moduleName);
                if (module)
                    return module.Id;
                return 0;
            }
            catch (e) {
                return 0;
            }
        }

        function getCurrentUser() {
            try {
                return parseInt($cookieStore.get('user'));
            }
            catch (e) {
                return 0;
            }
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
        function getModuleById(moduleId) {
            try {
                var module = _.findWithProperty(modules, 'Id', moduleId);
                if (module)
                    return module;
                return null;
            }
            catch (e) {
                return null;
            }
        }

        function getViewIdByModule(moduleName) {
            return serviceHelper.get(url + 'GetViewIdByModule/' + moduleName);
        }
        function getModulesByParent(parentId) {
            try {
                return _.sortBy(_.where(modules, { 'Parent': parentId }), ['SortOrder']);
            }
            catch (e) {
                return null;
            }
        }
        function getReportModules() {
            if (reportModules.length == 0)
                loadReportModules();
            return reportModules;
        }


        function getPickListByModule(moduleId) {
            return serviceHelper.get(url + 'GetPickListByModule/' + moduleId);
        }

        function getPickListByCustomViewId(viewId) {
            return serviceHelper.get(url + 'GetPickListByCustomView?viewId=' + viewId);
        }

        function getPickListByModules(arrModules, needReferenceList) {
            return serviceHelper.post(url + 'GetPickListByModules/?needRef=' + needReferenceList, arrModules);
        }

        function getReferenceListsByModule(moduleId) {
            return serviceHelper.get(url + 'GetReferenceListsByModule/' + moduleId);
        }
        function hasPermission(moduleId) {
            return serviceHelper.get(url + 'HasEditPermission/' + moduleId);
        }
        function hasCommandPermission(command, moduleId) {
            return serviceHelper.get('/Home/HasPermisson/?permission=' + command + '&moduleId=' + moduleId);
        }

        function getModuleForEntityFields() {
            return serviceHelper.get(url + 'GetModuleForEntityFields');
        }

        function getModules() {
            return serviceHelper.get(url + 'GetAllModule');
        }

        function updateSortOrder(fields) {
            return serviceHelper.post(url + 'UpdateSortOrder', fields);
        }

        function updateModule(model, moduleId) {
            return serviceHelper.post(url + 'UpdateModule/?moduleId=' + moduleId, model);
        }

        function getModuleByModuleId(id) {
            return serviceHelper.get(url + 'GetModuleByModuleId/' + id);
        }

        function getModuleName(moduleId) {
            try {
                var module = _.findWithProperty(modules, 'Id', moduleId);
                if (module)
                    return module.Name;
                return '';
            }
            catch (e) {
                return '';
            }
        }

        function getFrontEndKey() {
        	return serviceHelper.get(url + 'GetFrontEndKey');
        }

        function getCurrentUserRole() {
            return serviceHelper.get(roleUrl + 'GetCurrentUserRole');
        }

        // Return the public API.
        return {
            getModuleId: getModuleId,
            getDefaultView: getDefaultView,
            getViewIdByModule: getViewIdByModule,
            getPickListByModule: getPickListByModule,
            getPickListByModules: getPickListByModules,
            getReferenceListsByModule: getReferenceListsByModule,
            getCurrentUser: getCurrentUser,
            getModulesByParent: getModulesByParent,
            hasPermission: hasPermission,
            getModuleForEntityFields: getModuleForEntityFields,
            getReportModules: getReportModules,
            getReportModuleId: getReportModuleId,
            getModules: getModules,
            updateSortOrder: updateSortOrder,
            updateModule: updateModule,
            getModuleByModuleId: getModuleByModuleId,
            getModuleById: getModuleById,
            hasCommandPermission: hasCommandPermission,
            getModuleName: getModuleName,
            loadModules: loadModules,
            getPickListByCustomViewId: getPickListByCustomViewId,
            getFrontEndKey: getFrontEndKey,
            getCurrentUserRole : getCurrentUserRole
        };
    }	

})(angular);