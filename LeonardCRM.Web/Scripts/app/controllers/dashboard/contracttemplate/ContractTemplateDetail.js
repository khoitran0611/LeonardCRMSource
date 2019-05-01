(function() {

    'use strict';

    angular.module("LeonardCRM").controller("ContractTemplateDetailCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'entityFieldService', 'ContractTemplateService', 'toolbarService', 'appService', 'requestContext', '_', 'filterService'];

    function ctrl($scope, $http, $location, entityFieldService, ContractTemplateService, toolbarService, appService, requestContext, _, filterService) {

        appService.hasPermission($scope.ModuleId).then(function (data) {
            $scope.hasPermission = data;
        });
        $scope.contractTemplate = new Object();
        $scope.tinymceOptions = {};
        $scope.templateId = requestContext.getParamAsInt("templateId", 0);
        $scope.columns = new Array();
        $scope.CustomerModuleId = appService.getModuleId('customer');
        $scope.OrderModuleId = appService.getModuleId('order');        
        $scope.insertField = { value : ''};
        $scope.select2Options = {
           
        };

        $scope.pickListForm = {
            States: []
        }

        $scope.myInit = function () {         

            //get column of customer
            entityFieldService.GetEntityFieldByModuleId($scope.CustomerModuleId)
                .then(function (data, resultstatus, resultheaders, resultconfig) {                    
                    $scope.columns = angular.fromJson(data);
                    //set prefix customer
                    angular.forEach($scope.columns, function (value, key) {
                        value.ColumnName = 'customer_' + value.ColumnName;
                        value.GroupBy = 'Customer';
                    });

                    //get column of order
                    entityFieldService.GetEntityFieldByModuleId($scope.OrderModuleId)
                       .then(function (orderCols) {
                           //set prefix order
                           angular.forEach(orderCols, function (value, key) {
                               value.ColumnName = 'order_' + value.ColumnName;
                               value.GroupBy = 'Order';
                           });

                           //append the order column to list
                           $scope.columns = $scope.columns.concat(angular.fromJson(orderCols));
                           
                           //add external field
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "CurrentDay", LabelDisplay: "Current Day" });
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "CurrentMonth", LabelDisplay: "Current Month" });
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "CurrentYear", LabelDisplay: "Current Year" });
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "CurrentDate", LabelDisplay: "Current Date" });
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "CurrentDateTime", LabelDisplay: "Current Date & Time" });
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "Order_Promotion", LabelDisplay: "Order Promotion" });
                           $scope.columns.push({ GroupBy: "Other", ColumnName: "order_CapitalizationPeriod_Addition", LabelDisplay: "Capitalization Period Number" });
						   $scope.columns.push({ GroupBy: "Other", ColumnName: "PercentOfCapitializationPeriod", LabelDisplay: "Percent Of Capitialization" });
                           //$scope.columns = _.groupBy($scope.columns, 'GroupBy');
                       });                   
                });

            
            ContractTemplateService.LoadData($scope.templateId).then(function (data, status, headers, config) {
                $scope.contractTemplate = angular.fromJson(data);
                if ($scope.templateId > 0) {
                    tinyMCE.execCommand('mceSetContent', false, $scope.contractTemplate.TemplateContent);
                }
                toolbarService.NeedSaveCommand(true);
                loadPicklist();
            });            
        };

        $scope.SaveContractTemplate = function () {
            ContractTemplateService.CreateTemplate($scope.contractTemplate, $scope.CurrentModule)
                .then(function (data, status, headers, config) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        if ($scope.templateId == 0) {
                            $scope.contractTemplate = '';
                            tinyMCE.execCommand('mceSetContent', false, "");
                        }
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                    toolbarService.NeedSaveCommand(true);
                });
        };

        $scope.insertYourContent = function () {
            tinyMCE.activeEditor.execCommand('mceInsertContent', false, '@' + $scope.insertField.value + '@');
        }

        $scope.groupFind = function (item) {
        	return item.GroupBy;
        };

        //----------------START HANDLE EVENT---------------------
        $scope.$on('saveEvent', function (e) {
            $scope.SaveContractTemplate();
        });

        $scope.$on('cancelEvent', function (event) {
            toolbarService.NeedSaveCommand(false);
            if ($scope.previousUrl == '') {                
                $location.path('/dashboard/contract_templates/');
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        //---------------END HANDLE EVENT-----------------------
        toolbarService.NeedSaveCommand(true);
        $scope.setWindowTitle($scope.languages.CONTRACT_TEMPLATE.DETAIL_TITLE);

        //---------------Internal methods-----------------------
        function loadPicklist() {
            var pickLists = filterService.getPickList($scope.CurrentModule);            
            $scope.pickListForm.States = _.where(pickLists, { 'FieldName': 'State' });
            if ($scope.contractTemplate != null &&
                $scope.contractTemplate.UsedStates != null && 
                $scope.contractTemplate.UsedStates.length > 0)
            {
                $scope.pickListForm.States = _.filterWithNotInValues($scope.pickListForm.States, "Id", $scope.contractTemplate.UsedStates);
            }
        }
    }   

})();