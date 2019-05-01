(function() {

    'use strict';

    angular.module("LeonardCRM").controller("ManageContractTemplateCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'ContractTemplateService', 'filterService', 'toolbarService', 'viewService'];

    function ctrl($scope, $http, $location, ContractTemplateService, filterService, toolbarService, viewService) {

            $scope.pageInfo = {};

            //---------START DELETE SALES INVOICES TEMPLATE-----------------
            $scope.DeleteSalesContractTemplate = function() {
                ContractTemplateService.DeleteContractTemplate($scope.EditValues)
                    .then(deleteSalesInvoiceTemplateSuccessCallback);
            };

            var deleteSalesInvoiceTemplateSuccessCallback = function(data, status, headers, config) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                } else {
                    NotifyError(data.Result, 5000);
                }
                $scope.GetViewById($scope.pageInfo);
            };

            //---------END DELETE SALES INVOICES TEMPLATE-------------------

            //------------START GETVIEWBYID----------------
            var getViewSuccessCallback = function(data, status, headers, config) {
                $scope.pageInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
            };

            $scope.GetViewById = function(args) {
                if (args.ViewId > 0) {
                    viewService.GetView(args)
                        .then(getViewSuccessCallback);
                }
            };
            //------------END GETVIEWBYID-------------------

            //----------------START REGISTER EVENT--------------------------

            $scope.$on('Grid_OnNeedDataSource', function(event, args) {
                $scope.pageInfo = args;
                $scope.GetViewById(args);
                event.preventDefault();
            });

            $scope.$on('GridPageIndexChanged', function(event, args) {
                $scope.pageInfo = angular.copy(args);
                filterService.doFilter(toolbarService.getCurrentViewId());
            });

            $scope.$on('sortEvent', function(event, args) {
                $scope.pageInfo = args;
                $scope.GetViewById(args);
                event.preventDefault();
            });

            $scope.$on('refreshEvent', function(event) {
                $scope.pageInfo.PageIndex = 1;
                $scope.pageInfo.AdvanceSearch = false;
                filterService.doFilter($scope.CurrentView);
                event.preventDefault();
            });

            $scope.$on('addEvent', function(e) {
                toolbarService.NeedSaveCommand(true);
                $location.path('/dashboard/contract_templates/add');
            });

            $scope.$on('deleteEvent', function(e) {
                $scope.SetConfirmMsg($scope.languages.INVTEMPLATE.DELETE_MSG, 'deleteContractTemplate');
            });

            $scope.$on('yesEvent', function(event, args) {
                if (args == 'deleteContractTemplate') {
                    $scope.DeleteSalesContractTemplate();
                }
            });

            $scope.$on('rowClickEvent', function(event, args) {
                toolbarService.NeedSaveCommand(true);
                $location.path('/dashboard/contract_templates/' + args.RowId + '/view/' + $scope.CurrentView);
            });
            $scope.$on('ServerFilter', function(event, args) {
                filterService.doFilter($scope.CurrentView);
            });

            $scope.$on('doFilterEvent', function(event, filterConditions) {

                var advConditions = filterConditions;
                viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pageInfo.DefaultOrderBy, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression).then(function(data) {
                    $scope.pageInfo = angular.fromJson(data);
                    $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
            });
            toolbarService.NeedSaveCommand(false);
            toolbarService.ShowAdvanceSearch(false);
            //---------------END REGISTER EVENT-------------------
            $scope.setWindowTitle($scope.languages.INVTEMPLATE.MANAGE_TITLE);
        }
    
})();