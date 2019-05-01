(function () {

    'use strict';

    angular.module("LeonardCRM").controller("ManageTaxCtrl", ctrl);

    ctrl.$inject = ["$scope", "$compile", "taxService", "viewService", "toolbarService", "filterService"];

    function ctrl($scope, $compile, taxService, viewService, toolbarService, filterService) {

        // --- Define Controller Variables. ---------------------- //

        // --- Define Controller Method. ---------------------- //

        var initDialog = function () {
            $('#tax-dialog-edit').dialog({
                title: $scope.languages.TAX.EDIT_TITLE,
                autoOpen: false,
                resizable: false,
                height: 'auto',
                show: { effect: 'drop', direction: 'up' },
                modal: true,
                draggable: true,
                closeOnEscape: false,
                width: 600,
                close: function (event, ui) {
                    //event handler for Esc pressed
                },
                open: function (event, ui) {
                },
                buttons:
                [
                    {
                        text: $scope.languages.COMMON.SAVE_BUTTON,
                        click: function () {
                            $scope.Save();
                        }
                    },
                    {
                        text: $scope.languages.COMMON.CANCEL_BUTTON,
                        click: function () {
                            $(this).dialog('close');
                        }
                    }
                ],
                position: ['top', 5],
                dialogClass: 'no-close'
            });
            $('.ui-dialog-buttonset button').addClass(function (index) {
                return "btn btn-sm label-primary";
            });

            var divTemplete = '<div></div>';
            var divEl = angular.element(divTemplete);
            var divContent = $('#taxeditContent');
            divEl.load('/appviews/dashboard/tax/detail.html', function () {
                var divContainer = angular.element(divContent);
                var html = $compile(divEl)($scope);
                divContainer.html(html);
                divContent.fadeIn();
            });
        };

        var showDialog = function (rowId) {
            $('#tax-dialog-edit').dialog('open');
            $scope.$broadcast('dialogDetail', rowId);
            return false;
        };

        var getViewSuccessCallback = function (data) {
            $scope.pageInfo = angular.fromJson(data);
            $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
        };

        var init = function () {
            initDialog();
            toolbarService.ShowAdvanceSearch(false);
        };

        // --- Define Scope Variables. ---------------------- //

        // --- Define Scope Method. ---------------------- //

        $scope.Save = function () {
            $scope.$broadcast('dialogSave');
        };

        $scope.GetViewById = function (args) {
            if (args.ViewId > 0) {
                viewService.GetView(args)
                    .then(getViewSuccessCallback);
            }
        };

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
            $scope.pageInfo.PageSize = $scope.maxPageSize;
            $scope.GetViewById($scope.pageInfo);
            $scope.setWindowTitle($scope.languages.TAX.TITLE);
        });

        $scope.$on('rowClickEvent', function (event, args) {
            showDialog(args.RowId);
        });

        $scope.$on('addEvent', function (event) {
            showDialog(0);
        });

        $scope.$on('reloadData', function (event) {
            $('#tax-dialog-edit').dialog('close');
            $scope.GetViewById($scope.pageInfo);
        });

        $scope.$on('refreshEvent', function (event) {
            $scope.pageInfo.PageIndex = 1;
            $scope.pageInfo.AdvanceSearch = false;
            $scope.GetViewById($scope.pageInfo);
        });

        $scope.$on('ServerFilter', function (event, args) {
            filterService.doFilter($scope.CurrentView);
        });

        $scope.$on('doFilterEvent', function (event, filterConditions) {
            
            var advConditions = filterConditions;
            viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression).then(function (data) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            });
        });

        $scope.$on('sortEvent', function (event, args) {
            filterService.doFilter($scope.ViewId);
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

            }
        );



        // --- Initialize. ---------------------------------- //
        init();
    }

})();