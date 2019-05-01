(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ImportLeadCtrl", ctrl);

    ctrl.$inject = ["$rootScope", "$scope", "$sce", "FileUploader", "$cookieStore", "appService", "toolbarService", "viewService", "dialogService", "salesCustomerService", "filterService", "_", "previewDataService", "windowService"];

    function ctrl($rootScope, $scope, $sce, FileUploader, $cookieStore, appService, toolbarService, viewService, dialogService, salesCustomerService, filterService, _, previewDataService, windowService) {
        function clearForm() {
            $scope.fileName = '';
            $scope.uploader.clearQueue();
            $scope.errorMessage = '';
        }

        var getSheetNamesCallback = function (data) {
            $scope.sheetNames = angular.fromJson(data);
            if ($scope.sheetNames.length > 0) {
                $scope.currentSheetName = $scope.sheetNames[0];
            }
        };

        var getAllParsersCallback = function (data) {
            $scope.parserSource = angular.fromJson(data);
            if ($scope.parserSource.length > 0) {
                $scope.parserId = $scope.parserSource[0].Id;
            }
           
        };

        var importDataCallback = function (data) {
            $scope.ResultMsg = '';
            if (data.ReturnCode == 200) {
                if ($scope.parserId != 3) {
                    $scope.showMsg = true;
                    $scope.isSuccess = true;
                    $scope.isError = false;
                    $scope.isValid = false;
                    $scope.is_injecting = false;
                    $scope.ResultMsg = data.Result;
                    $scope.showInjectionButton = false;
                } else {

                    var index = $scope.pageIndexSource - 1;
                    for (var i = 0; i < $scope.pageSizeSource; i++) {
                        if (index < $scope.parserData.ReturnData[0].length) {
                            var appObj = $scope.parserData.ReturnData[0][index];
                            $scope.AppResult.push({ msg: appObj.AppName });
                            index++;
                        }
                    }

                    $scope.pageIndexSource += $scope.pageSizeSource; // skip pagesize

                    if ($scope.pageIndexSource < $scope.parserData.ReturnData[0].length) {
                        $scope.injectDataClick();
                    } else {
                        $scope.AppResult.push({ msg: $scope.languages.DATA_INJECTION.END_PROCESSED_TITLE_MSG });
                        $scope.ResultMsg = $scope.languages.DATA_INJECTION.COUNT_APP_PROCESSED_TITLE_MSG.replace('{0}', $scope.parserData.ReturnData[0].length);
                        $scope.showMsg = true;
                        $scope.isSuccess = true;
                        $scope.isValid = false;
                        $scope.is_injecting = false;
                        $scope.showInjectionButton = false;
                        $scope.iswaiting = false;
                    }
                }
            } else {
                $scope.showMsg = true;
                $scope.isSuccess = false;
                $scope.isError = true;
                $scope.isValid = false;
                $scope.is_injecting = false;
                $scope.iswaiting = false;
                if ($scope.parserId == 3) {
                    $scope.AppResult.push({ msg: $scope.languages.DATA_INJECTION.CANCELED_INJECTION });
                }
                $scope.ResultMsg = data.Result;
            }
        };

        var columnMappingCallback = function (data) {
            $scope.resetValidate();
            $scope.columnMappingSource = angular.fromJson(data);
            if ($scope.parserId != 3 && $scope.columnMappingSource.length < 1) {
                $scope.isMappingError = true;
                $scope.ResultMsg = $scope.languages.SALES_CUSTOMER.NOT_FOUND_COLUMN_MAP_MSG;
            }
        };

        var dataInjectionGetFieldDatatypeCallback = function (data) {
            $scope.entityFieldSources = angular.fromJson(data);
        };

        var validateExcelFileCallback = function (data) {
            $scope.isInjecting = false;
            $scope.AppResult = [];

            $scope.parserData = angular.fromJson(data);

            var resultObj = $scope.parserData.ResultObj;
            if (resultObj.ReturnCode == 200) {
                $scope.pickLists = filterService.getPickList($scope.ModuleId);
                $scope.referenceLists = filterService.getReferenceList($scope.ModuleId);
                $scope.ResultMsg = resultObj.Result;

                $scope.showMsg = true;
                $scope.isSuccess = true;
                $scope.isValid = true;
                $scope.isError = false; // error on
                $scope.showInjectionButton = true;
                // Send Data to Service
                var dynamicObj = {
                    ParserData: $scope.parserData,
                    EntiyField: $scope.entityFieldSources,
                    PickList: $scope.pickLists,
                    ReferenceList: $scope.referenceLists
                };
                previewDataService.bindingData(dynamicObj);

                if ($scope.parserId != 3 && $scope.parserData.ReturnData.length > 0) {
                    $scope.showInjectionButton = true;
                }

            } else {
                $scope.showMsg = true; // show block
                $scope.isSuccess = false; // success off
                $scope.isValid = false; // button off
                $scope.isError = true; // error on
                $scope.ResultMsg = resultObj.Result; // assign value
            }
            $scope.is_validating = false;
        };

        var getSheetName = function () {
            salesCustomerService.getSheetNames($scope.fileName)
                .then(getSheetNamesCallback);
        };

        var getAllParsers = function () {
            salesCustomerService.getAllParsers($scope.ModuleId)
                .then(getAllParsersCallback);
        };

        //Upload File
        //$scope.uploader = $fileUploader.create({
        //    scope: $scope,
        //    url: $scope.UploadHandlerUrl + '?folder=customers',
        //    onBeforeUploadItem: function () {
        //        $scope.uploader.clearQueue();
        //    },
        //    filters: [
        //        function (item) {
        //            var extension = item.name.split('.').pop();
        //            if ($.inArray(extension, ['xls', 'xlsx', 'csv', 'XLS', 'XLSX', 'CSV']) > -1) {
        //                $scope.isProcess = true;
        //                return true;
        //            }

        //            //error msg
        //            NotifyError($scope.languages.APPOINTMENT.IMPORT_IMAGE_FORMAT);
        //            return false;
        //        }
        //    ]
        //});

        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            url: $scope.UploadHandlerUrl + '?folder=customers'
        });

        // ADDING FILTERS
        uploader.filters.push({
            name: 'filterName',
            fn: function (item) {
                    var extension = item.name.split('.').pop();
                    if ($.inArray(extension, ['xls', 'xlsx', 'csv', 'XLS', 'XLSX', 'CSV']) > -1) {
                        $scope.isProcess = true;
                        return true;
                    }

                    //error msg
                    NotifyError($scope.languages.APPOINTMENT.IMPORT_IMAGE_FORMAT);
                    return false;
                }
        });

        //$scope.uploader.bind('success', function (event, xhr, item, response) {
        //    var fileList = response;
        //    if (fileList.length > 0) {
        //        var res = fileList[0];

        //        $scope.fileName = res.name; 
        //        $scope.isProcess = false;

        //        // reset default
        //        $scope.columnMappingSource = [];
        //        $scope.sheetNames = [];
        //        $scope.isShowImport = true;
        //        $scope.showInjection = true;

        //        // get Sheet Name
        //        getSheetName();
        //    }
        //});

        uploader.onSuccessItem = function (item) {
            $scope.uploader.clearQueue();
        };
        uploader.onSuccessItem = function (item, response, status, headers) {
            var fileList = response;
            if (fileList.length > 0) {
                var res = fileList[0];

                $scope.fileName = res.name;
                $scope.isProcess = false;

                // reset default
                $scope.columnMappingSource = [];
                $scope.sheetNames = [];
                $scope.isShowImport = true;
                $scope.showInjection = true;

                // get Sheet Name
                getSheetName();
            }
        };

        // Get the render context local to this controller (and relevant params).
        function processImport(args) {
            var importInfo = angular.copy(args);
            viewService.ImportDataToServer(importInfo, $scope.ModuleId).then(function (data, status, headers, config) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                    $scope.CloseDialog();
                    $rootScope.$broadcast('refreshEvent');
                    $scope.CloseDialog();
                } else {
                    $scope.errorMessage = data.Result;
                }
            });
        }

        // --- Define Scope Variables. ---------------------- //
        $scope.fileName = '';
        $scope.errorMessage = '';
        $scope.isProcess = true;
        // reset default
        $scope.columnMappingSource = [];
        $scope.sheetNames = [];
        $scope.currentSheetName = '';
        $scope.isShowImport = false;
        $scope.parserSource = [];
        $scope.parserId = 1;
        $scope.columnNameRowIndex = 1;
        $scope.sheetNames = [];
        $scope.fileName = '';
        $scope.columnMappingSource = [];
        $scope.entityFieldSources = [];
        $scope.parserData = {};
        $scope.EmptyRowAction = 0;
        $scope.InvalidCellDataAction = 1;

        $scope.isSuccess = false;
        $scope.isError = false;
        $scope.isValid = false;
        $scope.isChangeColumn = false;
        $scope.is_validating = false;
        $scope.is_injecting = false;
        $scope.isMappingError = false;
        $scope.ResultMsg = '';
        $scope.showMsg = false;

        $scope.showInjection = false;
        $scope.whatthisPopover = $scope.languages.SALES_CUSTOMER.WHAT_THIS_NOTE;
        $scope.pageIndexSource = 1;
        $scope.pageSizeSource = 2;
        $scope.isInjecting = false;
        $scope.pickLists = [];
        $scope.referenceLists = [];

        $scope.iswaiting = false;
        $scope.showInjectionButton = false;

        // --- Define Scope Method. ---------------------- //
        $scope.Import = function () {
            var option = {
                ModuleId: $scope.ModuleId,
                FileName: $scope.fileName
            };
            if ($scope.fileName == '') {
                NotifyError($scope.languages.TOOLBAR.IMPORT_NO_FILE_SELECTED_MESSAGE);
                return false;
            }
            processImport(option);
        };

        $scope.CloseDialog = function () {
            clearForm();
            $('#dialogDetail').dialog('close');
        };

        $scope.RawHtml = function (str) {
            return $sce.trustAsHtml(str);
        };

        $scope.intPage = function () {
            $scope.setTitle($scope.languages.SALES_CUSTOMER.IMPORT_TITLE);
            $scope.setSaveButtonVisible(false);
            getAllParsers();
        }

        $scope.columnMapping = function () {
            $scope.isValid = false;
            $scope.isMappingError = false;
            var parser = _.where($scope.parserSource, { 'Id': $scope.parserId });
            if (parser != null) {
                $scope.parserData.ModuleId = parser[0].ModuleId;

                var parserData = {
                    FileName: $scope.fileName,
                    CurrentSheetName: $scope.currentSheetName,
                    ColumnNameRowIndex: $scope.columnNameRowIndex,
                    ParserId: $scope.parserId
                };

                // Get Column Map
                salesCustomerService.columnMapping(parserData, $scope.ModuleId)
                    .then(columnMappingCallback);

                // get ListNameValue & ForiegnKey
                filterService.getPickListByModule(parser[0].ModuleId);

                // Get Entity Field
                salesCustomerService.dataInjectionGetFieldDatatype(parser[0].ModuleId)
                .then(dataInjectionGetFieldDatatypeCallback);
            }
        };

        $scope.resetValidate = function () {
            $scope.isChangeColumn = true;
            $scope.isSuccess = false;
            $scope.isError = false;
            $scope.ResultMsg = '';
            $scope.isValid = false;

            $scope.pageIndexSource = 1;
            $scope.pageSizeSource = 2;
            $scope.isInjecting = false;
        };

        $scope.ValidateExcelFile = function () {
            $scope.resetValidate();
            $scope.is_validating = true; // show button state

            var colMaps = _.filter($scope.columnMappingSource, function (item) {
                return true; //item.ObjectColumnName != undefined && item.ObjectColumnName.length > 0;
            });
            var parserData = {
                FileName: $scope.fileName,
                CurrentSheetName: $scope.currentSheetName,
                ColumnNameRowIndex: $scope.columnNameRowIndex,
                ColumnMaps: colMaps,
                ParserId: $scope.parserId,
                ModuleId: $scope.parserData.ModuleId,
                EmptyRowAction: $scope.EmptyRowAction,
                InvalidCellDataAction: $scope.InvalidCellDataAction
            };
            salesCustomerService.validateExcelFile(parserData, $scope.ModuleId)
                .then(validateExcelFileCallback);
        };

        $scope.previewDataClick = function () {

            // Show Preview Dialog
            var param = {
                Url: '/appviews/customer/previewDataDialog.html',
                Id: 555,
                ParentId: 555,
                ViewId: 555,
                ModuleId: 555,
                Key: 555
            };

            windowService.openWindow(param);
        };

        $scope.injectDataClick = function () {
            $scope.is_injecting = true;
            $scope.showMsg = false;
            $scope.isValid = true;
            $scope.isSuccess = true;
            $scope.isError = false;

            var parserData = {};
            if ($scope.parserId != 3) {
                parserData = {
                    ParserId: $scope.parserId,
                    ReturnData: $scope.parserData.ReturnData,
                    ModuleId: $scope.parserData.ModuleId,
                    ColumnMaps: $scope.parserData.ColumnMaps
                };
            } else {
                $scope.isInjecting = true;
                $scope.iswaiting = true;

                // Process $scope.parserData.ReturnData[0]
                var index = $scope.pageIndexSource - 1;
                var isFirst = false;
                var totalRow = $scope.parserData.ReturnData[0].length;

                if (index == 0) {
                    isFirst = true;
                }

                var objsSource = [];
                for (var i = 0; i < $scope.pageSizeSource; i++) {
                    if (index < $scope.parserData.ReturnData[0].length) {
                        objsSource.push($scope.parserData.ReturnData[0][index]);
                        index++;
                    }
                }

                parserData = {
                    CurrentIndex: index,
                    IsFirst: isFirst,
                    TotalRow: totalRow,
                    ParserId: $scope.parserId,
                    ReturnData: objsSource,
                    ModuleId: $scope.parserData.ModuleId,
                    ColumnMaps: $scope.parserData.ColumnMaps
                };
            }


            salesCustomerService.importData(parserData, $scope.ModuleId)
                .then(importDataCallback);
        };

        $scope.showRecurrence = function (flag) {
            if (flag) {
                $('#popUpPreviewData').appendTo("body").modal({
                    show: true,
                    keyboard: false,
                    backdrop: 'static'
                });
            } else {
                $('#popUpPreviewData').modal('hide');
            }

        }
         //--- Bind To Scope Events. ------------------------ //

         //I handle changes to the request context.
        $scope.$watch('columnMappingSource', function (newvalue, oldvalue) {
            if ($scope.isChangeColumn) {
                $scope.resetValidate();
            }
        }, true);
        $scope.$on(
            "requestContextChanged",
            function () {
                if ($('#dialogDetail').length > 0 && $('#dialogDetail').hasClass('ui-dialog-content')) {
                    $('#dialogDetail').dialog('destroy').remove();
                }

            }
        );

        // --- Initialize. ---------------------------------- //
        $scope.intPage();
    }

})(angular);
