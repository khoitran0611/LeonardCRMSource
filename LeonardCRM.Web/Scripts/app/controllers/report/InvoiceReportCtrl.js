(function () {

    "use strict";

    angular.module("LeonardCRM").controller("InvoiceReportCtrl", ctrl);

    ctrl.$inject = ["$scope", "$routeParams", "appService", "registryService", "picklistService", "userService", "salesCustomerService", "salesInvoiceService", "$timeout", "currencyService", "_"];

    function ctrl($scope, $routeParams, appService, registryService, picklistService, userService, salesCustomerService, salesInvoiceService, $timeout, currencyService, _) {

        // --- Define Controller Variables. ---------------------- //

        var chart = {};
        chart.type = $scope.chartType;
        chart.displayed = false;
        chart.cssStyle = "height:500px;width:100%";
        var defaultCol = [{ id: "date", label: "Date", type: "string" }, { id: "fake", label: "No data", type: "number" }];
        chart.data = {
            "cols": defaultCol,
            "rows": []
        };

        chart.options = {
            "title": "",
            "isStacked": "false",
            "fill": 20,
            "displayExactValues": true,
            "legend": { position: 'bottom' },
            "vAxis": {
                "title": "", "gridlines": { "count": $scope.gridLines }
            },
            "hAxis": {
                "title": ""
            },
            pointSize: 4
            ,
            "tooltip": { "isHtml": true }
        };

        var invoiceModule = 4;

        // --- Define Controller Method. ---------------------- //

        var getCurrentDate = function () {
            var currentDate = new Date();
            $scope.reportObj.ToDate = currentDate;

            var fromDate = new Date();
            fromDate.setDate(currentDate.getDate() - 30);
            $scope.reportObj.FromDate = fromDate;
        };

        var configChart = function (flag) {
            if (flag) {
                $scope.clientChart.options.title = $scope.languages.INVOICES.CLIENT_REPORT_CHART_TITLE;
                $scope.clientChart.options.vAxis.title = $scope.languages.INVOICES.CLIENT_REPORT_TABLE_COLUMN + ' (' + $scope.currentSymbol + ')';
                $scope.clientChart.options.hAxis.title = $scope.languages.INVOICES.CLIENT_REPORT_CHART_X_TITLE;
            } else {
                $scope.staffChart.options.title = $scope.languages.INVOICES.STAFF_REPORT_CHART_TITLE;
                $scope.staffChart.options.vAxis.title = $scope.languages.INVOICES.STAFF_REPORT_TABLE_COLUMN + ' (' + $scope.currentSymbol + ')';
                $scope.staffChart.options.hAxis.title = $scope.languages.INVOICES.STAFF_REPORT_CHART_X_TITLE;
            }
        };

        var updateClientChart = function () {
            $scope.reportObj.ByClient = true;
            salesInvoiceService.getInvoiceReportDashboard($scope.reportObj)
                .then(function (data) {
                    var obj = angular.fromJson(data);

                    if (!angular.isDefined(obj) || obj == null)
                        return false;

                    var total = obj.rows != null ? obj.rows.length : 0;

                    for (var i = 0; i < total; i++) {
                        for (var j in obj.cols) {
                            if (obj.cols[j].type === "number") {
                                obj.rows[i].c[j].v = Number(obj.rows[i].c[j].v);
                            }
                        }
                    }
                    if (obj.rows != null && obj.rows.length > 0) {
                        $scope.clientChart.data.cols = obj.cols;
                        $scope.clientChart.data.rows = obj.rows;
                    } else {
                        $scope.clientChart.data.cols = defaultCol;
                        $scope.clientChart.data.rows = null;
                    }
                });
        };

        var updateStaffChart = function () {
            $scope.reportObj.ByClient = false;
            salesInvoiceService.getInvoiceReportDashboard($scope.reportObj)
                .then(function (data) {
                    var obj = angular.fromJson(data);

                    if (!angular.isDefined(obj) || obj == null)
                        return false;

                    if (obj.rows != null) {
                        for (var i in obj.rows) {
                            for (var j in obj.cols) {
                                if (obj.cols[j].type === "number") {
                                    obj.rows[i].c[j].v = Number(obj.rows[i].c[j].v);
                                }
                            }
                        }
                    }

                    if (obj.rows != null && obj.rows.length > 0) {
                        $scope.staffChart.data.cols = obj.cols;
                        $scope.staffChart.data.rows = obj.rows;
                    } else {
                        $scope.staffChart.data.cols = defaultCol;
                        $scope.staffChart.data.rows = null;
                    }

                });
        };

        var getCurrency = function () {
            currencyService.GetAllCurrency().then(function (data) {
                $scope.listCurrency = angular.fromJson(data);

                angular.forEach($scope.listCurrency, function (value, index) {
                    if (value.BaseCurrency) {
                        $scope.reportObj.Currency = value.Id;
                        $scope.currentSymbol = value.Symbol;
                        configChart(true);
                        configChart(false);
                    }
                });
            });
        };

        var getListValues = function () {
            picklistService.GetListValueByModuleListName(invoiceModule, 'InvoiceStatus')
                .then(function (data) {
                    $scope.listValues = angular.fromJson(data);
                    //$scope.reportObj.Status = $scope.listValues[0].Id;

                    $timeout(function () {
                        $scope.updateStaff();
                        $scope.updateClient();
                    }, 500);
                });
        };



        var getSelectedUsers = function () {
            if ($scope.staffs && $scope.staffs.length > 0)
                return $scope.staffs;
            var all = [];
            for (var i = 0; i < $scope.users.length; i++) {
                var u = $scope.users[i];
                all.push(u.Id);
            }
            return all;
        };

        var getSelectedClients = function () {
            if ($scope.clients && $scope.clients.length > 0)
                return $scope.clients;
            var all = [];
            for (var i = 0; i < $scope.customers.length; i++) {
                var u = $scope.customers[i];
                all.push(u.Id);
            }
            return all;
        };

        var init = function () {
            invoiceModule = appService.getReportModuleId($routeParams.module);

            userService.getAllUserGroup().then(function (data) {
                $scope.users = angular.fromJson(data);
            });

            salesCustomerService.getAllClients().then(function (data) {
                $scope.customers = angular.fromJson(data);
            });

            $scope.staffChart = angular.copy(chart);
            $scope.clientChart = angular.copy(chart);

            getCurrentDate();
            configChart(true);
            configChart(false);
            getCurrency();
            getListValues();
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };
        $scope.opened1 = false;
        $scope.opened2 = false;
        $scope.clients = [];
        $scope.staffs = [];
        $scope.listValues = [];
        $scope.listCurrency = [];
        $scope.users = [];
        $scope.customers = [];
        $scope.currentSymbol = '';
        $scope.reportObj = {
            FromDate: '',
            ToDate: '',
            Status: 0,
            UserIds: [],
            ByClient: false,
            Currency: 0
        };

        // --- Define Scope Method. ---------------------- //

        $scope.open1 = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened1 = true;
        };

        $scope.open2 = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.opened2 = true;
        };

        $scope.updateStaff = function () {
            $scope.reportObj.UserIds = getSelectedUsers();
            updateStaffChart();
            configChart(false);
        };

        $scope.updateClient = function () {
            $scope.reportObj.UserIds = getSelectedClients();
            updateClientChart();
            configChart(true);
        };

        $scope.currencyChanged = function () {
            angular.forEach($scope.listCurrency, function (value, index) {
                if ($scope.reportObj.Currency == value.Id) {
                    $scope.currentSymbol = value.Symbol;
                }
            });
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$watch('chartType', function (newVal, oldVal) {
            if (newVal != oldVal) {
                $scope.staffChart.type = newVal;
                $scope.clientChart.type = newVal;
            }
        });

        $scope.$watch('gridLines', function (newVal, oldVal) {
            if (newVal != oldVal) {
                $scope.staffChart.options.vAxis.gridlines.count = newVal;
                $scope.clientChart.options.vAxis.gridlines.count = newVal;
            }
        });


        // --- Initialize. ---------------------------------- //
        init();
    }

})();