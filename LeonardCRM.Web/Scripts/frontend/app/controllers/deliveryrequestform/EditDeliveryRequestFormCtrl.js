(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("EditDeliveryRequestFormCtrl", ctrl);

    ctrl.$inject = ["$scope", 'feFilterService', '_', 'salesCustomerService', "dialogService", "salesOrderService", "salesDeliveryService", "appConfig"];

    function ctrl($scope, feFilterService, _, salesCustomerService, dialogService, salesOrderService, salesDeliveryService, appConfig) {

        //----------Scope variables-------------                
        $scope.states = [];
        $scope.deliveryTypes = [];
        $scope.deliveryTimes = [];
        $scope.loadDoorFacings = [];
        $scope.isCompleted = false;
        $scope.FormTitle = $scope.languages.APPLICANT_FORM.CREATE_SIGNATURE_POP_TITLE;
        $scope.detailFormUrl = "/appviews/frontend/deliveryrequestform/signaturePop.html";
        $scope.keySignaturePop = 'delivery-signarure';
        $scope.dateOptions = {
			minDate : new Date()
        };
        $scope.deliveryTypeConstants = appConfig.deliveryTypes;

        var customerSignatureApi = null;
        var driverSignatureApi = null;

        $scope.isShowCustomerSignature = false;

        //-------------Method------------------
        function loadDataForDropdownList() {
            // For Applicant Form
            $scope.PickList = feFilterService.getPickList($scope.moduleId);
            $scope.states = _.filterWithProperty($scope.PickList, 'FieldName', 'PhysicalState');

            $scope.RequestPickList = feFilterService.getPickList(appConfig.salesDeliveryModule);
            $scope.deliveryTypes = _.filterWithProperty($scope.RequestPickList, 'FieldName', 'DeliveryType');
            $scope.deliveryTypes.pop();
            $scope.deliveryTimes = _.filterWithProperty($scope.RequestPickList, 'FieldName', 'DeliveryTime');

            $scope.loadDoorFacings = _.filterWithProperty($scope.RequestPickList, 'FieldName', 'LoadDoorFacing');
        }

        function SaveDeliveryRequest() {
            $scope.applicant.FieldData = [];
            $scope.applicant.CustomFields = [];
            var delivery = angular.copy($scope.applicant.SalesOrders[0].SalesOrderDeliveries[0]);
            delivery.SalesOrder = { Status: $scope.applicant.SalesOrders[0].Status, SalesCustomer: angular.copy($scope.applicant) };
            if (delivery.Id == 0) {
            	delivery.OrderId = $scope.applicantId;
            }

            $scope.savePromise = salesDeliveryService.saveSaleDelivery(delivery, $scope.modeParam).then(function (data) {
                if (data.ReturnCode === 200) {
                    NotifySuccess(data.Result.Message, 10000);
                    $scope.applicant.SalesOrders[0].Status = data.Result.AppStatus;
                    $scope.applicant.Editable = data.Result.AppStatus === $scope.pendingStatus;
                    $scope.getApplicationById($scope.applicantId);
                    $scope.$emit('ReloadStep');
                } else {
                    NotifyError(data.Result, 10000);
                }
            });
        }

        $scope.save = function () {
            var form = $('#deliveryRequestForm');
            form.submit();         
        }

        //-------------Init----------------
        function init() {
            // Define Form
            $.validate({
                form: '#deliveryRequestForm',
                validateOnBlur: true,
                errorMessagePosition: 'top',
                scrollToTopOnError: true,
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    $scope.formStatus = status;
                    if ($scope.formStatus && isFurtherInfoValid()) {
                        SaveDeliveryRequest();
                    }
                    return false;
                },
                onError: function () {
                    return false;
                },
                onValidate: function () {
                    return "";
                }
            });

            //Init the input masks
            $("[data-mask]").inputmask();
            loadDataForDropdownList();
            
        }

        // Other validation
        function isFurtherInfoValid() {
            var error = "";
            if ($scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].DeliveryType == $scope.deliveryTypeConstants.moveJobType) {
                if ($scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].MoveToAddress == null) {
                    error += $scope.languages.APPLICANT_FORM.MOVE_TO_ADDRESS_REQUIRE_ERROR_MSG + "</br>";
                }

                if ($scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].MoveFromAddress == null) {
                    error += $scope.languages.APPLICANT_FORM.MOVE_FROM_ADDRESS_REQUIRE_ERROR_MSG + "</br>";
                }

                if (!!error) {
                    NotifyError(error);
                    return false;
                }
            }
            return true;
        }

        function initSignatureCtrl() {
        	setTimeout(function () {
        		customerSignatureApi = $('#customer-sigPad').signaturePad({validateFields: false, lineTop: 85 });        		
        		clearSiganture();
        	}, 200);
        }

        function clearSiganture() {
        	if (customerSignatureApi != null) {
        		$("#print-name-customer").val('');
        		customerSignatureApi.clearCanvas();
        	}
        }

        //---------event hanlder method-----------------------      
        $scope.$on(
             "openSignaturePopup",
             function (e, key) {
                 if (key === $scope.keySignaturePop) {
                 	$scope.setPopUp($scope.FormTitle, $scope.detailFormUrl);
                 	clearSiganture();
                 }
             }
        );

        $scope.$on("includedPopup", function (e, key) {
        	if (key === $scope.keySignaturePop) {
        		initSignatureCtrl();
        	}
        });

        $scope.$on(
            "saveSignaturePopup",
            function (e, key) {
                if (key === $scope.keySignaturePop) {
                    var app = angular.copy($scope.applicant.SalesOrders[0]);
                    var isValid = customerSignatureApi.validateForm();
                    if (isValid) {
                        app.SalesOrderDeliveries[0].CustomerSignImage = customerSignatureApi.getSignatureImage(app.SalesOrderDeliveries[0].CustomerSignIP, $("#print-name-customer").val()).replace("data:image/png;base64,", "");
                    }

                    if (isValid) {
                        salesOrderService.saveDeliverySignature(app).then(function(result) {
                            if (result.ReturnCode === 200) {                                
                                $scope.hidePopup();
                                
                                //update the signature data
                                var updatedApplicant = result.Result.Applicant.SalesOrderDeliveries[0];
                                $scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerSignImageUrl = customerSignatureApi.getSignatureImage(app.SalesOrderDeliveries[0].CustomerSignIP, $("#print-name-customer").val());
                                if (angular.isDefined(updatedApplicant) && updatedApplicant != null) {
                                    $scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerSignIP = updatedApplicant.CustomerSignIP;
                                    $scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerSignDate = updatedApplicant.CustomerSignDate;
                                    $scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerSignature = updatedApplicant.CustomerSignature;
                                }

                                $scope.isShowCustomerSignature = false;
                            } else {
                                NotifyError(result.Result, 10000);
                            }
                        });
                    }
                }
            }
        );

        $scope.$on('filterPickListEvent', function () {
            loadDataForDropdownList();
        });

        $scope.$watch("$parent.applicant", function () {
            if ($scope.$parent.applicant &&
                $scope.$parent.applicant.SalesOrders != null &&
                $scope.$parent.applicant.SalesOrders.length > 0) {
                if ($scope.$parent.applicant.SalesOrders[0].SalesOrderDeliveries != null &&
                    $scope.$parent.applicant.SalesOrders[0].SalesOrderDeliveries.length > 0) {
                    $scope.isShowCustomerSignature = ($scope.$parent.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerSignature == null ||
                                                      $scope.$parent.applicant.SalesOrders[0].SalesOrderDeliveries[0].CustomerSignature === "");
                }
            }
        });

        init();
    }

})();