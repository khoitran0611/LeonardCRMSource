(function () {

    'use strict';

    angular.module("LeonardCRM").controller("ManagePicklistDependencyCtrl", ctrl);

    ctrl.$inject = ['$scope', 'windowService', 'filterService', 'viewService', 'picklistDependencyService'];

    function ctrl($scope, windowService, filterService, viewService, picklistDependencyService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var deleteObjectCallback = function(data) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                filterService.doFilter($scope.CurrentView);
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var initForm = function () {

        };

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        $scope.pageInfo = {};

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.AdvanceSearch = false;
            $scope.pageInfo.PageIndex = 1;
            filterService.doFilter($scope.CurrentView);
            event.preventDefault();
        });
        $scope.$on('addEvent', function (event) {
            var param = {
                Url: '/appviews/dashboard/picklistdependency/edit.html',
                Id: 0,
                ParentId: 0,
                ViewId: $scope.CurrentView,
                ModuleId: $scope.pageInfo.ModuleId,
                Key: $scope.pageInfo.Models.module
            };
            windowService.openWindow(param);
        });
        $scope.$on('deleteEvent', function (event) {
            $scope.SetConfirmMsg($scope.languages.PICKLIST_DEPENDENCY.CONFIRM_DELETE_MSG, 'picklistdependency');
        });
        $scope.$on('yesEvent', function (event, args) {
            if (args == 'picklistdependency') {
                picklistDependencyService.deleteObjects($scope.EditValues)
                    .then(deleteObjectCallback);
            }
        });
        $scope.$on('noEvent', function (event) {

        });
        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            $scope.setWindowTitle($scope.languages.PICKLIST_DEPENDENCY.MANAGEMENT_TITLE);
            filterService.doFilter($scope.CurrentView);

        });
        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter($scope.CurrentView);
        });
        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            filterService.doFilter($scope.ViewId);

        });
        $scope.$on('rowClickEvent', function (event, args) {
            var param = {
                Url: '/appviews/dashboard/picklistdependency/edit.html',
                Id: args.RowId,
                ParentId: args.ParentId,
                ViewId: args.ViewId,
                ModuleId: args.ModuleId,
                Key: args.Key
            };
            windowService.openWindow(param);
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.$broadcast('refeshDataByKey', args);
            $('#dialogDetail').remove();
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.CurrentView);
        });
        // --- Advance Search Event Listener -------
        $scope.$on('doFilterEvent', function (event, filterConditions) {
            $scope.pageInfo.AdvanceSearch = true;
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression)
                .then(function (data) {
                    $scope.pageInfo = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
        });

        
        // --- Initialize. ------------------------ //
        initForm();
    }

})();