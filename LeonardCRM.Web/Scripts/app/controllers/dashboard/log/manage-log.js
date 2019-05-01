(function() {

    'use strict';

    angular.module("LeonardCRM").controller("ManageLogCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'filterService', 'toolbarService', 'logService', 'windowService', 'viewService'];

    function ctrl($scope, $http, $location, filterService, toolbarService, logService, windowService, viewService) {

            $scope.pageInfo = {};

            //------------START GETVIEWBYID----------------
            var getViewSuccessCallback = function(data, status, headers, config) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            };

            $scope.GetViewById = function(args) {
                if (args.ViewId > 0) {
                    viewService.GetView(args)
                        .then(getViewSuccessCallback);
                }
            };
            //------------END GETVIEWBYID-------------------

            //--------------START DELETE LOG----------------
            $scope.DeleteLog = function() {
                logService.DeleteLog($scope.EditValues)
                    .then(deleteLogSuccessCallback);
            };

            var deleteLogSuccessCallback = function(data, status, headers, config) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                } else {
                    NotifyError(data.Result, 5000);
                }
                $scope.GetViewById($scope.pageInfo);
            };

            //-------------END DELETE LOG------------------

            //---------------start clear log---------------
            $scope.ClearLog = function() {
                logService.ClearLog()
                    .then(clearLogSuccessCallback);
            };

            var clearLogSuccessCallback = function(data, status, headers, config) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                } else {
                    NotifyError(data.Result, 5000);
                }
                $scope.GetViewById($scope.pageInfo);
            };
            //------------end clear log-------------------------

            //----------------START REGISTER EVENT--------------------------

            $scope.$on('Grid_OnNeedDataSource', function(event, args) {
                $scope.pageInfo = args;
                $scope.GetViewById(args);
                toolbarService.ShowAdvanceSearch(false);
                event.preventDefault();
            });

            $scope.$on('GridPageIndexChanged', function(event, args) {
                $scope.pageInfo = angular.copy(args);
                filterService.doFilter(toolbarService.getCurrentViewId());
            });

            $scope.$on('sortEvent', function(event, args) {
                $scope.pageInfo = args;
                $scope.GetViewById(args);
                event.preventDefault();
            });

            $scope.$on('refreshEvent', function(event) {
                $scope.pageInfo.PageIndex = 1;
                $scope.pageInfo.AdvanceSearch = false;
                $scope.GetViewById($scope.pageInfo);
                event.preventDefault();
            });

            $scope.$on('deleteEvent', function(e) {
                $scope.SetConfirmMsg($scope.languages.LOG.DELETE_MSG, 'managelog');
            });

            $scope.$on('yesEvent', function(event, args) {
                if (args == 'managelog') {
                    $scope.DeleteLog();
                }
            });

            $scope.$on('rowClickEvent', function(event, args) {
                var param = {
                    Url: '/appviews/dashboard/log/detail.html',
                    Id: args.RowId,
                    ParentId: args.ParentId,
                    ViewId: args.ViewId,
                    ModuleId: args.ModuleId,
                    Key: args.Key
                };
                windowService.openWindow(param);
            });

            $scope.$on('ServerFilter', function(event, args) {
                filterService.doFilter($scope.CurrentView);
            });

            $scope.$on('doFilterEvent', function(event, filterConditions) {

                var advConditions = filterConditions;
                viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression).then(function(data) {
                    $scope.pageInfo = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
            });
            $scope.$on('$destroy', function() {
                toolbarService.NeedAddCommand(true);
            });
            //---------------END REGISTER EVENT-------------------
            $scope.setWindowTitle($scope.languages.LOG.MANAGE_TITLE);
            toolbarService.NeedAddCommand(false);
        }   

})();
