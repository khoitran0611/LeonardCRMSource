(function () {

    "use strict";

    angular.module("LeonardCRM").controller("toolbarCtrl", ctrl);

    ctrl.$inject = ["$scope", "$rootScope", "$filter", "toolbarService", "filterService", "requestContext"];

    function ctrl($scope, $rootScope, $filter, toolbarService, filterService, requestContext) {
        $scope.isAddNew = false;
        $scope.isEdit = false;
        $scope.isDelete = false;
        $scope.isExportPDF = false;
        $scope.service = toolbarService;
        $scope.showAdvanceSearch = true;
        $scope.showCancelButton = true;
        $scope.showAddButton = true;
        $scope.windowOpen = false;
        $scope.allowImport = false;
        $scope.allowExport = false;        
        
        // I handle changes to the module.
        $scope.$watch('service.needCancelCommand', function (newVal) {
            if (newVal != $scope.showCancelButton)
                $scope.showCancelButton = newVal;
        });
        $scope.$watch('service.needAddCommand', function (newVal, oldVal) {
            if (newVal != oldVal)
                $scope.showAddButton = newVal;
        });
        $scope.$watch('service.needSaveCommand', function (newVal) {
            if (newVal != $scope.isAddNew || newVal != $scope.isEdit)
                $scope.isAddNew = $scope.isEdit = newVal;
        });
        $scope.$watch('service.showAdvanceSearch', function (newVal) {
            if (newVal != $scope.showAdvanceSearch)
                $scope.showAdvanceSearch = newVal;
        });
        $scope.$watch('service.needExportPDF', function (newVal) {
            if (newVal != $scope.isExportPDF)
                $scope.isExportPDF = newVal;
        });
        $scope.$watch('service.allowImport', function (newVal) {
            if (newVal != $scope.allowImport)
                $scope.allowImport = newVal;
        });

        $scope.$watch('service.allowExport', function (newVal) {
            if (newVal != $scope.allowExport)
                $scope.allowExport = newVal;
        });

        $scope.$on(
            "itemSelected",
            function (e, checked) {
                $scope.isDelete = checked;
            }
        );
        $scope.$on("openWindow", function (e, param) {
            $scope.windowOpen = true;
        });
        $scope.$on("windowClosed", function (event, args) {
            if (args == 0) {
                $scope.windowOpen = false;
            }
        });
        // --- Define events for toolbar buttons ------------------------ //

        $scope.Add = function () {
            toolbarService.Add();
        };
        $scope.Save = function () {
            toolbarService.Save();
        };
        $scope.Delete = function () {
            toolbarService.Delete();
        };
        $scope.Refresh = function () {
            filterService.resetAllConditions(toolbarService.getCurrentViewId());
            toolbarService.Refresh();
        };
        $scope.Cancel = function () {
            toolbarService.Cancel();
            $scope.isEdit = false;
        };

        $scope.ExportPDF = function () {
            toolbarService.ExportPDF();
        };
        $scope.ExportCSV = function () {
            toolbarService.ExportCsv();
        };
        $scope.ImportDataToDB = function () {
            toolbarService.ImportData();
        };
        $scope.ExportFromDbToCSV = function () {
            toolbarService.ExportAllDataCsv();
        };

        // Quick search
        $scope.textSearch = '';
        $scope.columns = filterService.getFields(toolbarService.getCurrentModuleId());
        $rootScope.$on('entityFieldsLoaded', function () {
            $scope.columns = filterService.getFields(toolbarService.getCurrentModuleId());
        });
        
        $scope.createSearchServerConditions = function (textSearch) {
            var first = true;
            var orCondition = [];
            var andCondition = [];
            var currentVisCols = filterService.getCurrentGridColumns();

            angular.forEach($scope.columns, function (key, index) {
                //detect if the column is showed on the current view
                var existOnView = $filter('filter')(currentVisCols, { ColumnName: key.ColumnName }).length > 0;

                if (existOnView == true &&  
                    key.AdvanceSearch == true && //showed on view and searchable                   
                    key.IsCheckBox != true && //not appy for checkbox                    
                    key.IsDate != true &&
                    key.IsDateTime != true) //not appy for datetime
                {
                    //key.IsMultiSelecttBox != true

                    var obj = {
                        ColumnName: key.ColumnName,
                        IsAND: first,
                        Operator: 'Like',
                        FilterValue: textSearch,
                        FieldId: key.FieldId,
                        Type: "text",
                        ViewId: toolbarService.getCurrentViewId()
                    };
                    if (first) {
                        andCondition.push(obj);
                    } else {
                        orCondition.push(obj);
                    }
                    first = false;
                }
            });
            if (andCondition.length > 0)
                filterService.setAndConditions(andCondition, toolbarService.getCurrentViewId());
            if (orCondition.length > 0)
                filterService.setOrConditions(orCondition, toolbarService.getCurrentViewId());
        };

        $scope.search = function () {            
            if ($scope.textSearch.length > 0) {
                $scope.createSearchServerConditions($scope.textSearch);
                //trigger to advance search "viewConditionCtrl"
                $rootScope.$broadcast('initCondition');
                filterService.doFilter(toolbarService.getCurrentViewId());
            } else {
                toolbarService.ResetQuickSearch();
            }
        };

        $scope.$on("refreshEvent", function () {
                $scope.textSearch = '';
        });
        $scope.$on("resetQuickSearchEvent", function () {
            $scope.textSearch = '';
        });
        // End Quick search

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext();

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

                $scope.textSearch = '';

                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {
                    return;
                }
                toolbarService.NeedSaveCommand(false);
                toolbarService.ShowAdvanceSearch(true);
                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();
            }
        );

        /*add by: phoangular.vo*/
        $scope.ImportData = function () {
            showDialogImport(true);
        };
        $scope.ExportData = function () {
            showDialogExport(true);
        };


        var showDialogImport = function (isShown) {
            if (isShown) {
                $('#dialogImport').dialog({
                    title: $scope.languages.TOOLBAR.IMPORT_FORM_TITLE,
                    autoOpen: false,
                    resizable: false,
                    height: 'auto',
                    show: { effect: 'drop', direction: "up" },
                    modal: true,
                    draggable: true,
                    width: 500,
                    position: ['top', 5],
                    dialogClass: 'dialog-close-icon',
                    open: function (event, ui) {

                    },
                });
                $('#dialogImport').dialog('open');

            } else {
                $(this).dialog('close');
            }
        };

        var showDialogExport = function (isShown) {
            if (isShown) {
                $('#dialogExport').dialog({
                    title: $scope.languages.TOOLBAR.EXPORT_FORM_TITLE,
                    autoOpen: false,
                    resizable: false,
                    height: 'auto',
                    show: { effect: 'drop', direction: "up" },
                    modal: true,
                    draggable: true,
                    width: 500,
                    position: ['top', 5],
                    dialogClass: 'dialog-close-icon',
                    open: function (event, ui) {

                    },
                });
                $('#dialogExport').dialog('open');
            } else {
                $(this).dialog('close');
            }
        };

        $scope.$on("closeDialogExport", function () {
            $('#dialogExport').dialog('close');
        });       
    }
})();
