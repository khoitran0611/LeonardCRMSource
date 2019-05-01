(function (ng) {

    'use strict';

    angular.module("LeonardCRM").factory("toolbarService", ser);

    ser.$inject = ["$rootScope", "viewService", "$window"];

    // a service for the toolbar.
    function ser($rootScope, viewService, $window) {
            var toolbarService = {};

            toolbarService.needAddCommand = false;
            toolbarService.needSaveCommand = false;
            toolbarService.needCancelCommand = true;
            toolbarService.needExportPDF = false;
            toolbarService.showAdvanceSearch = true;
            toolbarService.allowExport = false;
            toolbarService.allowImport = false;
            toolbarService.needImportData = false;
            toolbarService.needExportData = false;

            toolbarService.Add = function () {
                $rootScope.$broadcast('addEvent');
            };
            toolbarService.NeedAddCommand = function (isNeeded) {
                this.needAddCommand = isNeeded;
            };
            toolbarService.NeedSaveCommand = function (isNeeded) {
                this.needSaveCommand = isNeeded;
            };
            toolbarService.NeedCanCelCommand = function (isNeeded) {
                this.needCancelCommand = isNeeded;
            };
            toolbarService.SetupExportPDF = function (isNeeded) {
                this.needExportPDF = isNeeded;
            };
            toolbarService.setAllowImport = function (isAllowed) {
                this.allowImport = isAllowed;
            };
            toolbarService.setAllowExport = function (isAllowed) {
                this.allowExport = isAllowed;
            };
            toolbarService.Save = function () {
                $rootScope.$broadcast('saveEvent');
            };
            toolbarService.Delete = function () {
                $rootScope.$broadcast('deleteEvent');
            };
            toolbarService.Refresh = function () {
                $rootScope.$broadcast('refreshEvent');
            };
            toolbarService.ResetQuickSearch = function (isResetAdvance) {
                $rootScope.$broadcast('resetQuickSearchEvent', isResetAdvance);
            };
            toolbarService.Cancel = function () {
                toolbarService.NeedSaveCommand(false);
                $rootScope.$broadcast('cancelEvent');
            };            
            toolbarService.ShowAdvanceSearch = function (isShown) {
                this.showAdvanceSearch = isShown;
            };

            toolbarService.ExportPDF = function () {
                $rootScope.$broadcast('exportPDFEvent');
            };
            toolbarService.ItemSelected = function (checked) {
                $rootScope.$broadcast('itemSelected', checked);
            };
            toolbarService.ExportCsv = function () {
                $rootScope.$broadcast('exportCsvEvent');
            };
            toolbarService.ImportData = function () {
                $rootScope.$broadcast('importEvent');
            };
            toolbarService.ExportAllDataCsv = function () {
                $rootScope.$broadcast('exportAllDataCsvEvent');
            };
            toolbarService.setCurrentViewId = function (viewId) {
                this.currentViewId = viewId;
            };
            toolbarService.getCurrentViewId = function () {
                return this.currentViewId;
            };
            toolbarService.setCurrentModuleId = function (moduleId) {
                this.currentModuleId = moduleId;
            };
            toolbarService.getCurrentModuleId = function () {
                return this.currentModuleId;
            };

            //Events handler
            $rootScope.$on('exportCsvInfoEvent', function (event, args) {
                var key = args.Models.module;
                angular.forEach(args.Models[key], function (row, index) {
                    delete row['$$hashKey'];
                    delete row['Selected'];
                });

                viewService.ExportCsv(args)
                    .then(function (data, status, headers, config) {
                        if (data.ReturnCode == 200) {
                            location.href = data.Result;
                        } else {
                            NotifyError(data.Result, 5000);
                        }
                    });
                event.preventDefault();
            });
            $rootScope.$on('exportAllDataCsvInfoEvent', function (event, args) {
                var key = args.Models.module;
                angular.forEach(args.Models[key], function (row, index) {
                    delete row['$$hashKey'];
                    delete row['Selected'];
                });
                var pageInfo = angular.copy(args);
                pageInfo.Models = {};
                viewService.ExportAllDataCsv(pageInfo)
                    .then(function (data, status, headers, config) {
                        if (data.ReturnCode == 200) {
                            location.href = data.Result;
                        } else {
                            NotifyError(data.Result, 5000);
                        }
                    });
            });
            return toolbarService;
        }
})(angular);