(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("EditSoldFormCtrl", ctrl);

    ctrl.$inject = ["$scope", "$location", 'salesCustomerService', '_', 'feFilterService', 'appConfig', '$sce', 'orderAttachmentService', 'userService', '$timeout', 'dialogService', 'salesOrderService'];

    function ctrl($scope, $location, salesCustomerService, _, feFilterService, appConfig, $sce, orderAttachmentService, userService, $timeout, dialogService, salesOrderService) {
        var countInclude = 0;
        var changeEmailTimer = null;

        $scope.moduleId = 2;
        $scope.applicant = {};
        $scope.salesCustReferences = [];
        $scope.states = [];
        $scope.maillingStates = [];
        $scope.residenceTypes = [];
        $scope.landTypes = [];
        $scope.stores = [];
        $scope.currentStep = 1;
        $scope.numOfStep = 4;
        $scope.numOfReferences = 2;
        $scope.isShowCam = false;
        $scope.isLockContiue = false;
        $scope.isExistEmail = false;
        var initDate = new Date();
        initDate.setYear(initDate.getFullYear() - 10);

        //Delivery Form Step
        $scope.states = [];
        $scope.deliveryTypes = [];
        $scope.deliveryTimes = [];
        $scope.loadDoorFacings = [];
        $scope.FormTitle = $scope.languages.APPLICANT_FORM.CREATE_SIGNATURE_POP_TITLE;
        $scope.detailFormUrl = "/appviews/frontend/deliveryrequestform/signaturePop.html";
        $scope.keySignaturePop = 'delivery-signarure';
        $scope.dateOptions = {
            minDate: new Date()
        };
        $scope.deliveryTypeConstants = appConfig.deliveryTypes;
        $scope.isShowCustomerSignature = true;
        $scope.deliveryObj = {
            CustomerSignImageUrl: null,
            WaiverAccepted: null,
            DeliveryType: $scope.deliveryTypeConstants.standardDeliveryType
        };

        var customerSignatureApi = null;
        var driverSignatureApi = null;
        var isOpeningSignPadPop = false;

        /*-------------Scope Method--------------*/
        $scope.addressSinceOptions = {
            minMode: 'month',
            datepickerMode: 'month',
            maxDate: new Date(),
            initDate: new Date()
        };

        $scope.infoSectionsBase = [{ Name: $scope.languages.APPLICANT_FORM.APPLICATION_INFO, Order: 1, StepClass: '', MinWidth: 179 },
                                   { Name: $scope.languages.APPLICANT_FORM.APPLICANT_SECTION_TITLE, Order: 2, StepClass: '', MinWidth: 125 },
                                   { Name: $scope.languages.APPLICANT_FORM.ADDRESS_SECTION_TITLE, Order: 3, StepClass: '', MinWidth: 125 },
                                   { Name: $scope.languages.DELIVERY_REQUEST_FORM.DELIVERY_REQUEST_TAB, Order: 4, StepClass: '', MinWidth: 125 }];

        $scope.infoSections = [];

        $scope.save = function () {
            var form = $('#applicantForm');
            form.submit();
            event.preventDefault();
        }

        $scope.checkStoreValue = function () {
            var temp = _.findWithProperty($scope.stores, "AdditionalInfo", $scope.applicant.StoreNumber);
            if (!angular.isDefined(temp)) {
                $scope.applicant.StoreNumber = ' ';
            }
        }

        $scope.searchStore = function (item) {
            var searchVal = $("#store-id").val();
            if (angular.isDefined(searchVal) && searchVal != null && searchVal != '') {
                var res = (item.Description.toLowerCase().indexOf(searchVal.toLowerCase()) != -1) || (item.AdditionalInfo.toLowerCase().indexOf(searchVal.toLowerCase()) != -1);
                return res;
            }
            return false;
        }

        $scope.changeSameMailingChk = function () {
            if ($scope.applicant.IsSameMailingAddress == true) {
                $(".physical-address").attr('data-validation', '');
                $scope.applicant.PhysicalStreet = null;
                $scope.applicant.PhysicalCity = null;
                $scope.applicant.PhysicalZip = null;
                $scope.applicant.PhysicalState = null;
            }
            else {
                $(".physical-address").attr('data-validation', 'required');                
            }
        }

        $scope.gotoNext = function () {
            validateStep();
        }

        $scope.gotoBack = function () {
            if ($scope.currentStep > 1) {
                $scope.currentStep = $scope.currentStep - 1;
                clearValidStyle();
            }
        }

        $scope.checkAddressSince = function () {
            if (($scope.applicant.AtAddressSince - (new Date())) > 0) {
                $scope.applicant.AtAddressSince = new Date();
            }
        }

        //Delivery Signature Popup
        $scope.openSignPopup = function (key) {            
            isOpeningSignPadPop = true;

            $scope.keyPopup = key;
            $("#dialogDetail p.error").remove();
            $('#dialogDetail input.error').removeClass("error");
            clearSiganture();
            $('#dialogDetail').modal();
            dialogService.hideAllButtons(false);

            $('#dialogDetail').off('hide.bs.modal');
            $('#dialogDetail').on('hide.bs.modal', function (e) {
                isOpeningSignPadPop = false;
            });

            $('#dialogDetail').off('hidden.bs.modal');
            $('#dialogDetail').on('hidden.bs.modal', function (e) {
                isOpeningSignPadPop = false;
            });
        }

        $scope.hidePopup = function () {
            $('#dialogDetail').modal('hide');
        }

        $scope.saveSignPopup = function () {            
            var isValid = customerSignatureApi.validateForm();
            if (isValid) {
                var signatureImageBase64 = customerSignatureApi.getSignatureImage($scope.applicant.ClientIP, $("#print-name-customer").val());
                $scope.deliveryObj.CustomerSignImage = signatureImageBase64.replace("data:image/png;base64,", "");
                $scope.deliveryObj.CustomerSignImageUrl = signatureImageBase64;
                $scope.isShowCustomerSignature = false;
                $('#dialogDetail').modal('hide');
            }
        }

        /*-------------Controller Method--------------*/
        function isFurtherInfoValid() {
            var error = "";

            if (!$scope.applicant.HomePhone && !$scope.applicant.CellPhone) {
                error += $scope.languages.APPLICANT_FORM.AT_LEAST_PHONE_ERROR_MSG + "</br>";
            }

            if (!isAdditionalReferenceEmpty() && !isAdditionalReferenceFull()) {
                error += $scope.languages.APPLICANT_FORM.REF_RELATION_PHONE_3_REQUIRE_ERROR_MSG + "</br>";
            }

            if (!!error) {
                NotifyError(error);
                return false;
            }

            return true;
        }

        function isAdditionalReferenceEmpty() {
            return !$scope.salesCustReferences[2].Name && !$scope.salesCustReferences[2].Relationship && !$scope.salesCustReferences[2].Phone;
        }

        function isAdditionalReferenceFull() {
            return !!$scope.salesCustReferences[2].Name && !!$scope.salesCustReferences[2].Relationship && !!$scope.salesCustReferences[2].Phone;
        }

        function addReferences() {
            $scope.applicant.SalesCustReferences = $scope.salesCustReferences;
        }

        function addOrder() {
            var order = {
                StoreNumber: $scope.applicant.StoreNumber,
                PartNumber: $scope.applicant.PartNumber,
                IsSold: true, //mark as Sold App
                SalesOrderDeliveries: [$scope.deliveryObj]
            }

            if ($scope.applicant.SalesOrders != null &&
                $scope.applicant.SalesOrders.length > 0) {
                $scope.applicant.SalesOrders.pop();
            }

            $scope.applicant.SalesOrders.push(order);            
        }

        function saveApplicant() {
            $scope.applicant.FieldData = [];
            $scope.applicant.CustomFields = [];

            addReferences();
            addOrder();
            $scope.savePromise = salesCustomerService.SaveCustomer($scope.applicant, $scope.modeParam).then(function (data) {
                if (data.ReturnCode == 200) {
                    NotifySuccess(data.Result, 10000);
                    $location.path('#/my-applications');
                } else {
                    NotifyError(data.Result, 10000);
                }
            });
        }

        function initDataForSalesCustReferences() {
            salesCustomerService.getApplicantById(0).then(function (data) {
                $scope.applicant = angular.fromJson(data);

                //clear the default name and email at assistant mode
                if ($scope.modeParam != null && $scope.modeParam != '') {
                    $scope.applicant.Name = null;
                    $scope.applicant.Email = null;
                }

                $scope.salesCustReferences = [];
                $scope.applicant.DateOfBirth = null;
                for (var i = 0; i < 3; i++) {
                    $scope.salesCustReferences.push({ Name: '', Relationship: null, Phone: '', IsActive: true });
                }

                //init order if null
                if(!$scope.applicant.SalesOrders)
                {
                    $scope.applicant.SalesOrders = [];
                }


            });
        }

        function loadDataForDropdownList() {
            $scope.PickList = feFilterService.getPickList($scope.moduleId);
            $scope.residenceTypes = _.filterWithProperty($scope.PickList, 'FieldName', 'ResidenceType');
            $scope.landTypes = _.filterWithProperty($scope.PickList, 'FieldName', 'LandType');
            $scope.maillingStates = $scope.states = _.filterWithProperty($scope.PickList, 'FieldName', 'PhysicalState');

            $scope.userModulePickList = feFilterService.getPickList(appConfig.usersModule);



            $scope.stores = _.filterWithProperty($scope.userModulePickList, 'FieldName', 'StoreId');

            $scope.cusRefPickList = feFilterService.getPickList(appConfig.saleCusRefModule);
            $scope.relationships = _.filterWithProperty($scope.cusRefPickList, 'FieldName', 'Relationship');

            //Delivery Form Step
            $scope.PickList = feFilterService.getPickList($scope.moduleId);
            $scope.states = _.filterWithProperty($scope.PickList, 'FieldName', 'PhysicalState');

            $scope.RequestPickList = feFilterService.getPickList(appConfig.salesDeliveryModule);
            $scope.deliveryTypes = _.filterWithProperty($scope.RequestPickList, 'FieldName', 'DeliveryType');
            $scope.deliveryTypes.pop();
            $scope.deliveryTimes = _.filterWithProperty($scope.RequestPickList, 'FieldName', 'DeliveryTime');

            $scope.loadDoorFacings = _.filterWithProperty($scope.RequestPickList, 'FieldName', 'LoadDoorFacing');
        }

        function validateStep() {
            $('#validate-form').hide();

            var container = $('#validate-form .field-content');
            container.empty();
            container.append($("#section-" + $scope.currentStep).clone());

            var mainForm = $('#applicantForm');
            mainForm.submit();

            var form = $('#validate-form');
            form.submit();
        }

        function clearValidStyle() {
            var form = $('#applicantForm');
            form.find('.has-error').removeClass('has-error');
            form.find('.has-success').removeClass('has-success');
            form.find('input,textarea,select')
                .css('border-color', '')
                .removeClass('error');
            form.find('span.form-error').remove();

            if ($('#validate-form .form-error').length > 0) {
                $('#validate-form .form-error').remove();
            }
        }

        function init() {
            angular.copy($scope.infoSectionsBase, $scope.infoSections);

            //use to validate form for whole form
            $.validate({
                form: '#applicantForm',
                modules: 'file',
                validateOnBlur: true,
                errorMessagePosition: 'top',
                scrollToTopOnError: true,
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    $scope.formStatus = status;
                    if ($scope.currentStep == $scope.numOfStep) {
                        if ($scope.formStatus && isFurtherInfoValid()) {
                            saveApplicant();
                        }
                    }
                    return false;
                },
                onError: function () {
                    return false;
                },
                onValidate: function (form) {
                    return "";
                }
            });

            //use to validate form for each step
            $.validate({
                form: '#validate-form',
                modules: 'file',
                validateOnBlur: false,
                errorMessagePosition: 'top',
                scrollToTopOnError: true,
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    clearValidStyle();
                    $('#validate-form').hide();
                    $('#validate-form .field-content').empty();
                    if ($scope.currentStep < $scope.numOfStep) {
                        $scope.currentStep = $scope.currentStep + 1;
                    }

                    if ($scope.currentStep === $scope.numOfStep) {//at last step
                        if ($scope.applicant.IsSameMailingAddress === true) {
                            $scope.applicant.PhysicalStreet = $scope.applicant.MailingStreet;
                            $scope.applicant.PhysicalCity = $scope.applicant.MailingCity;
                            $scope.applicant.PhysicalZip = $scope.applicant.MailingZip;
                            $scope.applicant.PhysicalState = $scope.applicant.MailingState;
                        }
                    }

                    return false;
                },
                onError: function () {
                    $('#validate-form .field-content').empty();
                    $('#validate-form').show();
                    return false;
                },
                onValidate: function (form) {
                    var msg = ''
                    if ($scope.currentStep == 2) {
                        var nameValue = $("#section-2 input[name='customer-name']").val();
                        if (nameValue != '' && nameValue.trim().split(' ').length < 2) {
                            msg += $scope.languages.APPLICANT_FORM.NAME_AT_LEAST_ERROR_MSG + "<br/>";
                        }

                        if (($scope.applicant.CellPhone == null || $scope.applicant.CellPhone == '') &&
                            ($scope.applicant.HomePhone == null || $scope.applicant.HomePhone == '')) {
                            msg += (msg != "" ? "* " : "") + $scope.languages.APPLICANT_FORM.AT_LEAST_PHONE_ERROR_MSG + "<br/>";
                        }

                        if (msg != '') {
                            return {
                                element: $("#validate-form input[name='HomePhone']"),
                                message: msg
                            }
                        }
                    }

                    return '';
                }
            });           

            initDataForSalesCustReferences();
            loadDataForDropdownList();
            initSignatureCtrl();

            //set window title
            $scope.setWindowTitle($scope.languages.APPLICANT_FORM.TITLE);
        }

        //Delivery Form Step
        function initSignatureCtrl() {
            setTimeout(function () {
                customerSignatureApi = $('#customer-sigPad').signaturePad({ validateFields: false, lineTop: 85 });
                clearSiganture();
            }, 200);
        }

        function clearSiganture() {
            if (customerSignatureApi != null) {
                $("#print-name-customer").val('');
                customerSignatureApi.clearCanvas();
            }
        }

        /*-------------Event Handler--------------*/
        $scope.$on('filterPickListEvent', function () {
            loadDataForDropdownList();
        });

        $scope.$on("$includeContentLoaded", function (event) {
            countInclude += 1;
            if (countInclude == $scope.numOfStep) {
                init();
            }

            if (countInclude == ($scope.numOfStep + 1)) { //include signature popup
                $("[data-mask]").inputmask();
            }
        });

        $scope.$on('$destroy', function () {
            $('#dialogDetail').remove();
        });

    }
})();