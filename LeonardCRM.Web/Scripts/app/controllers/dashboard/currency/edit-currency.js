(function() {

    "use strict";

    angular.module("LeonardCRM").controller("EditCurrencyCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$compile', 'currencyService', '_'];

    function ctrl($scope, $http, $compile, currencyService, _) {

        $scope.currencies = [];
        $scope.currency = {};
        $scope.currencyNames = [];
        $scope.originalCurrencyNames = [];
        $scope.filteredCurrencyNames = [];

        var initCurrencyForm = function () {
            $scope.setTitle($scope.languages.CURRENCY.DIALOG_TITLE);
            currencyService.GetAllCurrency().then(function (data, status, headers, config) {
                $scope.currencies = angular.fromJson(data);
            });
            currencyService.GetCurrencyById($scope.CurrentParam.Id).then(function (data1) {
                $scope.currency = angular.fromJson(data1);
                currencyService.GetAllCurrencyName().then(function (data, status, headers, config) {
                    $scope.currencyNames = angular.fromJson(data);
                    $scope.originalCurrencyNames = $scope.currencyNames.concat();

                    if ($scope.currency.Id == 0) {
                        $scope.filteredCurrencyNames.length = 0;
                        for (var i = 0; i < $scope.currencyNames.length; i++) {
                            var name = $scope.currencyNames[i].Country + ',' + $scope.currencyNames[i].Currency;
                            var currency = _.findWithProperty($scope.currencies, 'Name', name);
                            if (currency != null) {
                                //$scope.filteredCurrencyNames.push($scope.currencyNames[i]);
                                $scope.currencyNames.splice(i, 1);
                                i--;
                            }
                        }
                        //$scope.currencyNames.length = 0;
                        //$scope.currencyNames = $scope.filteredCurrencyNames.concat();

                        $scope.currency.Name = $scope.currencyNames[0].Country + ',' + $scope.currencyNames[0].Currency;
                        $scope.currency.Code = $scope.currencyNames[0].Code;
                        $scope.currency.Symbol = $scope.currencyNames[0].Symbol;
                    }

                });
            });
        };

        $scope.SetValue = function () {
            for (var i = 0; i < $scope.currencyNames.length; i++) {
                var temp = $scope.currencyNames[i];
                if ($scope.currency.Name.indexOf(',') != -1) {
                    var name = temp.Country + ',' + temp.Currency;
                    if (name == $scope.currency.Name) {
                        $scope.currency.Code = temp.Code;
                        $scope.currency.Symbol = temp.Symbol;
                        break;
                    }
                } else {
                    if (temp.Country == $scope.currency.Name) {
                        $scope.currency.Code = temp.Code;
                        $scope.currency.Symbol = temp.Symbol;
                    }
                }
            }
        };


        $scope.$on('EditCurrencyEvent', function (event, args) {
            $scope.currency = jQuery.extend(true, {}, args);
            currencyService.GetAllCurrencyName().then(function (data, status, headers, config) {
                $scope.currencyNames = angular.fromJson(data);
                $scope.originalCurrencyNames = $scope.currencyNames.concat();

                if ($scope.currency.Id == 0) {
                    $scope.filteredCurrencyNames.length = 0;
                    for (var i = 0; i < $scope.currencyNames.length; i++) {
                        var name = $scope.currencyNames[i].Country + ',' + $scope.currencyNames[i].Currency;
                        var currency = _.findWithProperty($scope.currencies, 'Name', name);
                        if (currency != null) {
                            //$scope.filteredCurrencyNames.push($scope.currencyNames[i]);
                            $scope.currencyNames.splice(i, 1);
                            i--;
                        }
                    }
                    //$scope.currencyNames.length = 0;
                    //$scope.currencyNames = $scope.filteredCurrencyNames.concat();

                    $scope.currency.Name = $scope.currencyNames[0].Country + ',' + $scope.currencyNames[0].Currency;
                    $scope.currency.Code = $scope.currencyNames[0].Code;
                    $scope.currency.Symbol = $scope.currencyNames[0].Symbol;
                }

            });
            event.preventDefault();
        });

        $scope.$on('saveDataEvent', function (event) {
            if ($scope.currency.ConversionRate == null || $scope.currency.ConversionRate == '' || isNaN($scope.currency.ConversionRate)) {
                NotifyError('Invalid Conversion Rate', 5000);
                return;
            }
            currencyService.Save($scope.currency, $scope.CurrentParam.ModuleId).then(function (data, status, headers, config) {
                if (data.ReturnCode == 200) {

                    if ($scope.currency.Id == 0) {
                        $scope.currencies.push($scope.currency);

                        for (var i = 0; i < $scope.currencyNames.length; i++) {
                            var name = $scope.currencyNames[i].Country + ',' + $scope.currencyNames[i].Currency;
                            if (name == $scope.currency.Name) {
                                $scope.currencyNames.splice(i, 1);
                                break;
                            }
                        }

                        $scope.currency.Name = $scope.currencyNames[0].Country + ',' + $scope.currencyNames[0].Currency;
                        $("#currName").val($scope.currency.Name).select2();

                        $scope.currency.Code = $scope.currencyNames[0].Code;
                        $scope.currency.Symbol = $scope.currencyNames[0].Symbol;
                    }

                    $scope.currency.Name = $scope.currencyNames[0].Country + ',' + $scope.currencyNames[0].Currency;
                    NotifySuccess(data.Result);
                    $scope.currency.ConversionRate = '';
                    $scope.currency.IsActive = true;
                    if ($scope.currency.Id > 0) {
                        $scope.$emit('closeWindow');
                    } else {
                        $scope.$emit('CloseAndOpenWindow', data.Id);
                    }
                } else {
                    NotifyError(data.Result, 5000);
                }
            });
        });

        initCurrencyForm();
    }

})();