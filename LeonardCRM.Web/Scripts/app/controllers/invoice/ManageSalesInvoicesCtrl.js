(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ManageInvoicesCtrl", ctrl);

    ctrl.$inject = ["$scope", "$routeParams", "$http", "$location", "$compile", "$timeout", "filterService", "salesInvoiceService", "salesOrderService", "registryService", "toolbarService", "viewService", "windowService"];
    
    function ctrl($scope, $routeParams, $http, $location, $compile, $timeout, filterService, salesInvoiceService, salesOrderService, registryService, toolbarService, viewService, windowService) {
        $scope.pageInfo = {};

        //---------------------------START DELETE SALES INVOICE-----------------------------
        $scope.DeleteSalesInvoice = function () {
            salesInvoiceService.DeleteSalesInvoice($scope.EditValues)
                .then(deleteSalesInvoiceSuccessCallback);
        };
        var deleteSalesInvoiceSuccessCallback = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result);
                $scope.refreshViewMenu();
            } else {
                NotifyError(data.Result);
            }
            $scope.GetViewById($scope.pageInfo);
        };

        var checkToShowDetail = function () {
            if (angular.isDefined($routeParams.viewId)
                && $routeParams.viewId !== null
                && angular.isDefined($routeParams.invoiceId)
                && $routeParams.invoiceId !== null) {
                var param = {
                    Url: '/appviews/invoice/detail.html',
                    Id: $routeParams.invoiceId,
                    ParentId: 0,
                    ViewId: $routeParams.viewId,
                    ModuleId: 4,
                    Key: 'invoice' + $routeParams.invoiceId
                };

                windowService.openWindow(param);
            }
        }

        //---------------------------END DELETE SALES INVOICES---------------------------

        //----------------------------START GETVIEWBYID-------------------------
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
        //---------------------------END GETVIEWBYID----------------------------

        //--------------------EVENT HANDLELER REGISTER----------------

        //-----------Advance Search Event Listener---------------------------------
        $scope.$on('doFilterEvent', function (event, filterConditions) {
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.ViewId, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression).then(function (data) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            });
        });

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

        $scope.$on('addEvent', function (e) {
            var param = {
                Url: '/appviews/invoice/detail.html',
                Id: 0,
                ParentId: 0,
                ViewId: $scope.ViewId,
                ModuleId: $scope.pageInfo.ModuleId,
                Key: $scope.pageInfo.Models.module
            };
            windowService.openWindow(param);
        });

        $scope.$on('rowClickEvent', function (event, args) {
            var param = {
                Url: '/appviews/invoice/detail.html',
                Id: args.RowId,
                ParentId: 0,
                ViewId: args.ViewId,
                ModuleId: args.ModuleId,
                Key: args.Key
            };
            windowService.openWindow(param);
        });

        $scope.$on('deleteEvent', function (e) {
            $scope.SetConfirmMsg($scope.languages.INVOICES.DELETE_MSG, 'manageinvoice');
        });

        $scope.$on('yesEvent', function (event, args) {
            if (args == 'manageinvoice') {
                $scope.DeleteSalesInvoice();
            }
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            filterService.doFilter($scope.ViewId);
            event.preventDefault();
        });

        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.AdvanceSearch = false;
            $scope.pageInfo.PageIndex = 1;
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
        $scope.setWindowTitle($scope.languages.INVOICES.MANAGE_TITLE);
    }

})(angular);