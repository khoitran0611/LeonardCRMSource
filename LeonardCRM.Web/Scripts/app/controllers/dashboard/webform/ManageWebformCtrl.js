(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ManageWebformCtrl", ctrl);

    ctrl.$inject = ["$scope", "requestContext", "viewService", "windowService", "filterService", "webformService"];

    function ctrl($scope, requestContext, viewService, windowService, filterService, webformService) {
        $scope.pagedInfo = {};
        $scope.formKey = 'managewebforms';

        $scope.$on('addEvent', function (event) {
            var param = {
                Url: '/appviews/dashboard/webform/webform-editor.html',
                Id: 0,
                ParentId: 0,
                ViewId: $scope.pageInfo.ViewId,
                ModuleId: $scope.pageInfo.ModuleId,
                Key: $scope.pageInfo.Models.module
            };
            windowService.openWindow(param);
        });

        $scope.$on('rowClickEvent', function (event, args) {
            var param = {
                Url: '/appviews/dashboard/webform/webform-editor.html',
                Id: args.RowId,
                ParentId: args.ParentId,
                ViewId: args.ViewId,
                ModuleId: args.ModuleId,
                Key: args.Key
            };
            windowService.openWindow(param);
        });

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            $scope.setWindowTitle($scope.languages.WEBFORM.MANAGE_TITLE);
            filterService.doFilter($scope.pageInfo.ViewId);
            event.preventDefault();
        });

        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter($scope.pageInfo.ViewId);
        });

        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.AdvanceSearch = false;
            $scope.pageInfo.PageIndex = 1;
            filterService.doFilter($scope.pageInfo.ViewId);
            event.preventDefault();
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.pageInfo.ViewId);
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            filterService.doFilter($scope.pageInfo.ViewId);
            event.preventDefault();
        });

        $scope.$on('doFilterEvent', function (event, filterConditions) {
            $scope.pageInfo.AdvanceSearch = true;
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.pageInfo.ViewId, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression)
                .then(function (data) {
                    $scope.pageInfo = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.$broadcast('refeshDataByKey', args);
        });

        $scope.$on('deleteEvent', function (event) {
            $scope.SetConfirmMsg($scope.languages.WEBFORM.CONFIRM_DELETE_MSG, $scope.formKey);
            event.preventDefault();
        });

        $scope.$on('yesEvent', function (event, args) {
            if (args == $scope.formKey) {
                webformService.deleteObject($scope.EditValues)
                .then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        filterService.doFilter($scope.pageInfo.ViewId);
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
            }
        });
        $scope.$on('noEvent', function (event) {
            event.preventDefault();
        });

    }

})(angular);