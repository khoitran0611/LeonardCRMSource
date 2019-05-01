(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("ServiceDetailCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "salesInvoiceService", "salesOrderService", "toolbarService", "appService", "taxService", "_"];

    function ctrl($scope, $http, $location, salesInvoiceService, salesOrderService, toolbarService, appService, taxService, _) {
        appService.hasPermission($scope.ModuleId).then(function (data) {
            $scope.hasPermission = data;
        });

        $scope.totalCost = 0;
        $scope.arrayService = [];
        $scope.tempServiceTaxes = $scope.baseTaxes = $scope.Taxes = [];

        //get taxes used for invoicing
        taxService.getAll().then(function (data) {
            $scope.Taxes = _.where(angular.fromJson(data), { 'IsActive': true });
            updateTaxes();
            $scope.baseTaxes = angular.copy($scope.Taxes);
            $scope.taxUsed = false;
        });

        //-------------------START DELETE SERVICE------------------------
        $scope.deleteService = function ($index) {
            $scope.totalCost = parseFloat($scope.totalCost) - parseFloat($scope.arrayService[$index].Cost);
            $scope.arrayService.splice($index, 1);
            $scope.oldServices.splice($index, 1);
            $scope.updateTaxTotal();
        };
        //-------------------END DELETE SERVICE--------------------------


        //---------------------START ADD NEW SERVICE---------------------
        $scope.addservice = function () {
            var temp = new Object();
            angular.copy($scope.service, temp);

            if (temp.ServiceName.toString().trim().length > 0 && temp.Cost.toString().trim().length > 0 && !isNaN(temp.Cost)) {
                $scope.invoiceCurrency = angular.copy($scope.currentCurrency.CurrentCurrency);
                $scope.arrayService.push(temp);
                $scope.oldServices.push(angular.copy(temp));
                $scope.totalCost = parseFloat($scope.totalCost) + parseFloat(temp.Cost);
                $scope.updateTaxTotal();
            } else {
                return;
            }

            //reset after adding
            resetService();
        };
        //---------------------END ADD NEW SERVICE----------------------


        //--------------START LOADING JQUERY VALIDATION-----------------
        $().ready(function () {

            $.validate({
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                submitHandler: function (form) { // for demo
                    return false;
                },
                onSuccess: function (status) {

                },
                onError: function () {
                },
                onValidate: function () {
                }

            });

        });
        //--------------END LOADING JQUERY VALIDATION-----------------


        //------------------START HANDLE EVENT---------------------------

        $scope.$on('parentSaveEvent', function (e) {
            var services = [];
            angular.forEach($scope.arrayService, function (item, index) {
                if (item.ServiceName && item.Cost && !isNaN(item.Cost) && item.Cost > 0)
                    services.push(item);
            });
            $scope.$emit('sendSeviceToInvoice', { Services: services, Taxes: $scope.Taxes });
        });

        $scope.count = 0;
        $scope.invoiceCurrency = {};
        $scope.oldServices = [];
        $scope.$on('CurrencyChanged', function (e, serviceData) {
            $scope.currentCurrency = serviceData.Currency;
            $scope.currencyFormat = $scope.currentCurrency.CurrentCurrency.Symbol;
            if ($scope.arrayService.length == 0) {
                $scope.arrayService = serviceData.Services;
                $scope.oldServices = angular.copy(serviceData.Services);
                $scope.invoiceCurrency = $scope.currentCurrency.CurrentCurrency;
            }

            $scope.totalCost = 0;
            if ($scope.arrayService.length > 0 && $scope.invoiceId != 0) {
                for (var i = 0; i < $scope.arrayService.length ; i++) {
                    try {
                        var currentItem = {};
                        currentItem = $scope.oldServices[i];

                        if ($scope.count > 0) {
                            if ($scope.invoiceCurrency.BaseCurrency != true) {
                                $scope.arrayService[i].Cost = ((parseFloat(currentItem.Cost) / $scope.invoiceCurrency.ConversionRate) * ($scope.currentCurrency.BaseCurrency.ConversionRate * parseFloat($scope.currentCurrency.CurrentCurrency.ConversionRate)));
                            } else {
                                $scope.arrayService[i].Cost = ((parseFloat(currentItem.Cost) * $scope.currentCurrency.CurrentCurrency.ConversionRate) / $scope.currentCurrency.BaseCurrency.ConversionRate);
                            }
                        }
                        $scope.totalCost = parseFloat($scope.totalCost) + parseFloat($scope.arrayService[i].Cost);
                        $scope.count += 1;

                    } catch (e) {
                        NotifyError(e);
                    }

                }
            }
            $scope.tempServiceTaxes = serviceData.Taxes;
            updateTaxes();
        });

        function updateTaxes() {
            if ($scope.tempServiceTaxes.length > 0) {
                angular.forEach($scope.tempServiceTaxes, function (item, index) {
                    if ($scope.Taxes.length > 0)
                        $scope.Taxes[index].TaxValue = item.TaxValue;
                });
                $scope.taxUsed = true;
            }
            $scope.updateTaxTotal();
        }

        $scope.updateTaxTotal = function (index, taxValue) {
            $scope.taxTotal = 0;
            if (taxValue && isNaN(taxValue)) {
                $scope.Taxes[index].TaxValue = 0;
            }
            for (var i = 0; i < $scope.Taxes.length; i++) {
                $scope.taxTotal += $scope.Taxes[i].TaxValue * $scope.totalCost / 100;
            }
        };

        $scope.$watch('taxUsed', function (newVal) {
            //if tax used, then using base taxes. Otherwise, resetting taxes to 0
            if (newVal == true) {
                if ($scope.arrayService.length == 0)
                    $scope.Taxes = angular.copy($scope.baseTaxes);
            } else {
                angular.forEach($scope.Taxes, function (item) {
                    item.TaxValue = 0;
                });
            }
            $scope.updateTaxTotal();
        });

        $scope.copyTaxes = function () {
            $scope.Taxes = angular.copy($scope.baseTaxes);
            $scope.updateTaxTotal();
        };

        $scope.saveItem = function (item) {
            if (!item.Cost || isNaN(item.Cost))
                item.Cost = 0;
            if (item.ServiceName) {
                item.isEditting = false;
                $scope.totalCost = 0;
                for (var i = 0; i < $scope.arrayService.length; i++) {
                    $scope.totalCost = parseFloat($scope.totalCost) + parseFloat($scope.arrayService[i].Cost);
                }
                $scope.updateTaxTotal();
            }
        };

        //$scope.$on('parentSaveSuccess', function (e) {
        //    resetService();
        //});

        function resetService() {
            $scope.service.ServiceName = "";
            $scope.service.Cost = "";
            $scope.service.Description = "";
            $scope.service.Comments = "";
            $scope.service.isEditting = false;
        }
        //-----------------END HANDLE EVENT-----------------------------

    }

})(angular);