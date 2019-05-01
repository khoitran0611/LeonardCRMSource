(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("InvoicesDetailCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "salesInvoiceService", "registryService", "noteService", "salesOrderService", "$window", "toolbarService", "appService", "$routeParams", "requestContext", "$timeout", "windowService", "_", "entityFieldService", "filterService"];

    function ctrl($scope, $http, $location, salesInvoiceService, registryService, noteService, salesOrderService, $window, toolbarService, appService, $routeParams, requestContext, $timeout, windowService, _, entityFieldService, filterService) {
        appService.hasPermission($scope.CurrentParam.ModuleId).then(function (data) {
            $scope.hasPermission = data;
        });
        $scope.date_format = registryService.siteSettings.DATE_FORMAT;
        $scope.userSelect2Option = {};
        $scope.select2Options = {};
        $scope.invoice = new Object();
        $scope.service = new Object();
        $scope.invoiceId = $scope.CurrentParam.Id;
        $scope.parentId = $scope.CurrentParam.ParentId;
        $scope.parentModuleId = 0;
        $scope.pagedInfo = {};
        $scope.dateOptions = {
            'year-format': "'yy'",
            'starting-day': 1
        };
        $scope.tabnote = true;
        $scope.customFieldArgs = {};
        $scope.FieldData = [];
        $scope.formStatus = false;
        $scope.pickListForm = {
            'InvTemplateId': [],
            'Status': [],
            'ResponsibleUsers': [],
            'CurrencyId': [],
            'OrderId': []
        };
        $scope.userSelect2Option = {};

        $scope.SectionControls = {
            'ServiceSection': { 'Collapsed': false }
        };

        $scope.myInit = function () {
            $('#Invoicetab a:first').tab('show');
            $('#Invoicetab a').click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            });

            $.validate({
                form: '#invoiceform',
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                submitHandler: function (form) { // for demo
                    return false;
                },
                onSuccess: function (status) {
                    $scope.formStatus = status;
                    if ($scope.formStatus) {
                        $scope.addinvoice();
                    }
                    return false;
                },
                onError: function () {
                    //alert('error');
                    //return false;
                },
                onValidate: function () {
                    //alert('on validate');
                    //return false;
                }

            });

            var param = $scope.invoiceId + ',' + $scope.CurrentParam.ModuleId;
            salesInvoiceService.LoadData(param).then(function (data, status, headers, config) {
                $scope.invoice = angular.fromJson(data);
                if ($scope.parentId != 0) {
                    $scope.invoice.OrderId = $scope.parentId;
                }
                if ($scope.invoiceId == 0) {
                    $scope.setTitle($scope.languages.INVOICES.ADD_INVOICE_TITLE);
                } else {
                    $scope.setTitle($scope.languages.INVOICES.EDIT_INVOICE_TITLE + ' #' + $scope.invoice.Id);
                }
                var pickList = filterService.getPickList($scope.CurrentParam.ModuleId);
                var referenceList = filterService.getReferenceList($scope.CurrentParam.ModuleId);
                $scope.pickListForm = {
                    'InvTemplateId': _.where(referenceList, { 'FieldName': 'InvTemplateId' }),
                    'Status': _.where(pickList, { 'FieldName': 'Status' }),
                    'ResponsibleUsers': _.where(referenceList, { 'FieldName': 'ResponsibleUsers' }),
                    'CurrencyId': _.where(referenceList, { 'FieldName': 'CurrencyId' }),
                    'OrderId': _.where(referenceList, { 'FieldName': 'OrderId' })
                };

                //---Setup toolbars commands------------------------
                toolbarService.SetupExportPDF(true);

                //after loading the invoice 
                $scope.$emit('getInvoiceIdSuccess', $scope.invoiceId);
                $scope.parentModuleId = $scope.CurrentParam.ModuleId;

                var properties = Object.keys($scope.invoice);
                var fields = filterService.getFields($scope.CurrentParam.ModuleId);
                angular.forEach(properties, function (prop, index) {
                    var field = _.findWithProperty(fields, 'ColumnName', prop);
                    if (field != null) {
                        $scope.invoice[prop + 'Control'] = {
                            'Visible': field.Visible,
                            'Locked': field.Locked
                        };
                    } else {
                        $scope.invoice[prop + 'Control'] = {
                            'Visible': false,
                            'Locked': true
                        };
                    }
                });

                entityFieldService.setCustomData($scope.invoice.CustomFields);
                $timeout(function () {
                    $scope.$broadcast('parentModulePassEvent');
                }, 500);
                if ($scope.invoiceId == 0) {

                    $scope.invoice.IssuedDate = new Date();
                    $scope.invoice.CurrencyId = $scope.pickListForm.CurrencyId[0].Id;
                    $scope.invoice.Status = $scope.pickListForm.Status[0].Id;
                    if ($scope.pickListForm.InvTemplateId.length > 0)
                        $scope.invoice.InvTemplateId = $scope.pickListForm.InvTemplateId[0].Id;
                }
            });
        };

        //-------------------------START ADD NEW ORDER--------------------------
        $scope.addinvoice = function () {
            for (var i = 0; i < $scope.FieldData.length; i++) {
                $scope.FieldData[i].CustFieldId = $scope.FieldData[i].FieldId;
                $scope.FieldData[i].MaterRecordId = $scope.invoiceId;
                if ($scope.FieldData[i].FieldDataId > 0) {
                    $scope.FieldData[i].Id = $scope.FieldData[i].FieldDataId;
                }
            }
            $scope.invoice.FieldData = $scope.FieldData;
            $scope.invoice.CustomFields = $scope.FieldData;

            if ($scope.invoice.Id > 0) {
                salesInvoiceService.AddSalesInvoice($scope.invoice, $scope.CurrentParam.ModuleId)
                .then(function (data, status, headers, config) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        if ($scope.invoiceId == 0) {
                            $scope.invoiceId = angular.copy(data.Id);
                            $scope.$emit('CloseAndOpenWindow', $scope.invoiceId);
                        } else {
                            $scope.$emit('closeWindow');
                        }
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
            } else {
                $scope.$broadcast('note_request');
            }

        };
        //----------------------END ADD NEW ORDER----------------------------


        //-----------------------START NOTES---------------------


        //------------------------END NOTES----------------------


        //----------------------START HANDLE EVENT---------------------------


        $scope.$on('exportPDFEvent', function (e) {
            var param = $scope.invoiceId + ',' + $scope.CurrentParam.ModuleId;
            salesInvoiceService.ExportPDF(param).then(function (data, status, headers, config) {
                if (data.ReturnCode == 200) {
                    $window.open(data.Result, 'invoice_pdf', 'width=980,height=600,menubar=no,toolbar=no,location=no,scrollbars=yes');
                } else {
                    NotifyError(data.Result, 5000);
                }
            });
        });

        $scope.$on('saveDataEvent', function (e) {
            $scope.$broadcast('parentSaveEvent');
            var form = $('#invoiceform');
            form.submit();
            e.preventDefault();
        });

        $scope.$on('cancelEvent', function (event) {
            if ($scope.previousUrl == '') {
                var id = appService.getDefaultView('invoice');
                $location.path('/invoice/view/' + id);
            } else {
                $location.path($scope.previousUrl);
            }
            $scope.invoiceId = 0;
            event.preventDefault();
        });

        $scope.$on('Grid_AddNewClicked', function (event, args) {
            var param = {
                Url: '/appviews/' + args.ModuleName + '/detail.html',
                Id: 0,
                ParentId: args.ParentId,
                ViewId: args.ViewId,
                ModuleId: args.ModuleId,
                Key: args.Key
            };
            windowService.openWindow(param);
        });

        $scope.$on('sendSeviceToInvoice', function (event, data) {
            $scope.invoice.Services = data.Services;
            $scope.invoice.Taxes = data.Taxes;
        });

        $scope.$on('sendCutomFieldEvent', function (event, data) {
            $scope.FieldData = data;
        });

        $scope.$watch('invoice.CurrencyId', function (newVal, oldVal) {
            if (newVal != undefined) {
                var baseCurrency = {};
                var serviceData = {
                    Currency: {},
                    Services: [],
                    Taxes: []
                };
                for (var i = 0; i < $scope.invoice.Currencies.length; i++) {
                    //get base currency
                    if ($scope.invoice.Currencies[i].BaseCurrency)
                        baseCurrency = $scope.invoice.Currencies[i];

                    if ($scope.invoice.Currencies[i].Id == newVal) {
                        serviceData.Currency = {
                            BaseCurrency: baseCurrency,
                            CurrentCurrency: $scope.invoice.Currencies[i]
                        };
                        serviceData.Taxes = angular.copy($scope.invoice.SalesInvTaxes);
                        serviceData.Services = angular.copy($scope.invoice.SalesInvServices);
                        break;
                    };
                }
                $timeout(function () {
                    $scope.$broadcast('CurrencyChanged', serviceData);
                }, 1000);
            }
        });

        $scope.$on('$destroy', function () {
            toolbarService.SetupExportPDF(false);
        });

        $scope.$on('notes_saved', function (event, notes) {
            $scope.invoice.Notes = notes;
            salesInvoiceService.AddSalesInvoice($scope.invoice, $scope.CurrentParam.ModuleId)
                .then(function (data, status, headers, config) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        if ($scope.invoiceId == 0) {
                            $scope.invoiceId = angular.copy(data.Id);
                            $scope.$emit('CloseAndOpenWindow', $scope.invoiceId);
                        } else {
                            $scope.$emit('closeWindow');
                        }
                    } else {
                        NotifyError(data.Result, 5000);
                    }
                });
        });
        //------------------------END HANDLE EVENT----------------------------

    }
})(angular);