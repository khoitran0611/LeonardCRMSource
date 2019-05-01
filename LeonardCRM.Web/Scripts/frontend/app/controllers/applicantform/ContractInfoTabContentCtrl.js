(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("ContractInfoTabContentCtrl", ctrl);

    ctrl.$inject = ["$scope", 'salesCustomerService', 'salesOrderService', '$sce', 'dialogService'];

    function ctrl($scope, salesCustomerService, salesOrderService, $sce, dialogService) {
        //----------private variables--------------------       
        var signatureApi = null;
        var coSignatureApi = null;

        //----------scope variables--------------------  
        $scope.contractContent = '';
        $scope.isShowSignature = false;
        $scope.keySignaturePop = 'contract-signarure';
        $scope.isHaveContent = false;

        //----------scope methods----------------------  
        $scope.saveSignaure = function () {
            var app = angular.copy($scope.applicant.SalesOrders[0]);
            if ($scope.applicant.CustomerInitials == null || $scope.applicant.CustomerInitials == '')
            {
                $scope.applicant.CustomerInitials = $("#inital-customer-name").val();
            }            
            app.SalesCustomer = {
                CustomerInitials: $("#inital-customer-name").val(),
                CoName: $scope.applicant.CoName
            }

            var isValid = false;
            if (!app.LesseeSignature || app.LesseeSignature == '') {
                isValid = signatureApi.validateForm();
                if (isValid) {
                    app.CustomerSignImage = signatureApi.getSignatureImage($scope.applicant.ClientIP, $("#print-name-customer").val()).replace("data:image/png;base64,", "");
                }
            }

            if ($scope.applicant.CoName != null && $scope.applicant.CoName != '' &&
                ($("#print-name-co-customer").val() != '' || (app.LesseeSignature && app.LesseeSignature != ''))) {
                isValid = coSignatureApi.validateForm();
                if (isValid) {
                    app.CoCustomerSignImage = coSignatureApi.getSignatureImage($scope.applicant.ClientIP, $("#print-name-co-customer").val()).replace("data:image/png;base64,", "");
                }
            }

        	if (isValid) {
            	salesOrderService.saveSignature(app, $scope.modeParam).then(function (result) {
                    if (result.ReturnCode == 200) {
                        NotifySuccess(result.Result.Message);
                        $scope.applicant.SalesOrders[0].Status = result.Result.Status;
                        $scope.applicant.SalesOrders[0].StatusName = result.Result.StatusName;
                        $scope.applicant.SalesOrders[0].StatusDescription = result.Result.StatusDescription;
                        $scope.$emit('ReloadStep');
                        $scope.hidePopup();
                        loadContractContent();
                        $scope.isShowSignature = false;
                        setTimeout(function () {
                            location.reload();
                        }, 5000);                        
                    } else {
                        NotifyError(result.Result, 10000);
                    }
                });
            }
        }

        //---------event hanlder method-----------------------      

        $scope.$watch("$parent.applicant", function () {
            if ($scope.$parent.applicant && $scope.$parent.applicant.SalesOrders != null && $scope.$parent.applicant.SalesOrders.length > 0) {
                $scope.isShowSignature = $scope.$parent.applicant.SalesOrders[0].LesseeSignature == null || $scope.$parent.applicant.SalesOrders[0].LesseeSignature == "";
                if ($scope.applicant.CoName != null && $scope.applicant.CoName != '') {
                    $scope.isShowSignature = $scope.$parent.applicant.SalesOrders[0].CoSignature == null || $scope.$parent.applicant.SalesOrders[0].CoSignature == "";
                }
            }
        });

        $scope.$on("openSignaturePopup",function (e, key) {
        	if (key === $scope.keySignaturePop) {        		
              	$scope.setPopUp($scope.languages.APPLICANT_FORM.CREATE_SIGNATURE_POP_TITLE, "/appviews/frontend/applicantform/signaturePop.html");
              	clearSiganture();
              }
          });

        $scope.$on("includedPopup", function (e, key) {
        	if (key === $scope.keySignaturePop) {
        		initSignatureCtrl();
        	}
        });

        $scope.$on(
            "saveSignaturePopup",
            function (e, key) {
                if (key === $scope.keySignaturePop) {
                    $scope.saveSignaure();
                }
            }
        );

        //---------internal method-----------------------       
        function init() {
            loadContractContent();
        }

        function initSignatureCtrl() {
            setTimeout(function () {
            	signatureApi = $('#employee-sigPad').signaturePad({ lineTop: 85, validateFields: false });
            	coSignatureApi = $('#coEmployee-sigPad').signaturePad({ lineTop: 85, validateFields: false });

                clearSiganture();
            }, 200);
        }

        function clearSiganture() {
        	if ((signatureApi != null) && (coSignatureApi != null)) {
        		//clear control
        		$("#print-name-customer").val('');
        		$("#print-name-co-customer").val('');

        		signatureApi.clearCanvas();
        		coSignatureApi.clearCanvas();
        		$(".typed").empty();
        	}
        }

        function loadContractContent() {
            salesCustomerService.getContractContent($scope.applicantId).then(function (result) {
            	$scope.contractContent = $sce.trustAsHtml(angular.fromJson(result).Result);
            	$scope.isHaveContent = $scope.contractContent != "";
            });
        }

        init();
    }

})();