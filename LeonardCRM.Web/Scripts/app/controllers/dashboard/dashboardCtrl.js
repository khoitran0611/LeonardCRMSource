(function (ng) {
    'use strict';

    angular.module("LeonardCRM").controller("dashboardCtrl", ctrl);

    ctrl.$inject = ["$scope", "requestContext", "appService", "filterService", "toolbarService"];

    function ctrl($scope, requestContext, appService, filterService, toolbarService) {

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("dashboard");

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();
        $scope.userGridUrl = "";
        $scope.CurrentModule = $scope.ModuleId;
        $scope.CurrentView = $scope.ViewId;

        $scope.Init = function () {
            $scope.setWindowTitle($scope.languages.USERS.DASHBOARD_TITLE);
            if ($scope.subview != null) {
                $scope.CurrentModule = appService.getModuleId($scope.subview);
                $scope.CurrentView = appService.getDefaultView($scope.subview);
                toolbarService.setCurrentViewId($scope.CurrentView);
                toolbarService.setCurrentModuleId($scope.CurrentModule);
                $scope.$broadcast('getCurrentViewSuccess');
                if ($scope.CurrentModule != null && $scope.CurrentModule > 0) {
                    if ($scope.subview !== 'views' || requestContext.getRenderContext("dashboard.views").getNextSection() !== 'add') {
                        filterService.preLoadFilterColumnsAndPickLists($scope.CurrentModule);
                    }
                }
            }
        };

        //only load grid after picklists loaded to avoid grid filter function error
        $scope.$on('filterPickListEvent', function (event, moduleId) {
            if ($scope.CurrentModule > 0 && moduleId == $scope.CurrentModule) {
                $scope.userGridUrl = '';
                $scope.userGridUrl = '/home/dynamicgrid/' + $scope.CurrentView + "/" + $scope.CurrentModule + "/?t=" + Math.random();
            }
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
        });

        $scope.$on('ColumnsChanged', function (event, args) {
            $scope.userGridUrl = '';
            $scope.userGridUrl = '/home/dynamicgrid/' + $scope.CurrentView + "/" + $scope.CurrentModule + "/?t=" + Math.random();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {
                $scope.userGridUrl = '';
                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {
                    return;
                }
                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();
                $scope.Init();
            }
        );
        $scope.Init();
    }

})(angular);