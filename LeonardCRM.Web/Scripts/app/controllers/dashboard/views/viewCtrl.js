(function () {
    'use strict';

    angular.module("LeonardCRM").controller("viewCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "filterService", "requestContext", "viewService", "appService", "toolbarService", "_", "roleService", "registryService"];
    
    function ctrl($scope, $http, $location, filterService, requestContext, viewService, appService, toolbarService, _, roleService, registryService) {
        $scope.pageInfo = {};


        var deleteView = function () {
            viewService.deleteViews($scope.EditValues)
                .then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        loadViewsData();
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
        };

        //-----Toolbar event handlers----------
        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.AdvanceSearch = false;
            $scope.pageInfo.PageIndex = 1;
            reloadViewData();
            event.preventDefault();
        });
        $scope.$on('deleteEvent', function () {
            $scope.SetConfirmMsg('Are you sure you want to delete?');
        });

        $scope.$on('addEvent', function (event) {
            //always set this to false when not redirecting to detail page, e.g: edit using dialog
            $location.path('/dashboard/views/add');
            event.preventDefault();
        });

        //-----grid events------------ 
        $scope.$on('rowClickEvent', function (event, args) {
            $location.path('/dashboard/' + args.ModuleName + '/' + args.RowId);
            event.preventDefault();
        });
        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = angular.copy(args);
            filterService.doFilter(toolbarService.getCurrentViewId());
        });
        $scope.$on('sortEvent', function (event, args) {
            reloadViewData(args);
        });

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            loadViewsData();
            $scope.setWindowTitle($scope.languages.VIEW.MANAGE_VIEW);
            toolbarService.ShowAdvanceSearch(false);
            event.preventDefault();
        });

        $scope.$on('deleteEvent', function (event) {
            $scope.SetConfirmMsg($scope.languages.VIEW.CONFIRM_DELETE_MSG);
            event.preventDefault();
        });
        $scope.$on('yesEvent', function (event) {
            deleteView();
            event.preventDefault();
        });
        $scope.$on('noEvent', function (event) {
            event.preventDefault();
        });

        //------Advance Search Event Listener---------------------------------
        $scope.$on('doFilterEvent', function (event, filterConditions) {
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression).then(function (data) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            });
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.CurrentView);
        });

        //------Define controller's methods
        function reloadViewData(args) {
            if (args != null) {
                $scope.pageInfo = args;
            }
            loadViewsData();
        }

        //------Define scope's methods
        function initManagePage() {
            $scope.setWindowTitle($scope.languages.USERS.MANAGE_TITLE);
            $scope.NoSelectedItems = true;
        };

        var loadViewsData = function () {
            viewService.GetView($scope.pageInfo).then(function (data, status, headers, config) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            });
        };

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext('dashboard.views');

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {

                    return;

                }

                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();

            }
        );

        //---Initiallize------------
        initManagePage();
    }
})();