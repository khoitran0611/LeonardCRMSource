(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("ManageApplications", ctrl);

    ctrl.$inject = ["$scope", "NgTableParams", "salesOrderService", "ngTableService", "$location"];

    function ctrl($scope, NgTableParams, salesOrderService, ngTableService, $location) {
        //----------private variables--------------------       

        //----------scope method--------------------       
        $scope.getColorFromPicklist = function (picklist) {
            return ngTableService.getColorFromPicklist(picklist);
        };

        $scope.editApp = function (appId) {
            if (appId > -1) {
                $location.path( + appId);
            }
        
        }


		//---------Scope event---------------
        $scope.onClickRow = function (appId) {
        	$location.path("/my-applications/" + appId);
        }

        //---------internal method-----------------------
        function init() {
            $scope.setPageHeader($scope.languages.MANAGE_APPLICATION.HEADING,'');
            $scope.setWindowTitle($scope.languages.MANAGE_APPLICATION.TITLE);
            var setting = angular.fromJson(localStorage.getItem("settings"));
            if (angular.isDefined(setting) && setting != null) {
                salesOrderService.getOwnApplication($scope.modeParam && $scope.modeParam != '').then(function (result) {
                    //init table
                    $scope.tableParams = new NgTableParams({
                        sorting: { Id: "desc" },
                        count: setting.ITEMS_PER_PAGE
                    }, {
                        counts: [5, 10, 20, 25, 50],
                        paginationMaxBlocks: setting.MAX_PAGE_NUMBERS,
                        paginationMinBlocks: 1,
                        dataset: result.Data
                    });
                });
            }
        }

        init();
    }

})();