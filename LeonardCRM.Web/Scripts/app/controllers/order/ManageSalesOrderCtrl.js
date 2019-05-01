(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ManageOrdersCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "$compile", "$timeout", "salesInvoiceService", "salesOrderService", "registryService", "toolbarService", "viewService", "filterService", "$routeParams", "requestContext", "windowService"];

    function ctrl($scope, $http, $location, $compile, $timeout, salesInvoiceService, salesOrderService, registryService, toolbarService, viewService, filterService, $routeParams, requestContext, windowService) {
        $scope.pageInfo = {};
        $scope.setWindowTitle($scope.languages.ORDERS.MANAGE_TITLE);       

        var checkToShowDetail = function () {
            if (angular.isDefined($routeParams.viewId)
                && $routeParams.viewId !== null
                && angular.isDefined($routeParams.orderId)
                && $routeParams.orderId !== null) {

                var param = {
                    Url: '/appviews/Order/detail.html',
                    Id: $routeParams.orderId,
                    ParentId: 0,
                    ViewId: $routeParams.viewId,
                    ModuleId: 3,
                    Key: 'order' + $routeParams.orderId
                };
                windowService.openWindow(param);
            }
        }

        //---------START DELETE ORDER-----------------
        $scope.DeleteSalesOrder = function () {
            salesOrderService.DeleteSalesOrder($scope.EditValues)
                .then(deleteSalesOrderSuccessCallback);
        };

        var deleteSalesOrderSuccessCallback = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result);
                $scope.refreshViewMenu();
            } else {
                NotifyError(data.Result);
            }
            $scope.GetViewById($scope.pageInfo);
        };
        //---------END DELETE ORDER-------------------

        var deleteSalesOrderErrorCallback = function (data, status, headers, config) {
            NotifyError(data.toString(), 3000);
        };

        //------------START GETVIEWBYID----------------
        var getViewSuccessCallback = function (data, status, headers, config) {
            $scope.pageInfo = angular.fromJson(data);
            $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
        };

        $scope.GetViewById = function (args) {
            if (args.ViewId > 0) {
                viewService.GetView(args)
                    .then(getViewSuccessCallback);
            }
        };
        //------------END GETVIEWBYID-------------------

        //----------------START REGISTER EVENT----------------

        //-----------Advance Search Event Listener---------------------------------
        $scope.$on('doFilterEvent', function (event, filterConditions) {
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.ViewId, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression)
                .then(function (data) {
                    $scope.pageInfo = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
        });

        //-----------END Advance Search Event Listener---------------------------------

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            filterService.doFilter($scope.ViewId);
            event.preventDefault();

            checkToShowDetail();
        });

        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter($scope.ViewId);
        });

        $scope.$on('rowClickEvent', function (event, args) {
            $('#dialogDetail').remove();
            var param = {
                Url: '/appviews/Order/detail.html',
                Id: args.RowId,
                ParentId: args.ParentId,
                ViewId: args.ViewId,
                ModuleId: args.ModuleId,
                Key: args.Key
            };
            windowService.openWindow(param);
        });

        $scope.$on('deleteEvent', function (e) {
            $scope.SetConfirmMsg($scope.languages.ORDERS.DELETE_MSG, 'manageorder');
        });

        $scope.$on('yesEvent', function (event, args) {
            if (args == 'manageorder') {
                //if user click ok then delete the selected record
                $scope.DeleteSalesOrder();
            }
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            filterService.doFilter($scope.ViewId);
            event.preventDefault();
        });

        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.PageIndex = 1;
            $scope.pageInfo.AdvanceSearch = false;
            filterService.doFilter($scope.ViewId);
            event.preventDefault();
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.$broadcast('refeshDataByKey', args);
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.ViewId);
        });

        //---------------END REGISTER EVENT-------------------
    }
})(angular);