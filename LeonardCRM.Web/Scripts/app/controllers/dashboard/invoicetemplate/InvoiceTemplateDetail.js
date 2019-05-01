(function() {

    'use strict';

    angular.module("LeonardCRM").controller("InvoiceTemplateDetail", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', 'entityFieldService', 'InvTemplateService', 'toolbarService', 'appService', '$routeParams', 'filterService', 'requestContext'];

    function ctrl($scope, $http, $location, entityFieldService, InvTemplateService, toolbarService, appService, $routeParams, filterService, requestContext) {

        appService.hasPermission($scope.ModuleId).then(function (data) {
            $scope.hasPermission = data;
        });
        $scope.invTemplate = new Object();
        $scope.tinymceOptions = {};
        $scope.invTemplateId = requestContext.getParamAsInt("invoiceTemplateId", 0);
        $scope.columns = new Array();
        $scope.InvoiceModuleId = appService.getModuleId('invoice');
        var ServiceLine = new Object();
        ServiceLine.ColumnName = "ServiceLine";
        ServiceLine.LabelDisplay = $scope.languages.INVTEMPLATE.SERVICELINE;
        var CustomerLink = new Object();
        CustomerLink.ColumnName = "CustomerLink";
        CustomerLink.LabelDisplay = $scope.languages.INVTEMPLATE.CUSTOMER_LINK;
        $scope.myInit = function () {
            entityFieldService.GetEntityFieldByModuleId($scope.InvoiceModuleId)
                .then(function (data, resultstatus, resultheaders, resultconfig) {
                    data[data.length - 1] = ServiceLine;
                    data[data.length - 1] = CustomerLink;
                    $scope.columns = angular.fromJson(data);
                });

            if ($scope.invTemplateId != 0) {
                InvTemplateService.LoadData($scope.invTemplateId).then(function (data, status, headers, config) {
                    $scope.invTemplate = angular.fromJson(data);
                    tinyMCE.execCommand('mceSetContent', false, $scope.invTemplate.TemplateContent);
                    toolbarService.NeedSaveCommand(true);
                });
            }
        };

        $scope.SaveInvoiceTemplate = function () {
            InvTemplateService.CreateInvTemplate($scope.invTemplate, $scope.CurrentModule)
                .then(function (data, status, headers, config) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        if ($scope.invTemplateId == 0) {
                            $scope.invTemplate = '';
                            tinyMCE.execCommand('mceSetContent', false, "");
                        }
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                    toolbarService.NeedSaveCommand(true);
                });
        };

        //----------------START HANDLE EVENT---------------------
        $scope.$on('saveEvent', function (e) {
            $scope.SaveInvoiceTemplate();
        });

        $scope.$on('cancelEvent', function (event) {
            toolbarService.NeedSaveCommand(false);
            if ($scope.previousUrl == '') {
                var id = appService.getDefaultView('invoice_templates');
                $location.path('/dashboard/invoice_templates/');
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        //---------------END HANDLE EVENT-----------------------
        toolbarService.NeedSaveCommand(true);
        $scope.setWindowTitle($scope.languages.INVTEMPLATE.DETAIL_TITLE);
    }
   

})();