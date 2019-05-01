(function (ng) {

    "use strict";

    ng.module("LeonardCRMFrontEnd").factory("feFilterService", ser);

    ser.$inject = ["$rootScope", "$timeout", "appService", "_"];

    function ser($rootScope, $timeout, appService, _) {
        var sharedService = {};

        sharedService.pickLists = [];
        
        sharedService.setPickList = function (pickList, moduleId) {
            var currentPickLists = _.find(sharedService.pickLists, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentPickLists == null) {
                currentPickLists = { key: 'module_' + moduleId, pickLists: [], referenceLists: [] };
                sharedService.pickLists.push(currentPickLists);
            }
            currentPickLists.pickLists = pickList.PickList;
            currentPickLists.referenceLists = pickList.ReferenceList;
        };
        
        sharedService.getPickList = function (moduleId) {
            var currentPickLists = _.find(sharedService.pickLists, function (obj) { return obj.key == ('module_' + moduleId); });
            if (currentPickLists == null)
                return [];
            return currentPickLists.pickLists;
        };
        
        sharedService.getPickListByModules = function (modules, needReferenceList) {
            appService.getPickListByModules(modules, needReferenceList).then(function (data) {
                var picklists = angular.fromJson(data);
                for (var i = 0; i < modules.length; i++) {
                    var pl = {
                        PickList: _.filter(picklists.PickList, function (e) { return e.ModuleId == modules[i]; }),
                        ReferenceList: needReferenceList ? _.filter(picklists.ReferenceList, function (e) { return e.ModuleId == modules[i]; }) : []
                    };
                    sharedService.setPickList(pl, modules[i]);
                }
                $rootScope.$broadcast('filterPickListEvent', modules);
            });
        };
        return sharedService;
    }

})(angular);