(function () {

    'use strict';

    angular.module("LeonardCRM").controller("ManageRoleCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "filterService", "roleService", "viewService", "requestContext", "toolbarService", "_"];

    function ctrl($scope, $http, $route, $location, $routeParams, filterService, roleService, viewService, requestContext, toolbarService, _) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var getRoleCallback = function (data) {
            $scope.pageInfo = angular.fromJson(data);
            $scope.key = $scope.pageInfo.Models.module;
            $scope.roles = angular.copy($scope.pageInfo.Models[$scope.key]);
            $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
        };

        var getRoles = function () {
            viewService.GetView($scope.pageInfo)
                .then(getRoleCallback);
        };

        var showDialog = function (isShown) {
            if (isShown)
                $('#dialogDetail').appendTo("body").modal();
            else {
                $('#dialogDetail').modal('hide');
            }
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.detailFormUrl = '/appviews/dashboard/roles/deleterole.html';
        $scope.FormTitle = $scope.languages.ROLES.DELETE_HEADING;
        $scope.pageInfo = {};
        $scope.replaceto = $scope.languages.ROLES.REPLACE_TO;
        $scope.fromRoles = [];
        $scope.toRoles = [];
        $scope.roles = [];
        $scope.key = '';

        // --- Define Scope Method. ---------------------- //

        $scope.Save = function () {
            roleService.DeleteRoles($scope.fromRoles)
                .then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        showDialog(false);
                        getRoles();
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.PageIndex = 1;
            $scope.pageInfo.AdvanceSearch = false;
            getRoles();
            event.preventDefault();
        });
        $scope.$on('deleteEvent', function (event) {

            $scope.fromRoles = [];
            $scope.toRoles = [];
            angular.forEach($scope.roles, function (value, key) {
                var obj = _.findWithProperty($scope.EditValues, 'Id', value.Id);
                if (obj == null || value.Name == 'Administrator') {
                    $scope.toRoles.push(value);
                }
            });
            var flag = false;
            angular.forEach($scope.EditValues, function (value, key) {
                var role = _.findWithProperty($scope.roles, 'Id', value.Id);
                if (role.Name != 'Administrator') {
                    var obj = {
                        Id: role.Id,
                        Name: role.Name,
                        ToRoleId: $scope.toRoles[0].Id
                    };
                    $scope.fromRoles.push(obj);
                } else {
                    flag = true;
                }
            });
            if (flag) {
                NotifyError($scope.languages.ROLES.DENY_DELETE_ROLE);
            }
            if ($scope.fromRoles.length > 0) {
                showDialog(true);
            } else {
                if ($scope.EditValues.length == 0) {
                    NotifyError($scope.languages.ROLES.NO_ITEM);
                }
            }
            event.preventDefault();
        });
        $scope.$on('yesEvent', function (event) {
            event.preventDefault();
        });
        $scope.$on('noEvent', function (event) {
            event.preventDefault();
        });
        $scope.$on('addEvent', function (event) {
            $location.path('/dashboard/roles/add');
            event.preventDefault();
        });

        $scope.$on('cancelEvent', function (event) {
            event.preventDefault();
        });

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            $scope.pageInfo.PageSize = $scope.maxPageSize;
            getRoles();
            toolbarService.ShowAdvanceSearch(false);
            $scope.setWindowTitle($scope.languages.ROLES.MANAGE_ROLE_TITLE);
            event.preventDefault();
        });

        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter(toolbarService.getCurrentViewId());
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            getRoles();
            event.preventDefault();
        });
        $scope.$on('rowClickEvent', function (event, args) {
            $location.path('/dashboard/roles/edit/' + args.RowId);
            event.preventDefault();
        });
        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.CurrentView);
        });

        $scope.$on('doFilterEvent', function (event, filterConditions) {

            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression).then(function (data) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            });
        });
    }

})();