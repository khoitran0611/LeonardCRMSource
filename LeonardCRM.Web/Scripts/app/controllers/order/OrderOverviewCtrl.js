(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("OrderOverviewCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "$window", "appService", "roleService", "registryService", "salesOrderService"];

    function ctrl($scope, $http, $route, $location, $routeParams, $window, appService, roleService, registryService, salesOrderService) {

        $scope.dateFormat = registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT;
        $scope.defaultView = appService.getDefaultView('order');
        $scope.isLoading = true;
        $scope.onlyMe = true;
        $scope.myOptions = [
            { Text: $scope.languages.REPORT.RECENTLY_ADDED, Value: 0 },
            { Text: $scope.languages.REPORT.LAST_7_DAYS, Value: 7 },
            { Text: $scope.languages.REPORT.LAST_30_DAYS, Value: 30 },
            { Text: $scope.languages.REPORT.CURRENT_YEAR, Value: 365 }
        ];
        $scope.currentValue = 0;
        $scope.listData = [];
        $scope.weekData = null;
        $scope.monthData = null;
        $scope.yearData = null;

        var chart1 = {};
        chart1.type = "LineChart";
        chart1.displayed = false;
        chart1.data = {
            "cols": [
                { id: "date", label: "Date", type: "string" },
                { id: "total", label: $scope.languages.ORDERS.OVERVIEW_CHART_Y_TITLE, type: "number" }
            ],
            "rows": [
            ]
        };

        chart1.options = {
            //"title": $scope.languages.SALES_CUSTOMER.OVERVIEW_CHART_TITLE,
            "isStacked": "false",
            "fill": 20,
            "displayExactValues": true,
            "legend": { position: 'none' },
            "vAxis": {
                "title": $scope.languages.ORDERS.OVERVIEW_CHART_Y_TITLE,
                "format": '#'
            },
            "hAxis": {
                "title": $scope.languages.ORDERS.OVERVIEW_CHART_X_TITLE,
                "format": registryService.siteSettings.DATE_FORMAT
            },
            pointSize: 4
        };

        function loadTopItems() {
            salesOrderService.getRecentlyAddedOrders($scope.onlyMe).then(function (data) {
                $scope.listData = angular.fromJson(data);
            });
        }

        function loadReport() {
            $scope.isLoading = true;

            loadTopItems();
            salesOrderService.getChartData($scope.onlyMe).then(function (data) {
                $scope.weekData = angular.fromJson(data)[0];
                $scope.monthData = angular.fromJson(data)[1];
                $scope.yearData = angular.fromJson(data)[2];
                reloadChart();
                $scope.isLoading = false;
            });
        }

        function reloadChart() {
            switch ($scope.currentValue) {
                case 7:
                    $scope.chart.displayed = true;
                    $scope.chart.data.rows = $scope.weekData.rows;
                    break;
                case 30:
                    $scope.chart.displayed = true;
                    $scope.chart.data.rows = $scope.monthData.rows;
                    break;
                case 365:
                    $scope.chart.displayed = true;
                    $scope.chart.data.rows = $scope.yearData.rows;
                    break;
                default:
                    $scope.chart.displayed = false;
                    break;
            }
        }

        $scope.chart = chart1;

        loadReport();

        $scope.$watch('currentValue', function (v) {
            reloadChart();
        });

        $scope.$watch('onlyMe', function (newVal, oldVal) {
            if (newVal != oldVal) {
                loadReport();
            }
        });

        $scope.openOrderDetail = function (defaultView, id) {
            var link = '#/order/view/' + defaultView + '/' + id;
            $window.location.href = link;
        }
    }

})(angular);