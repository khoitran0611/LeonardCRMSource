(function (ng) {

    "use strict";

    angular.module("LeonardCRM").factory("filterService", ser);

    ser.$inject = ["$rootScope", "$timeout", "entityFieldService", "appService", "picklistDependencyService", "_"];

    function ser($rootScope, $timeout, entityFieldService, appService, picklistDependencyService, _) {
        var sharedService = {};

        sharedService.fields = [];
        sharedService.pickLists = [];
        sharedService.pickListDependency = [];
        //sharedService.referenceLists = [];
        sharedService.Conditions = [];
        sharedService.currentGridColumns = [];

        sharedService.doFilter = function (viewId) {
            sharedService.CollectConditions();// Get Conditions from GridView
            $rootScope.$broadcast('doFilterEvent', sharedService.getAllConditions(viewId));
        };
        sharedService.setFields = function (fields, needBroadcast, moduleId) {
            var currentFields = _.find(sharedService.fields, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentFields == null) {
                currentFields = { 'key': 'module_' + moduleId, 'fields': fields };
                sharedService.fields.push(currentFields);
            }
            if (needBroadcast)
                $rootScope.$broadcast('entityFieldsLoaded');
        };
        sharedService.setPickList = function (pickList, moduleId) {
            var currentPickLists = _.find(sharedService.pickLists, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentPickLists == null) {
                currentPickLists = { key: 'module_' + moduleId, pickLists: [], referenceLists: [] };
                sharedService.pickLists.push(currentPickLists);
            }
            currentPickLists.pickLists = pickList.PickList;
            currentPickLists.referenceLists = pickList.ReferenceList;
            //    $timeout(function () {
            //$rootScope.$broadcast('filterPickListEvent', pickList);
            //}, 3000);
        };
        sharedService.setAndConditions = function (andConditions, viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null) {
                currentViewConditions = { key: 'view_' + viewId, andConditions: [], orConditions: [], filterConditions: [], alphabetConditions: [] };
                sharedService.Conditions.push(currentViewConditions);
            }
            currentViewConditions.andConditions = andConditions;
        };
        sharedService.setOrConditions = function (orConditions, viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null) {
                currentViewConditions = { key: 'view_' + viewId, andConditions: [], orConditions: [], filterConditions: [], alphabetConditions: [] };
                sharedService.Conditions.push(currentViewConditions);
            }
            currentViewConditions.orConditions = orConditions;
        };
        sharedService.setFilterCondition = function (filterConditions, viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null) {
                currentViewConditions = { key: 'view_' + viewId, andConditions: [], orConditions: [], filterConditions: [], alphabetConditions: [] };
                sharedService.Conditions.push(currentViewConditions);
            }
            currentViewConditions.filterConditions = filterConditions;
        };
        sharedService.setAlphabetCondition = function (alphabetConditions, viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null) {
                currentViewConditions = { key: 'view_' + viewId, andConditions: [], orConditions: [], filterConditions: [], alphabetConditions: [] };
                sharedService.Conditions.push(currentViewConditions);
            }
            currentViewConditions.alphabetConditions = alphabetConditions;
        };
        sharedService.getFields = function (moduleId) {
            var currentFields = _.find(sharedService.fields, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentFields == null)
                return [];
            return currentFields.fields;
        };
        sharedService.getPickList = function (moduleId) {
            var currentPickLists = _.find(sharedService.pickLists, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentPickLists == null)
                return [];
            return currentPickLists.pickLists;
        };
        sharedService.getReferenceList = function (moduleId) {
            var currentPickLists = _.find(sharedService.pickLists, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentPickLists == null)
                return [];
            return currentPickLists.referenceLists;
        };
        sharedService.getAndConditions = function (viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null)
                return [];
            return currentViewConditions.andConditions;
        };
        sharedService.getOrConditions = function (viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null)
                return [];
            return currentViewConditions.orConditions;
        };
        sharedService.getFilterConditions = function (viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null)
                return [];
            return currentViewConditions.filterConditions;
        };
        sharedService.loadFilterColumns = function (moduleId) {
            //sharedService.fields = [];
            entityFieldService.GetEntityFieldByModuleId(moduleId).then(function (data) {
                //--transfer entity fields to advanced search form through shared filter service ------------------
                sharedService.setFields(angular.fromJson(data), true, moduleId);
            }, function (data) {
                NotifyError(data.ExceptionMessage.toString());
            });
        };
        sharedService.preLoadFilterColumnsAndPickLists = function (moduleId) {
            picklistDependencyService.getPicklistDependenciesByModuleId(moduleId)
                .then(function (data) {
                    var pickList = angular.fromJson(data);
                    sharedService.setPicklistDependency(moduleId, pickList);
                });
            // sharedService.fields = [];
            entityFieldService.GetEntityFieldByModuleId(moduleId).then(function (data) {
                //--transfer entity fields to advanced search form through shared filter service ------------------
                sharedService.setFields(angular.fromJson(data), false, moduleId);
                appService.getPickListByModule(moduleId).then(function (dt) {
                    var picklists = angular.fromJson(dt);
                    sharedService.setPickList(picklists, moduleId);
                    $rootScope.$broadcast('entityFieldsLoaded');
                    $rootScope.$broadcast('filterPickListEvent', moduleId);
                }, function (dt) {
                    NotifyError(dt.ExceptionMessage.toString());
                });
            }, function (data) {
                NotifyError(data.ExceptionMessage.toString());
            });
        };
        sharedService.getPickListByModule = function (moduleId) {
            picklistDependencyService.getPicklistDependenciesByModuleId(moduleId)
                .then(function (data) {
                    var pickList = angular.fromJson(data);
                    sharedService.setPicklistDependency(moduleId, pickList);
                });
            appService.getPickListByModule(moduleId).then(function (data) {
                var picklists = angular.fromJson(data);
                sharedService.setPickList(picklists, moduleId);
                $rootScope.$broadcast('filterPickListEvent', moduleId);
            });
        };
        sharedService.getAllConditions = function (viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            if (currentViewConditions == null)
                return [];
            return currentViewConditions.andConditions.concat(currentViewConditions.orConditions).concat(currentViewConditions.filterConditions).concat(currentViewConditions.alphabetConditions);
        };
        sharedService.resetAllConditions = function (viewId) {
            var currentViewConditions = _.find(sharedService.Conditions, function (obj) { return obj.key == ('view_' + viewId); });
            sharedService.Conditions.splice(sharedService.Conditions.indexOf(currentViewConditions), 1);
        };
        sharedService.CollectConditions = function () {
            $rootScope.$broadcast('collectConditionsEvent');
        };
        sharedService.resetAllFields = function () {
            sharedService.fields = [];
        };
        sharedService.setCurrentGridColumns = function (items) {
            sharedService.currentGridColumns = angular.copy(items);
            $rootScope.$broadcast('setCurrentGridColumnsEvent');
        };
        sharedService.getCurrentGridColumns = function () {
            return sharedService.currentGridColumns;
        };
        sharedService.setPicklistDependency = function (moduleId, pickList) {
            var result = _.find(sharedService.pickListDependency, function (obj) { return obj.key == ('module_' + moduleId); });
            if (result == null) {
                result = { 'key': 'module_' + moduleId, 'picklist': pickList };
                sharedService.pickListDependency.push(result);
            }
        };
        sharedService.getPicklistDependency = function (moduleId) {
            var result = _.find(sharedService.pickListDependency, function (obj) { return obj.key == ('module_' + moduleId); });
            if (result == null)
                return [];
            return result.picklist;
        };
        sharedService.resetAllPicklistDependencies = function () {
            sharedService.pickListDependency = [];
        };
        return sharedService;
    }

})(angular);