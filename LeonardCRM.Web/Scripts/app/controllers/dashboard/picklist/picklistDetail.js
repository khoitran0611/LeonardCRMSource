(function() {

    'use strict';

    angular.module("LeonardCRM").controller("PickListDetail", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'entityFieldService', 'picklistService', 'toolbarService', 'appService', '$routeParams', 'filterService', 'requestContext'];

    function ctrl($scope, $http, $location, entityFieldService, picklistService, toolbarService, appService, $routeParams, filterService, requestContext) {
            appService.hasPermission($scope.ModuleId).then(function(data) {
                $scope.hasPermission = data;
            });

            $scope.locked = false;

            $scope.picklistId = requestContext.getParamAsInt("picklistId", 0);
            $scope.modules = [];
            $scope.listname = {};
            $scope.listname.ListValues = [];
            $scope.listname.Eli_ListValues = [];

            $scope.templistvalue = {};
            $scope.editing = false;

            $scope.index = 0;
            $scope.Id = 0;
            $scope.status = "Active";
            $scope.listvalue = {};

            $scope.sortableOptions1 = {
                placeholder: 'field',
                connectWith: '.fields-container'
            };

            $scope.myInit = function() {
                $scope.listvalue.Active = true;
                picklistService.GetAllModule($scope.CurrentModule).then(function (data, status, headers, config) {
                    $scope.modules = angular.fromJson(data);

                    if ($scope.picklistId > 0) {
                        $scope.locked = true;

                        picklistService.GetListNameById($scope.picklistId)
                            .then(function(rdata, rstatus, rheaders, rconfig) {
                                $scope.listname = angular.fromJson(rdata);
                                toolbarService.NeedSaveCommand(true);
                            });
                    } else {
                        $scope.locked = false;

                        $scope.listname.Active = true;
                        $scope.listvalue.Active = true;
                    }
                });
            };

            $scope.SaveListName = function() {
                var index = 0;
                if (typeof $scope.listname.Eli_ListValues === 'undefined') {
                } else {
                    for (var i = 0; i < $scope.listname.Eli_ListValues.length; i++) {
                        $scope.listname.Eli_ListValues[i].ListOrder = i + 1;
                    }
                }
                picklistService.SaveListName($scope.listname, $scope.CurrentModule)
                    .then(function(data, status, headers, config) {
                        if (data.ReturnCode == 200) {
                            if (parseInt($scope.picklistId) == 0) {
                                $scope.listname.ListName = '';
                                $scope.listname.Description = '';
                                $scope.listname.Eli_ListValues.length = 0;
                            }
                            NotifySuccess(data.Result);

                        } else {
                            NotifyError(data.Result, 5000);
                        }
                        toolbarService.NeedSaveCommand(true);
                    });
            };

            $scope.AddListValue = function() {
                var temp = {};
                angular.copy($scope.listvalue, temp);
                for (var i = 0; i < $scope.listname.Eli_ListValues.length; i++) {
                    if (temp.Description == $scope.listname.Eli_ListValues[i].Description && temp.Id != $scope.listname.Eli_ListValues[i].Id && temp.Id != 0) {
                        NotifyError($scope.languages.PICKLIST.LISTVALUE_DESC_DUPLICATED, 5000);
                        return;
                    }
                }

                if (temp.Description.toString().trim().length > 0) {
                    if ($scope.editing == true) {
                        $scope.listname.Eli_ListValues.splice(parseInt($scope.index), 1);
                        $scope.listname.Eli_ListValues.splice(parseInt($scope.index), 0, temp);
                    } else {
                        temp.Editable = true;
                        temp.Active = true;
                        $scope.listname.Eli_ListValues.push(temp);
                    }

                } else {
                    return;
                }

                $scope.listvalue.Id = 0;
                $scope.listvalue.Editable = true;
                $scope.listvalue.Description = "";
                $scope.listvalue.AdditionalInfo = "";
                $scope.editing = false;
                $scope.listvalue.Active = true;
                $('#btnAdd').val($scope.languages.PICKLIST.SAVE_CMD);
            };

            $scope.Cancel = function() {
                $('#btnAdd').val($scope.languages.PICKLIST.SAVE_CMD);
                $scope.listvalue.Id = 0;
                $scope.editing = false;
                $scope.listvalue.Active = false;
                $scope.listvalue.Description = "";
                $scope.listvalue.AdditionalInfo = "";
                $scope.listvalue.Active = true;
            };

            $scope.editListValue = function($index) {
                angular.copy($scope.listname.Eli_ListValues[$index], $scope.templistvalue);
                $scope.listvalue = $scope.templistvalue;
                $scope.editing = true;
                $scope.index = $index;
                $('#btnAdd').val($scope.languages.PICKLIST.EDIT_CMD);
            };

            $scope.deleteListValue = function($index) {
                $scope.listname.Eli_ListValues.splice($index, 1);
            };

            $scope.SoftDeleteListValue = function($index) {
                $scope.listname.Eli_ListValues[$index].Active = false;
            };

            //----------------START HANDLE EVENT---------------------
            $scope.$on('saveEvent', function(e) {
                $scope.SaveListName();
            });

            $scope.$on('cancelEvent', function(event) {
                toolbarService.NeedSaveCommand(false);
                if ($scope.previousUrl == '') {
                    var id = appService.getDefaultView('picklist');
                    $location.path('/dashboard/picklist/');
                } else {
                    $location.path($scope.previousUrl);
                }
                event.preventDefault();
            });

            //---------------END HANDLE EVENT-----------------------

            toolbarService.NeedSaveCommand(true);
            $scope.setWindowTitle($scope.languages.PICKLIST.DETAIL_TITLE);
        }

})();