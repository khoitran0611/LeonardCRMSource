(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("PaidOverviewCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$route", "$location", "$routeParams", "currencyService", "appService", "roleService", "registryService", "salesInvoiceService", "requestContext", "viewService", "toolbarService", "_"];

    function ctrl($scope, $http, $route, $location, $routeParams, currencyService, appService, roleService, registryService, salesInvoiceService, requestContext, viewService, toolbarService, _) {

        $scope.dateFormat = registryService.siteSettings.DATE_FORMAT + ' ' + registryService.siteSettings.TIME_FORMAT;
        $scope.defaultView = appService.getDefaultView('invoice');
        $scope.isLoading = false;
        $scope.onlyMe = true;
        $scope.currentValue = 7;
        $scope.listData = [];
        $scope.weekData = null;
        $scope.monthData = null;
        $scope.yearData = null;
        $scope.totalInvoicedWeek = 0;
        $scope.totalInvoicedMonth = 0;
        $scope.totalInvoicedYear = 0;
        $scope.totalPaidWeek = 0;
        $scope.totalPaidMonth = 0;
        $scope.totalPaidYear = 0;
        $scope.currency = registryService.siteSettings.CURRENCY;
        $scope.currencySymbol = '';
        $scope.listCurrency = [];

        var configChart = function () {
            chart1.type = "ColumnChart";
            chart1.displayed = false;
            chart1.data = {
                "cols": [
                    { id: "date", label: "Date", type: "string" },
                    { id: "invoiced", label: $scope.languages.INVOICES.INVOICED, type: "number" },
                    { id: "paid", label: $scope.languages.INVOICES.PAID, type: "number" }
                ],
                "rows": [
                ]
            };

            chart1.options = {
                "isStacked": "false",
                "fill": 20,
                "displayExactValues": true,
                "vAxis": {
                    "title": $scope.languages.INVOICES.INVOICED_PAID_CHART_Y_TITLE + ' (' + $scope.currencySymbol + ')',
                    "gridlines": { "count": 10 }
                },
                "hAxis": {
                    "title": $scope.languages.INVOICES.INVOICED_PAID_CHART_X_TITLE,
                    "format": registryService.siteSettings.DATE_FORMAT
                },
                pointSize: 4
            };

            $scope.chart = chart1;
        };

        var chart1 = {};


        var calculateTotal = function () {
            $scope.totalInvoicedWeek = 0;
            $scope.totalInvoicedMonth = 0;
            $scope.totalInvoicedYear = 0;
            $scope.totalPaidWeek = 0;
            $scope.totalPaidMonth = 0;
            $scope.totalPaidYear = 0;

            //Week
            var weekObj = $scope.weekData.rows;
            angular.forEach(weekObj, function (value, index) {
                var obj = value.c;
                $scope.totalInvoicedWeek += parseFloat(obj[1].v);
                $scope.totalPaidWeek += parseFloat(obj[2].v);
            });

            //Months
            var monthObj = $scope.monthData.rows;
            angular.forEach(monthObj, function (value, index) {
                var obj = value.c;
                $scope.totalInvoicedMonth += parseFloat(obj[1].v);
                $scope.totalPaidMonth += parseFloat(obj[2].v);
            });

            //Year
            var yearObj = $scope.yearData.rows;
            angular.forEach(yearObj, function (value, index) {
                var obj = value.c;
                $scope.totalInvoicedYear += parseFloat(obj[1].v);
                $scope.totalPaidYear += parseFloat(obj[2].v);
            });
        };

        var getCurrency = function () {
            currencyService.GetAllCurrency().then(function (data) {
                $scope.listCurrency = angular.fromJson(data);

                angular.forEach($scope.listCurrency, function (value, index) {
                    if (value.BaseCurrency) {
                        $scope.currencySymbol = value.Symbol;
                        $scope.currency = value.Id;
                    }
                });
                configChart();
                loadReport();
            });
        };

        function loadReport() {
            $scope.isLoading = true;


            salesInvoiceService.getChartInvoicePaidData($scope.onlyMe, $scope.currency).then(function (data) {
                $scope.weekData = angular.fromJson(data)[0];
                $scope.monthData = angular.fromJson(data)[1];
                $scope.yearData = angular.fromJson(data)[2];

                calculateTotal();
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

        $scope.currencyChanged = function () {
            angular.forEach($scope.listCurrency, function (value, index) {
                if ($scope.currency == value.Id) {
                    $scope.currencySymbol = value.Symbol;
                }
            });
            configChart();
            loadReport();
        };

        $scope.$watch('currentValue', function (newVal, oldVal) {
            if (newVal != oldVal) {
                reloadChart();
            }
        });

        $scope.$watch('onlyMe', function (newVal, oldVal) {
            if (newVal != oldVal) {
                loadReport();
            }
        });

        getCurrency();
    }

})(angular);