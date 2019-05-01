(function() {
    
    "use strict";

    angular.module("LeonardCRM").controller("ManageCurrencyCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$compile', 'currencyService', 'toolbarService', '$timeout', 'windowService'];

    function ctrl($scope, $http, $compile, currencyService, toolbarService, $timeout, windowService) {

        $scope.pagedInfo = {};
        $scope.currencies = [];
        $scope.currencyName = [];

        $scope.LoadData = function () {
            currencyService.GetAllCurrency().then(function (data, status, headers, config) {
                $scope.currencies = angular.fromJson(data);
            });
        };

        $scope.editRow = function (item) {
            var param = {
                Id: item.Id,
                Url: '/appviews/dashboard/currency/editcurrency.html',
                ParentId: 0,
                ViewId: 0,
                ModuleId: $scope.CurrentModule,
                Key: 'currency' + item.Id.toString()
            };
            windowService.openWindow(param);
        };

        $scope.$on('addEvent', function (event, args) {
            var param = {
                Id: 0,
                Url: '/appviews/dashboard/currency/editcurrency.html',
                ParentId: 0,
                ViewId: 0,
                ModuleId: $scope.CurrentModule,
                Key: 'currency0'
            };
            windowService.openWindow(param);
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.LoadData();
        });

        $scope.LoadData();
        toolbarService.ShowAdvanceSearch(false);
        //---------------END REGISTER EVENT-------------------
        $scope.setWindowTitle($scope.languages.CURRENCY.MANAGE_TITLE);
    }
    

})();