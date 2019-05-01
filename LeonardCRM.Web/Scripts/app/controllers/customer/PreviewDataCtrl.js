(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("previewDataCtrl", ctrl);

    ctrl.$inject = ["$scope", "$sce", "previewDataService", "_"];

    function ctrl($scope, $sce, previewDataService, _) {

        // --- Define Controller Variables. ---------------------- //

        var cellHeadertemplate = '<th><div class="row"><div class="col-xs-12 text-center"><span>[string]<span></div></div></th>';
        var cellBodytemplate = '<td><div class="row"><div class="col-xs-12"><span>[string]<span></div></div></td>';

        var appCellHeaderTemplate1 = '<th colspan="2" class="appdatacellbody">[string]</th>';
        var cellRotateHeadertemplate = '<th class="throtate"><div><span>[string]</span></div></th>';
        var appCellBodytemplate = '<td class="appdatacellbody">[string]</td>';
        var appCellHeadertemplate = '<th class="appdatacellbody">[string]</td>';
        var appLanguageCellBodytemplate = '<td class="languagecellbody">[string]</td>';
        var appLanguageIsoCellBodytemplate = '<td class="languageisocellbody">[string]</td>';
        var cell3ColSpan = '<th colspan="3"><div class="row"><div class="col-xs-12"><span>[string]<span></div></div></th>';

        // --- Define Controller Method. ---------------------- //

        var toHtml = function (str) {
            var output = '';
            if (str != null) {
                var pattern = '\n';
                var reg = new RegExp(pattern, 'g');
                output = str.replace(reg, '<br/>');
            }
            return output;
        };

        var createColumnNameHeader = function () {
            $scope.columns = $scope.gridDataResource.ParserData.ColumnMaps;
            $scope.entityFields = angular.copy($scope.gridDataResource.EntiyField);
            var htmlString = '<thead><tr>';

            // .No Column
            htmlString += cellHeadertemplate.replace('[string]', $scope.languages.SALES_CUSTOMER.INDEX_COLUMN);

            angular.forEach($scope.columns, function (colObj, key) {
                var cols = _.filterWithProperty($scope.entityFields, 'FieldName', colObj.ObjectColumnName);
                if (cols.length > 0) {
                    var htmlStr = cellHeadertemplate.replace('[string]', cols[0].LabelDisplay);
                    htmlString += htmlStr;
                }
            });
            htmlString += '</tr></thead>';
            return htmlString;
        };

        var createTableBody = function () {
            $scope.dataSource = $scope.gridDataResource.ParserData.ReturnData;
            // build template
            var htmlString = '<tbody>';
            $scope.totalDuplicated = 0;
            angular.forEach($scope.dataSource, function (entity, index) {
                // data row
                if (entity.IsDuplicated) {
                    htmlString += '<tr class="danger">';
                } else {
                    htmlString += '<tr>';
                }
                if (entity.IsDuplicated)
                    $scope.totalDuplicated++;
                // .No Column
                htmlString += cellBodytemplate.replace('[string]', index + 1);
                // print entity
                angular.forEach($scope.columns, function (column, i) {
                    // check Property data type
                    var cols = _.filterWithProperty($scope.entityFields, 'FieldName', column.ObjectColumnName);
                    if (cols.length > 0) {
                        var col = cols[0];
                        var cellData = '';
                        var listTemp;
                        var temp;
                        var j = 0;
                        var arry = new Array();
                        if (!col.Deletable) {
                            if (col.FieldName == 'ResponsibleUsers') {
                                arry = entity[col.FieldName].toString().split(",");
                                listTemp = _.filterWithProperty($scope.refList, 'FieldName', col.FieldName);
                                for (j = 0; j < listTemp.length; j++) {
                                    temp = _.filterWithProperty(listTemp, 'Id', parseInt(arry[j]));
                                    if (temp.length > 0) {
                                        for (var t = 0; t < temp.length; t++) {
                                            cellData = temp[t].Description + ',';
                                        }
                                    }
                                }
                                if (cellData != '') cellData = cellData.substring(0, cellData.length - 1);
                            } else {
                                if (col.FieldName == 'ModifiedBy' || col.FieldName == 'CreatedBy') {
                                    arry = entity[col.FieldName].toString().split(",");
                                    listTemp = _.filterWithProperty($scope.referenceLists, 'FieldName', col.FieldName);
                                    for (j = 0; j < listTemp.length; j++) {
                                        temp = _.filterWithProperty(listTemp, 'Id', parseInt(arry[j]));
                                        if (temp.length > 0) {
                                            for (var t = 0; t < temp.length; t++) {
                                                cellData = temp[t].Description + ',';
                                            }
                                        }
                                    }
                                    if (cellData != '') cellData = cellData.substring(0, cellData.length - 1);
                                } else {

                                    cellData = entity[col.FieldName];

                                }

                            }

                        } else {

                            var custFields = entity.FieldData;

                            for (var i = 0; i < custFields.length; i++) {
                                var field = custFields[i];
                                arry.push(field.FieldData);
                            }
                            // check list
                            if (col.IsList == true && col.ForeignKey != true) {
                                listTemp = _.filterWithProperty($scope.pickList, 'FieldName', col.FieldName);
                                if (listTemp.length > 0) {
                                    for (j = 0; j < arry.length; j++) {
                                        temp = _.filterWithProperty(listTemp, 'Id', parseInt(arry[j]));
                                        if (temp.length > 0) {
                                            for (var t = 0; t < temp.length; t++) {
                                                cellData = temp[t].Description + ',';
                                            }
                                        }
                                    }
                                    if (cellData != '') cellData = cellData.substring(0, cellData.length - 1);
                                } else {
                                    cellData = '';
                                }
                            }
                                // check list & foreignkey
                            else if (col.IsList == true && col.ForeignKey == true) {
                                listTemp = _.filterWithProperty($scope.refList, 'FieldName', col.FieldName);
                                if (listTemp.length > 0) {
                                    temp = _.filterWithProperty(listTemp, 'Id', cellData);
                                    for (j = 0; j < arry.length; j++) {
                                        if (temp.length > 0) {
                                            for (var t = 0; t < temp.length; t++) {
                                                cellData = temp[t].Description + ',';
                                            }
                                        }
                                    }
                                    if (cellData != '') cellData = cellData.substring(0, cellData.length - 1);
                                } else {
                                    cellData = '';
                                }
                            }
                                // check multiselectbox
                            else if (col.IsMultiSelecttBox == true && col.ForeignKey != true) {
                                listTemp = _.filterWithProperty($scope.pickList, 'FieldName', col.FieldName);
                                if (listTemp.length > 0 && cellData != undefined && cellData.length > 0) {
                                    temp = cellData.substring(0, cellData.length - 1).split(',');
                                    if (temp.length > 0) {
                                        var str = '';
                                        angular.forEach(temp, function (objId, key) {
                                            var obj = _.filterWithProperty(listTemp, 'Id', parseInt(objId));
                                            str += obj[0].Description + ', ';
                                        });
                                        cellData = str.substring(0, str.length - 1);
                                    }
                                } else {
                                    cellData = '';
                                }
                            }
                                // check multiselectbox & foreignkey
                            else if (col.IsMultiSelecttBox == true && col.ForeignKey == true) {
                                listTemp = _.filterWithProperty($scope.refList, 'FieldName', col.FieldName);
                                if (listTemp.length > 0 && cellData != undefined && cellData.length > 0) {
                                    temp = cellData.substring(0, cellData.length - 1).split(',');
                                    if (temp.length > 0) {
                                        str = '';
                                        angular.forEach(temp, function (objId, key) {
                                            var obj = _.filterWithProperty(listTemp, 'Id', parseInt(objId));
                                            str += obj[0].Description + ', ';
                                        });
                                        cellData = str.substring(0, str.length - 1);
                                    }
                                } else {
                                    cellData = '';
                                }
                            }
                                // replace \n to br tag
                            else {
                                cellData = toHtml(cellData);
                            }
                        }

                        if (cellData == undefined || cellData == null)
                            cellData = '';
                        var htmlStr = cellBodytemplate.replace('[string]', cellData);
                        htmlString += htmlStr;

                    }
                });
                htmlString += '</tr>';
            });
            htmlString += '</tbody>';
            return htmlString;
        };

        var createTable = function () {
            var htmlString = '<table class="table table-bordered">';
            var headerTable = createColumnNameHeader();
            var bodyTable = createTableBody();

            htmlString += headerTable + bodyTable;

            htmlString += '</tbody>';
            return htmlString;
        };

        // ============================[Begin Release App]============================ //

        var createAppHeaderRow = function () {
            var htmlString = '<thead><tr>';

            // add three empty Columns
            htmlString += cell3ColSpan.replace('[string]', '&nbsp;');

            // add App name is HeaderColumn & colspan:2
            angular.forEach($scope.appsSource, function (appObj, key) {
                var htmlStr = appCellHeaderTemplate1.replace('[string]', appObj.AppName);
                htmlString += htmlStr;
            });
            htmlString += '</tr>';
            return htmlString;
        };

        var createHeaderColumn = function () {
            var htmlString = '<tr>';

            // add three columns of Country
            htmlString += appCellHeadertemplate.replace('[string]', $scope.languages.SALES_CUSTOMER.COUNTRY_COLUMN);
            htmlString += appCellHeadertemplate.replace('[string]', $scope.languages.SALES_CUSTOMER.ISO_COLUMN);
            htmlString += appCellHeadertemplate.replace('[string]', $scope.languages.SALES_CUSTOMER.ISO_MARKET_COLUMN);

            // add HeaderColumn
            angular.forEach($scope.appsSource, function (appObj, key) {
                var htmlStr = cellRotateHeadertemplate.replace('[string]', $scope.languages.SALES_CUSTOMER.SERVICE_COLUMN);
                htmlString += htmlStr;

                htmlStr = cellRotateHeadertemplate.replace('[string]', $scope.languages.SALES_CUSTOMER.MARKET_COLUMN);
                htmlString += htmlStr;
            });
            htmlString += '</tr></thead>';
            return htmlString;
        };

        var createReleaseAppBodyTable = function () {
            // build template
            var htmlString = '<tbody>';

            angular.forEach($scope.countrySource, function (countryObj, countryIndex) {
                // start data row
                htmlString += '<tr>';

                htmlString += appLanguageCellBodytemplate.replace('[string]', countryObj.CountryName);
                htmlString += appLanguageIsoCellBodytemplate.replace('[string]', countryObj.ISOCode);
                htmlString += appLanguageIsoCellBodytemplate.replace('[string]', countryObj.ISOMarketCode);

                angular.forEach($scope.appsSource, function (appObj, appIndex) {
                    $scope.appsCountrySource = angular.copy(appObj.AppCountries);
                    var tempObjs = _.filter($scope.appsCountrySource, { 'CountryID': countryObj.Id, 'AppID': appObj.Id });
                    if (tempObjs.length > 0) {
                        var tempObj = tempObjs[0];
                        var t = tempObj.ServiceAvail ? 'X' : '&nbsp;';
                        htmlString += appCellBodytemplate.replace('[string]', t);
                        t = tempObj.MarketAvail ? 'X' : '&nbsp;';
                        htmlString += appCellBodytemplate.replace('[string]', t);
                    } else {
                        htmlString += appCellBodytemplate.replace('[string]', '&nbsp;');
                        htmlString += appCellBodytemplate.replace('[string]', '&nbsp;');
                    }
                });

                // end data row
                htmlString += '</tr>';
            });

            htmlString += '</tbody>';
            return htmlString;
        };

        var createReleaseAppTable = function () {
            var htmlString = '<table class="table table-bordered">';
            var headerTable = createAppHeaderRow();
            headerTable += createHeaderColumn();
            var bodyTable = createReleaseAppBodyTable();

            htmlString += headerTable + bodyTable;
            return htmlString;
        };

        // ============================[End Release App]============================ //

        var initPreviewData = function () {
            $scope.isShowPreviewData = true;
            $scope.setSaveButtonVisible(false);
            previewDataService.loadData();
        };

        // --- Define Scope Variables. ---------------------- //

        $scope.isShowPreviewData = false;
        $scope.gridHtml = '';
        $scope.gridDataResource = {};
        $scope.columns = [];
        $scope.entityFields = [];
        $scope.dataSource = [];
        $scope.pickList = [];
        $scope.refList = [];

        $scope.appsSource = [];
        $scope.appsCountrySource = [];
        $scope.countrySource = [];
        //$scope.ParserID = 0;
        $scope.totalDuplicated = 0;
        // --- Define Scope Method. ---------------------- //

        $scope.RawHtml = function (str) {
            return $sce.trustAsHtml(str);
        }

        $scope.showLanguages = function (languageIdsArray) {
            var languages = _.where($scope.pickList, { 'FieldName': 'Languages', 'ModuleId': 3 });
            var langStr = '';
            angular.forEach(languageIdsArray, function (langId, index) {
                var res = _.where(languages, { 'Id': langId });
                if (res.length > 0) {
                    langStr += res[0].Description + '<br/>';
                }
            });
            return $scope.RawHtml(langStr);
        };

        $scope.showGridView = function () {
            var htmlString = createTable();
            $scope.gridHtml = $scope.RawHtml(htmlString);
        };

        $scope.showReleaseApp = function () {
            $scope.appsSource = angular.copy($scope.gridDataResource.ParserData.ReturnData[0]);
            $scope.countrySource = angular.copy($scope.gridDataResource.ParserData.ReturnData[1]);
            var htmlString = createReleaseAppTable();
            $scope.gridHtml = $scope.RawHtml(htmlString);
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('bindingDataEvent', function (event, args) {
            // handle event only if it was not defaultPrevented
            if (event.defaultPrevented) {
                return;
            }
            // mark event as "not handle in children scopes"
            event.preventDefault();
            // Process
            $scope.gridDataResource = angular.copy(args);
            $scope.pickList = $scope.gridDataResource.PickList;
            $scope.refList = $scope.gridDataResource.ReferenceList;
            if ($scope.gridDataResource.ParserData.ParserId != 3) {
                $scope.showGridView();
            } else {
                $scope.showReleaseApp();
            }
        });

        // --- Initialize. ---------------------------------- //
        initPreviewData();
    }

})(angular);
