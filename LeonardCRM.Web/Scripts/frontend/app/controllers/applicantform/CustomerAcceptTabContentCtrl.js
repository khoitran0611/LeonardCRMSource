(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("CustomerAcceptTabContentCtrl", ctrl);

    ctrl.$inject = ["$scope", "feFilterService", "appConfig", "_", "registryService", "salesOrderCompleteService"];

    function ctrl($scope, feFilterService, appConfig, _, registryService, salesOrderCompleteService) {
        //----------private variables--------------------       
        var customerSignatureApi = null;
        var deliverySignatureApi = null;
        var standardHour = 1;
        var standardMile = 30;
        var isDeliverySign = false;
        //----------scope variables--------------------  
        $scope.picklistForm = {
            States: [],
            PaymentTypes: [],
            ServiceRatings: [],
            deliveryTypes : []
        };
        $scope.FormTitle = $scope.languages.APPLICANT_FORM.CREATE_SIGNATURE_POP_TITLE;
        $scope.detailFormUrl = "/appviews/frontend/applicantform/acceptSignPop.html";
        $scope.keySignaturePop = 'customer-acceptance-signarure';
        $scope.deliveryDetailFormUrl = "/appviews/frontend/applicantform/deliverySignPop.html";
        $scope.keyDeliverySignaturePop = 'delivery-acceptance-signarure';
        $scope.registry = registryService.siteSettings;
        $scope.deliveryTypeConstants = appConfig.deliveryTypes;
        $scope.isEnableOtherDeliveryType = false;

        //----------scope methods----------------------  
        $scope.autoCaculatePrice = function () {
            var saleComplete = $scope.applicant.SalesOrders[0].SalesOrderCompletes[0];
            saleComplete.BlockCharge = saleComplete.ActBlocksUsed != null && saleComplete.ActBlocksUsed != "" ? (saleComplete.ActBlocksUsed * registryService.siteSettings.BLOCK_UNIT_PRICE) : 0;
            saleComplete.LaborCharge = saleComplete.ActHoursOfLabor != null && saleComplete.ActHoursOfLabor != "" && saleComplete.ActHoursOfLabor > standardHour ? ((saleComplete.ActHoursOfLabor - standardHour) * registryService.siteSettings.MAN_HOUR_UNIT_PRICE) : 0;
            saleComplete.MileageCharge = saleComplete.ActMilesToSite != null && saleComplete.ActMilesToSite != "" && saleComplete.ActMilesToSite > standardMile ? ((saleComplete.ActMilesToSite - standardMile) * registryService.siteSettings.LOADED_MILE_UNIT_PRICE) : 0;

            $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].AdditionalCharge = saleComplete.BlockCharge > 0 || saleComplete.LaborCharge > 0 || saleComplete.MileageCharge  > 0;
        }

        $scope.saveOrderComplete = function () {
            if (($scope.applicant.SalesOrders[0].SerialNumber == null || $scope.applicant.SalesOrders[0].SerialNumber == '') &&
                ($scope.serialNumber == null || $scope.serialNumber == ""))
            {
                NotifyError($scope.languages.CUSTOMER_ACCEPTANCE_FORM.CONFIRM_INPUT_SERIAL_NUMBER_MSG, 10000);
            }
            else
            {
                if ($scope.applicant.SalesOrders[0].SalesOrderCompletes[0].Signature == null || $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].Signature == '')
                {
                    saveCompleteObj();
                }
                else
                {
                    $scope.SetConfirmMsg($scope.languages.CUSTOMER_ACCEPTANCE_FORM.CONFIRM_SAVING_MSG, "save-customer-acceptance-form", $scope.languages.CUSTOMER_ACCEPTANCE_FORM.CONFIRM_SAVING_HEADING);
                }                
            }            
        }

        $scope.parseNumber = function (value) {
            return value != null && value != "" ? parseFloat(value) : 0;
        }

        $scope.getUnitPrice = function (resource, replaceVal) {
            return resource.replace("{0}", $scope.currencyDisplay + replaceVal);            
        }
      
        $scope.onToggleDeliveryTypeCheckbox = function (deliveryTypeId, isChecked) {
        	if (deliveryTypeId == $scope.deliveryTypeConstants.other) {
        		$scope.isEnableOtherDeliveryType = isChecked;
        	}
        }

        //---------event hanlder method-----------------------      
        $scope.$on(
            "openSignaturePopup",
            function (e, key) {
                if (key === $scope.keySignaturePop || key === $scope.keyDeliverySignaturePop) {
                    isDeliverySign = key === $scope.keyDeliverySignaturePop;
                    $scope.setPopUp($scope.FormTitle, !isDeliverySign ? $scope.detailFormUrl : $scope.deliveryDetailFormUrl);
                    clearSignature();
                }
            }
        );

        $scope.$on("includedPopup", function (e, key) {
        	if (key === $scope.keySignaturePop || key === $scope.keyDeliverySignaturePop) {
        		initSignatureCtrl();
        	}
        });


        $scope.$on(
            "saveSignaturePopup",
            function (e, key) {
                if (key === $scope.keySignaturePop || key === $scope.keyDeliverySignaturePop) {
                    var isDeliverySign = key === $scope.keyDeliverySignaturePop;

                    var saleComplete = angular.copy($scope.applicant.SalesOrders[0].SalesOrderCompletes[0]);
                    saleComplete.SalesOrder = { Id :$scope.applicant.SalesOrders[0].Id, Status: $scope.applicant.SalesOrders[0].Status };
                    var isValid = !isDeliverySign ? customerSignatureApi.validateForm() : deliverySignatureApi.validateForm();

                    if (isValid) {

                    	if (!isDeliverySign) {
                    		saleComplete.CustomerSignatureUrl = customerSignatureApi.getSignatureImage(saleComplete.SignIP, $("#print-name-customer").val()).replace("data:image/png;base64,", "");
                    	}
                    	else {
                    		saleComplete.DeliverySignatureUrl = deliverySignatureApi.getSignatureImage(saleComplete.SignIP, $("#print-name-delivery").val()).replace("data:image/png;base64,", "");
                    	}

                        salesOrderCompleteService.saveCustomerSignature(saleComplete, $scope.applicantId).then(function (data) {
                            if (data.ReturnCode === 200) {
                                $scope.hidePopup();

                                //update the signature data
                                var updatedData = data.Result;                                

                                if (angular.isDefined(updatedData) && updatedData != null) {
                                    if (!isDeliverySign) {
                                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].SignIP = updatedData.SignIP;
                                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].SignDate = updatedData.SignDate;
                                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].Signature = updatedData.Signature;
                                    }
                                    else
                                    {
                                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].DeliverSignIP = updatedData.DeliverSignIP;
                                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].DeliverSignDate = updatedData.DeliverSignDate;
                                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].DeliverSignature = updatedData.DeliverSignature;
                                    }
                                }
                                if (!isDeliverySign) {
                                    $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].CustomerSignatureUrl = customerSignatureApi.getSignatureImage(saleComplete.SignIP, $("#print-name-customer").val());
                                }
                                else {
                                    $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].DeliverySignatureUrl = deliverySignatureApi.getSignatureImage(saleComplete.SignIP, $("#print-name-delivery").val());
                                }
                            } else {
                                NotifyError(data.Result, 10000);
                            }
                        });
                    }
                }
            }
        );

        $scope.$on('yesEvent', function (event, args) {
            if (args == 'save-customer-acceptance-form') {
                saveCompleteObj();               
            }
        });

        $scope.$on("ApplicantLoaded", function () {
        	bindingDeliveryTypes();
        });

        $scope.$on("filterPickListEvent", function () {
            loadPicklist();
            bindingDeliveryTypes();
        });
        
        //---------internal method-----------------------       
        function init() {            
            initDateTimeInput();
            loadPicklist();

            if ($scope.applicant.SalesOrders &&
				$scope.applicant.SalesOrders.length > 0 &&
				$scope.applicant.SalesOrders[0].SalesOrderCompletes &&
				$scope.applicant.SalesOrders[0].SalesOrderCompletes.length > 0) {
            	bindingDeliveryTypes();
            }
        }

        function loadPicklist() {            
            var customerPicklist = feFilterService.getPickList($scope.moduleId);
            $scope.picklistForm.States = _.filterWithProperty(customerPicklist, 'FieldName', 'PhysicalState');

            var completePicklist = feFilterService.getPickList(appConfig.salesCompleteModule);
            $scope.picklistForm.PaymentTypes = _.filterWithProperty(completePicklist, 'FieldName', 'PaymentType');
            $scope.picklistForm.ServiceRatings = _.filterWithProperty(completePicklist, 'FieldName', 'Rating');

            var deliveryPicklist = feFilterService.getPickList(appConfig.salesDeliveryModule);
            $scope.picklistForm.deliveryTypes = _.filterWithProperty(deliveryPicklist, 'FieldName', 'DeliveryType');
        }

        function initDateTimeInput() {
            //Timepicker
            $("#customerAccpetanceForm .timepicker").timepicker({
                showInputs: false
            });
        }

        function initSignatureCtrl() {
        	setTimeout(function () {
        		if (!isDeliverySign) {
        			customerSignatureApi = $('#customer-sigPad').signaturePad({ validateFields: false, lineTop: 85 });       			
        		}
        		else {
        			deliverySignatureApi = $('#delivery-sigPad').signaturePad({ validateFields: false, lineTop: 85 });
        		}

        		clearSignature();
        	}, 200);
        }

        function clearSignature() {
        	if (!isDeliverySign) {        		
        		if (customerSignatureApi != null) {
        			$("#print-name-customer").val('');
        			customerSignatureApi.clearCanvas();
        		}
        	}
        	else {        		
        		if (deliverySignatureApi != null) {
        			$('#print-name-delivery').val('');
        			deliverySignatureApi.clearCanvas();
        		}
        	}
        }

        function bindingDeliveryTypes() {
            if ($scope.applicant.SalesOrders != null && $scope.applicant.SalesOrders.length > 0)
            {
                var complete = $scope.applicant.SalesOrders[0].SalesOrderCompletes[0];
                if (complete && complete.DeliveryType && complete.DeliveryType.length > 0) {
                    var deliverTypes = complete.DeliveryType.split(",").map(function (id) { return parseInt(id); });
                    var selectedDeliverTypes = _.filterWithValues($scope.picklistForm.deliveryTypes, "Id", deliverTypes);
                    if (angular.isArray(selectedDeliverTypes)) {
                        selectedDeliverTypes.forEach(function (item) {
                            item.checked = true;
                        });
                    }
                }
            }        	
        }

        function saveCompleteObj() {
            //caculate total
            $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].TotalAmountDue = $scope.parseNumber($scope.applicant.SalesOrders[0].SalesOrderCompletes[0].BalanceDue) +
                                                                                    $scope.parseNumber($scope.applicant.SalesOrders[0].SalesOrderCompletes[0].BlockCharge) +
                                                                                    $scope.parseNumber($scope.applicant.SalesOrders[0].SalesOrderCompletes[0].LaborCharge) +
                                                                                    $scope.parseNumber($scope.applicant.SalesOrders[0].SalesOrderCompletes[0].MileageCharge);
            var saleComplete = angular.copy($scope.applicant.SalesOrders[0].SalesOrderCompletes[0]);
            saleComplete.SalesOrder = angular.copy($scope.applicant.SalesOrders[0]);

            if ($scope.serialNumber != null && $scope.serialNumber != "") {                
                saleComplete.SalesOrder.SerialNumber = $scope.serialNumber;
            }

            //binding the Delivery Type data for complete
            var checkedDeliveryTypes = _.where($scope.picklistForm.deliveryTypes, { "checked": true });
            saleComplete.DeliveryType = checkedDeliveryTypes.map(function (item) { return item.Id; });
            if (angular.isArray(saleComplete.DeliveryType)) {
                saleComplete.DeliveryType = saleComplete.DeliveryType.join(",");
            }

            $scope.savePromise = salesOrderCompleteService.saveSaleComplete(saleComplete, $scope.applicantId).then(function (data) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result.Message, 10000);
                    if (data.Result.AppStatus != null) {
                        $scope.applicant.SalesOrders[0].Status = data.Result.AppStatus;
                        $scope.applicant.SalesOrders[0].StatusName = data.Result.StatusName;
                        $scope.applicant.SalesOrders[0].StatusDescription = data.Result.StatusDescription;
                        $scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerAccepted = true;
                        if ($scope.serialNumber != null && $scope.serialNumber != "") {
                            $scope.applicant.SalesOrders[0].SerialNumber = $scope.serialNumber;
                        }
                        $scope.applicant.SalesOrders[0].SalesOrderCompletes[0].Id = data.Id;
                        $scope.$emit('ReloadStep');
                    }
                } else {
                    NotifyError(data.Result, 10000);
                }
            });
        }

		init();
    }

})();