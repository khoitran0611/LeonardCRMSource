(function () {

    "use strict";

    angular.module("LeonardCRM").controller("viewMenuCtrl", ctrl);

    ctrl.$inject = ["$scope", "$location", "appService", "viewService", "_"];

    function ctrl($scope, $location, appService, viewService, _) {
        $scope.viewMenus = [];
        $scope.personalViews = [];
        $scope.deleteViewId = 0;
        $scope.viewKey = '';
        $scope.moduleObj = {};
        var processRedirect = function () {
            var ind = 0;
            // check personal view
            angular.forEach($scope.personalViews, function (viewObj, index) {
                if ($scope.deleteViewId == viewObj.ViewId) {
                    ind = index;
                    $scope.moduleObj = appService.getModuleById(viewObj.ModuleId);
                }
            });

            if (ind >= 0 && ind < $scope.personalViews.length - 1) {
                $location.path('/' + $scope.moduleObj.Name + '/view/' + $scope.personalViews[ind + 1].ViewId);
            } else if (ind > 0) {
                $location.path('/' + $scope.moduleObj.Name + '/view/' + $scope.personalViews[ind - 1].ViewId);
            } else {
                var view = _.findWithProperty($scope.viewMenus, "DefaultView", true);
                $location.path('/' + $scope.moduleObj.Name + '/view/' + view.ViewId);
            }
            // delete
            $scope.personalViews.splice(ind, 1);
        };

        $scope.reloadData = function () {
            $scope.moduleObj = appService.getModuleById($scope.ModuleId);
            if ($scope.moduleObj && !$scope.moduleObj.UseCustomView) {
                viewService.getViewsWithTotal($scope.ModuleId).then(function(data) {
                    var menu = angular.fromJson(data);
                    $scope.viewMenus = _.filter(menu, function(v) {
                        return v.Shared === true;
                    });
                    $scope.personalViews = _.filter(menu, function(v) {
                        return v.Shared === false;
                    });
                });
            }
        };
        $scope.DeletePrivateView = function(item) {
            $scope.viewKey = 'view-' + item.ViewId;
            $scope.SetConfirmMsg($scope.languages.GRID_VIEW.CONFIRM_DELETE_MSG, $scope.viewKey);
            $scope.deleteViewId = item.ViewId;
        };
        $scope.$on('reloadViewMenuEvent', function (event) {
            $scope.reloadData();
            event.preventDefault();
        });
        $scope.$on('yesEvent', function(event, args) {
            if (args == $scope.viewKey) {
                var indexChar = args.indexOf('-') + 1;
                var id = parseInt(args.substr(indexChar, args.length));
                var viewObjs = [{'Id':id}];
                viewService.deleteViews(viewObjs).then(function(data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        $scope.reloadData();
                        processRedirect();
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
            }
        });
        $scope.$broadcast('noEvent', function(event) {
            $scope.viewKey = '';
        });
		$scope.$watch('ModuleId', function (newVal, oldVal) {
            if (newVal > 0)
                $scope.reloadData();
        });
    }

})();
