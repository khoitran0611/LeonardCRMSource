(function() {

    'use strict';

    angular.module("LeonardCRM").controller("ManagePickListCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', '$compile', '$timeout', 'picklistService', 'salesInvoiceService', 'salesOrderService', 'registryService', 'toolbarService', 'viewService', 'filterService'];

    function ctrl($scope, $http, $location, $compile, $timeout, picklistService, salesInvoiceService, salesOrderService, registryService, toolbarService, viewService, filterService) {

            $scope.pagedInfo = {};
            //---------START DELETE PICK LIST-----------------
            $scope.DeleteListName = function() {
                picklistService.DeleteListName($scope.EditValues)
                    .then(deleteListNameeSuccessCallback);
            };
            var deleteListNameeSuccessCallback = function(data, status, headers, config) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result);
                } else {
                    NotifyError(data.Result, 5000);
                }
                $scope.GetViewById($scope.pagedInfo);
            };

            //---------END DELETE PICK LIST------------------

            //------------START GETVIEWBYID----------------
            var getViewSuccessCallback = function(data, status, headers, config) {
                $scope.pagedInfo = angular.fromJson(data);
                $scope.$broadcast('Grid_DataBinding', $scope.pagedInfo);
            };

            $scope.GetViewById = function(args) {
                if (args.ViewId > 0) {
                    viewService.GetView(args)
                        .then(getViewSuccessCallback);
                }
            };
            //------------END GETVIEWBYID-------------------

            //----------------START REGISTER EVENT--------------------------

            //-----------Advance Search Event Listener---------------------------------
            $scope.$on('doFilterEvent', function(event, filterConditions) {
                var advConditions = filterConditions;
                viewService.AdvanceSearch(advConditions, $scope.CurrentView, $scope.pagedInfo.DefaultOrderBy, $scope.pagedInfo.PageSize, $scope.pagedInfo.PageIndex, $scope.pagedInfo.GroupColumn, $scope.pagedInfo.SortExpression)
                    .then(function(data) {
                        $scope.pagedInfo = angular.fromJson(data);
                        $scope.$broadcast('Grid_DataBinding', $scope.pagedInfo);
                    });

            });
            //-----------END Advance Search Event Listener---------------------------------


            $scope.$on('Grid_OnNeedDataSource', function(event, args) {
                $scope.pagedInfo = args;
                $scope.GetViewById(args);
                event.preventDefault();
            });


            $scope.$on('addEvent', function(e) {
                toolbarService.NeedSaveCommand(true);
                $location.path('/dashboard/picklist/add');
            });

            $scope.$on('sortEvent', function(event, args) {
                $scope.pagedInfo = args;
                $scope.GetViewById(args);
                event.preventDefault();
            });

            $scope.$on('refreshEvent', function(event) {
                $scope.pagedInfo.PageIndex = 1;
                $scope.pagedInfo.AdvanceSearch = false;
                $scope.GetViewById($scope.pagedInfo);
                event.preventDefault();
            });

            $scope.$on('deleteEvent', function(e) {
                $scope.SetConfirmMsg($scope.languages.INVTEMPLATE.DELETE_MSG, 'picklist');
            });

            $scope.$on('yesEvent', function(event, args) {
                if (args == 'picklist') {
                    $scope.DeleteListName();
                }
            });

            $scope.$on('rowClickEvent', function(event, args) {
                toolbarService.NeedSaveCommand(true);
                $location.path('/dashboard/picklist/' + args.RowId + '/view/' + $scope.CurrentView);
            });


            $scope.$on('GridPageIndexChanged', function(event, args) {
                $scope.pagedInfo = angular.copy(args);
                filterService.doFilter(toolbarService.getCurrentViewId());
            });

            $scope.$on('ServerFilter', function(event, args) {
                filterService.doFilter($scope.CurrentView);
            });

            //---------------END REGISTER EVENT-------------------
            $scope.setWindowTitle($scope.languages.PICKLIST.MANAGE_TITLE);
        }

})();