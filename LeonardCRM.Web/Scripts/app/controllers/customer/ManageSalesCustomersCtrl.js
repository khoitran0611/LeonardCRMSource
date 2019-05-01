(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ManageSalesCustomersCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$route', '$location', '$routeParams', 'filterService', 'salesCustomerService', 'viewService', 'requestContext', 'windowService', 'toolbarService'];

    function ctrl($scope, $http, $route, $location, $routeParams, filterService, salesCustomerService, viewService, requestContext, windowService) {

        // --- Define Controller Methods. ------------------- //

        var deleteCustomersSelectedSuccessCallBack = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                $scope.refreshViewMenu();
                filterService.doFilter($scope.ViewId);
            } else {
                NotifyError(data.Result, 5000);
            }
        };
        var showDialongImport = function (isShown) {
            if (isShown)
                $('#dialogDetail').appendTo("body").modal({
                    show: true,
                    keyboard: false,
                    backdrop: 'static'
                });
            else {
                $('#dialogDetail').modal('hide');
            }
        };

        var initForm = function () {

            showDialongImport();
        };

        var checkToShowDetail = function () {
            if (angular.isDefined($routeParams.viewId)
                && $routeParams.viewId !== null
                && angular.isDefined($routeParams.customerId)
                && $routeParams.customerId !== null) {
                var param = {
                    Url: '/appviews/Customer/detail.html',
                    Id: $routeParams.customerId,
                    ParentId: 0,
                    ViewId: $routeParams.viewId,
                    ModuleId: 2,
                    Key: 'customer' + $routeParams.customerId
                };
                windowService.openWindow(param);
            }
        }

        // --- Define Scope Methods. ------------------------ //

        $scope.DeleteCustomersDeleted = function () {
            salesCustomerService.DeleteCustomers($scope.EditValues)
                .then(deleteCustomersSelectedSuccessCallBack);
        };

        $scope.filterAlphabet = function (str) {
            var conditionObjs = [];
            if (str == 'All') {
                $scope.resetGrid();
            } else {
                var conditionObj = {
                    FieldId: 51,
                    ColumnName: 'Name',
                    Operator: 'StartsWith',
                    FilterValue: str,
                    IsAND: true
                };
                conditionObjs = [];
                conditionObjs.push(conditionObj);
                filterService.setAlphabetCondition(conditionObjs, $scope.pageInfo.ViewId);
                $scope.activeAnphabe = str;
            }
            filterService.doFilter($scope.ViewId);
        };

        $scope.resetGrid = function () {
            $scope.activeAnphabe = 'All';
            filterService.setAlphabetCondition([], $scope.pageInfo.ViewId);
        };

        // --- Define Controller Variables. ----------------- //

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("customer");

        // --- Define Scope Variables. ---------------------- //

        $scope.pageInfo = {};
        $scope.BrowseAllCustomers = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];
        $scope.activeAnphabe = 'All';
        $scope.count = 0;
        $scope.viewName = '';

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('refreshEvent', function (event) {
            $scope.resetGrid();
            $scope.pageInfo.AdvanceSearch = false;
            $scope.pageInfo.PageIndex = 1;
            filterService.doFilter($scope.ViewId);
            event.preventDefault();
        });

        $scope.$on('addEvent', function (event) {
            var param = {
                Url: '/appviews/Customer/detail.html',
                Id: 0,
                ParentId: 0,
                ViewId: $scope.ViewId,
                ModuleId: $scope.pageInfo.ModuleId,
                Key: $scope.pageInfo.Models.module
            };
            windowService.openWindow(param);
        });

        $scope.$on('deleteEvent', function (event) {
            $scope.SetConfirmMsg($scope.languages.SALES_CUSTOMER.CONFIRM_DELETE_MSG, 'managecustomer');
            event.preventDefault();
        });
        $scope.$on('yesEvent', function (event, args) {
            if (args == 'managecustomer') {
                $scope.DeleteCustomersDeleted();
            }
        });
        $scope.$on('noEvent', function (event) {
            event.preventDefault();
        });
        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            $scope.setWindowTitle($scope.languages.SALES_CUSTOMER.MANAGE_CUSTOMER_TITLE);
            filterService.doFilter($scope.ViewId);
            event.preventDefault();

            checkToShowDetail();
        });

        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter($scope.ViewId);
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            filterService.doFilter($scope.ViewId);
            event.preventDefault();
        });

        $scope.$on('rowClickEvent', function (event, args) {
            if (args.IsCommand) {
                //NotifySuccess('Cell clicked: ' + args.ColumnName);
                //Cell clicked should be handled
            } else {
                var param = {
                    Url: '/appviews/Customer/detail.html',
                    Id: args.RowId,
                    ParentId: args.ParentId,
                    ViewId: args.ViewId,
                    ModuleId: args.ModuleId,
                    Key: args.Key
                };
                windowService.openWindow(param);
            }
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.$broadcast('refeshDataByKey', args);
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.ViewId);
        });

        // --- Advance Search Event Listener -------
        $scope.$on('doFilterEvent', function (event, filterConditions) {
            $scope.pageInfo.AdvanceSearch = true;
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.ViewId, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression)
                .then(function (data) {
                    $scope.pageInfo = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
        });

        $scope.$on('collectConditionsEvent', function (event) {
            if ($scope.activeAnphabe == 'All') {
                $scope.resetGrid();
            } else {
                var conditionObj = {
                    FieldId: 51,
                    ColumnName: 'Name',
                    Operator: 'StartsWith',
                    FilterValue: $scope.activeAnphabe,
                    IsAND: true
                };
                var conditionObjs = [];
                conditionObjs.push(conditionObj);
                filterService.setAlphabetCondition(conditionObjs, $scope.pageInfo.ViewId);
            }
        });

        $scope.$on('importEvent', function (event) {
            var param = {
                Url: '/appviews/Customer/importdialoglead.html',
                Id: 0,
                ParentId: 0,
                ViewId: 0,
                ModuleId: $scope.ModuleId,
                Key: $scope.ModuleId
            };
            windowService.openWindow(param);
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                if (requestContext.haveParamsChanged(["viewId"])) {
                    $scope.activeAnphabe = 'All';
                    if ($scope.ViewId == 0) {
                        $scope.pageInfo.Models = {};
                    }
                }

                $("#dialogDetail").remove();

                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {
                    return;
                }
            }
        );

        // --- Initialize. ---------------------------------- //
        initForm();
    }
})(angular);