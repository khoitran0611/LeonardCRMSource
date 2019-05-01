(function () {
    "use strict";

    angular.module("LeonardCRM").controller("customerReportCtrl", ctrl);

    ctrl.$inject = ["$scope", "requestContext", "appService", "roleService", "salesCustomerService"];

    function ctrl($scope, requestContext, appService, roleService, salesCustomerService) {
        var chart1 = {};
        chart1.type = $scope.chartType;
        chart1.displayed = false;
        chart1.cssStyle = "height:500px; width:100%;";
        chart1.data = {
            "cols": [
                { id: "empl", label: $scope.languages.SALES_CUSTOMER.REPORT_CHART_X_TITLE, type: "string" },
                { id: "total", label: $scope.languages.SALES_CUSTOMER.OVERVIEW_CHART_Y_TITLE, type: "number" },
                { id: "tip", label: $scope.languages.SALES_CUSTOMER.REPORT_TABLE_COLUMN, "role": "tooltip", "type": "string", "p": { "role": "tooltip", 'html': true } }
            ],
            "rows":[]
        };

        chart1.options = {
            "title": $scope.languages.SALES_CUSTOMER.REPORT_CHART_TITLE,
            "isStacked": "false",
            "legend": { position: 'none' },
            "fill": 20,
            "displayExactValues": true,
            "vAxis": {
                "title": $scope.languages.SALES_CUSTOMER.OVERVIEW_CHART_Y_TITLE, "gridlines": { "count": $scope.gridLines },
                "format": '#'
            },
            "hAxis": {
                "title": $scope.languages.SALES_CUSTOMER.REPORT_CHART_X_TITLE
            },
            pointSize: 4
            ,
            "tooltip": { "isHtml": true },
            'showRowNumber': true,
            'allowHtml': true
        };
        
        var formatCollection = [
            //{
            //    name: "color",
            //    format: [
            //        {
            //            columnNum: 4,
            //            formats: [
            //                {
            //                    from: 0,
            //                    to: 3,
            //                    color: "white",
            //                    bgcolor: "red"
            //                },
            //                {
            //                    from: 3,
            //                    to: 5,
            //                    color: "white",
            //                    fromBgColor: "red",
            //                    toBgColor: "blue"
            //                },
            //                {
            //                    from: 6,
            //                    to: null,
            //                    color: "black",
            //                    bgcolor: "#33ff33"
            //                }
            //            ]
            //        }
            //    ]
            //},
            //{
            //    name: "arrow",
            //    checked: false,
            //    format: [
            //        {
            //            columnNum: 1,
            //            base: 19
            //        }
            //    ]
            //},
            //{
            //    name: "date",
            //    format: [
            //        {
            //            columnNum: 5,
            //            formatType: 'long'
            //        }
            //    ]
            //},
            //{
            //    name: "number",
            //    format: [
            //        {
            //            columnNum: 4,
            //            prefix: '$'
            //        }
            //    ]
            //},
            //{
            //    name: "bar",
            //    format: [
            //        {
            //            columnNum: 1,
            //            width: 100
            //        }
            //    ]
            //}
        ];

        //chart1.formatters = {};

        $scope.users = [];

        roleService.getRoleUserHierachy().then(function (data) {
            $scope.users = angular.fromJson(data).results;
            $scope.updateChart();
        });

        $scope.chart = chart1;

        var getSelectedUsers = function() {
            if ($scope.selectedUsers && $scope.selectedUsers.length > 0)
                return $scope.selectedUsers;
            var all = [];
            for (var i = 0; i < $scope.users.length; i++) {
                var u = $scope.users[i];
                for (var j = 0; j < u.children.length; j++) {
                    all.push(u.children[j].id);
                }
            }
            return all;
        };

        $scope.updateChart = function () {
            var employees = getSelectedUsers();
            salesCustomerService.getReportDataByUsers(employees).then(function (data) {
                $scope.chart.data.rows = angular.fromJson(data).rows;
                for (var i in $scope.chart.data.rows) {
                    for (var j in $scope.chart.data.cols) {
                        if ($scope.chart.data.cols[j].type == "number") {
                            $scope.chart.data.rows[i].c[j].v = Number($scope.chart.data.rows[i].c[j].v);
                        }
                    }
                }
            });
        };

        $scope.$watch('chartType', function (newVal, oldVal) {
            if (newVal != oldVal) {
                $scope.chart.type = newVal;
                $scope.chartSelectionChange();
            }
        });
        
        $scope.$watch('gridLines', function (newVal, oldVal) {
            if (newVal != oldVal)
                $scope.chart.options.vAxis.gridlines.count = newVal;
        });

        $scope.chartSelectionChange = function () {
            if ($scope.chart.type === 'PieChart') {
                $scope.chart.options.legend = { position: 'right' };
            } else {
                $scope.chart.options.legend = { position: 'none' };
            }
        };

        //$scope.formatCollection = formatCollection;
        //$scope.toggleFormat = function (format) {
        //    $scope.chart.formatters[format.name] = format.format;
        //};

        $scope.chartReady = function () {
            fixGoogleChartsBarsBootstrap();
        };

        function fixGoogleChartsBarsBootstrap() {
            // Google charts uses <img height="12px">, which is incompatible with Twitter
            // * bootstrap in responsive mode, which inserts a css rule for: img { height: auto; }.
            // *
            // * The fix is to use inline style width attributes, ie <img style="height: 12px;">.
            // * BUT we can't change the way Google Charts renders its bars. Nor can we change
            // * the Twitter bootstrap CSS and remain future proof.
            // *
            // * Instead, this function can be called after a Google charts render to "fix" the
            // * issue by setting the style attributes dynamically.

            $(".google-visualization-table-table img[width]").each(function (index, img) {
                $(img).css("width", $(img).attr("width")).css("height", $(img).attr("height"));
            });
        };        
    }

})();