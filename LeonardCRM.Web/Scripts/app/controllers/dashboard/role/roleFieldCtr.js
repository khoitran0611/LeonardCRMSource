(function () {

    'use strict';

    angular.module("LeonardCRM").controller("roleFieldCtr", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "appService", "roleService", "toolbarService", "requestContext", "_", "entityFieldService"];

    function ctrl($scope, $http, $route, $location, $routeParams, appService, roleService, toolbarService, requestContext, _, entityFieldService) {
        $scope.isFirstItem = true;
        $scope.isCollapsed = false;

        $scope.clickPanel = function() {

        };

        $scope.checkShowPanel = function (item) {
            var lst = _.filterWithProperty(item.EntityFields, 'Display', true);
            if (lst.length > 0)
                return true;
            return false;
        };

        $scope.ChangeBlock = function ($event, field) {
            if (field.Mandatory)
                $event.preventDefault();
            else {
                for (var i = 0; i < $scope.role.Eli_RolesPermissions[$scope.index].EntityFields.length; i++) {
                    if ($scope.role.Eli_RolesPermissions[$scope.index].EntityFields[i].FId == field.FId) {
                        $scope.role.Eli_RolesPermissions[$scope.index].EntityFields[i].Locked = !$scope.role.Eli_RolesPermissions[$scope.index].EntityFields[i].Locked;
                        break;
                    }
                }
            }
        };

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                $scope.roleId = requestContext.getParamAsInt('roleId', 0);
                if (requestContext.haveParamsChanged(['roleId']) && requestContext.getParamAsInt('roleId', 0) > 0) {
                }
            }
        );
    }

})();