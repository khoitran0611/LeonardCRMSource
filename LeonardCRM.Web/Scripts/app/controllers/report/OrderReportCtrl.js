(function () {

    "use strict";

    angular.module("LeonardCRM").controller("OrderReportCtrl", ctrl);

    ctrl.$inject = ["$scope", "$routeParams", "appService", "picklistService", "registryService", "userService", "salesOrderService", "_"];

    function ctrl($scope, $routeParams, appService, picklistService, registryService, userService, salesOrderService, _) {

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
                "title": "", "gridlines": { "count": $scope.gridLines },
                "format": '#'
            },
            "hAxis": {
                "title": ""
            },
            pointSize: 4
            ,
            "tooltip": { "isHtml": true }
        };

        var orderModuleModule = 0;

        var getListValue = function () {
            picklistService.GetListValueByModuleListName(orderModuleModule, 'OrderStatus')
                .then(function (data) {
                    $scope.listValues = angular.fromJson(data);
                    $scope.reportObj.Status = $scope.listValues[0].Id;

                    $scope.updateStaff();
                });
        };

        // --- Define Controller Method. ---------------------- //

        var getCurrentDate = function () {
            var currentDate = new Date();
            $scope.reportObj.ToDate = currentDate;

            var fromDate = new Date();
            fromDate.setDate(currentDate.getDate() - 30);
            $scope.reportObj.FromDate = fromDate;
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

        var convertToPieChart = function () {
            //$scope.staffChart.
            //var cols = [{ id: "user", label: "User", type: "string" }, { id: "total", label: "Total", type: "number" }];
            //var rows = [];
            //for (var i = 1; i < $scope.staffChart.cols.length; i++) {
            //var t = 0;
            //for (var j = 0; j < $scope.staffChart.rows.length; j++) {
            //    t += $scope.staffChart.rows[j].c[i].v;
            //}
            //angular.forEach($scope.staffChart.rows, function (value, key) {
            //    var obj = value.c;
            //    for (var j = 1; i < obj.length; i++) {

            //    }
            //});
            //var row = {
            //    c : [{ v }]
            //};
            //rows.push(row);
            //}
        };

        var updateStaffChart = function () {
            salesOrderService.getOrderReportDashboard($scope.reportObj)
                .then(function (data) {
                    var obj = angular.fromJson(data);

                    if (!angular.isDefined(obj) || obj == null)
                        return false;

                    if (obj.rows != null) {
                        for (var i in obj.rows) {
                            for (var j in obj.cols) {
                                if (obj.cols[j].type == 'number') {
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
                    $scope.staffChartShadow = angular.copy($scope.staffChart);
                });
        };

        var configChart = function () {
            $scope.staffChart.options.title = $scope.languages.ORDERS.STAFF_REPORT_CHART_TITLE;
            $scope.staffChart.options.vAxis.title = $scope.languages.ORDERS.STAFF_REPORT_TABLE_COLUMN;
            $scope.staffChart.options.hAxis.title = $scope.languages.ORDERS.STAFF_REPORT_CHART_X_TITLE;
        };

        var init = function () {
            $scope.staffChart = angular.copy(chart);
            configChart();

            orderModuleModule = appService.getReportModuleId($routeParams.module);
            userService.getAllUserGroup().then(function (data) {
                $scope.users = angular.fromJson(data);
            });
            getListValue(orderModuleModule);
            getCurrentDate();

        };

        // --- Define Scope Variables. ---------------------- //

        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };
        $scope.opened1 = false;
        $scope.opened2 = false;
        $scope.reportObj = {
            FromDate: '',
            ToDate: '',
            Status: 0,
            UserIds: [],
            ByClient: false
        };
        $scope.staffs = [];
        $scope.users = [];
        $scope.pieChartSource = {};
        $scope.staffChartShadow = {};


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
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$watch('chartType', function (newVal, oldVal) {
            if (newVal != oldVal) {
                $scope.staffChart.type = newVal;
                if (newVal == 'PieChart') {
                    convertToPieChart();
                }
            }
        });

        $scope.$watch('gridLines', function (newVal, oldVal) {
            if (newVal != oldVal) {
                $scope.staffChart.options.vAxis.gridlines.count = newVal;
            }
        });

        // --- Initialize. ---------------------------------- //
        init();
    }

})();