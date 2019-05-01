(function () {

    'use strict';

    angular.module("LeonardCRMFrontEnd").controller("AppStepsPageCtrl", ctrl);

    ctrl.$inject = ["$scope", 'salesCustomerService', "FileUploader", '_', 'salesOrderService', 'requestContext', 'dialogService', "appService", "appConfig",  '$timeout', '$location'];

    function ctrl($scope, salesCustomerService, FileUploader, _, salesOrderService, requestContext, dialogService, appService, appConfig, $timeout, $location) {
        //----------private variables--------------------       
        var lastStep = 5;
        var countIncludedView = 0;
        var isOpeningSignPadPop = false;
        var initDate = new Date();
        initDate.setYear(initDate.getFullYear() - 10);
        var copyAppConfirmKey = "copy-app-confirm";
        //----------scope variables--------------------  
        $scope.FormTitle = '';
        $scope.detailFormUrl = "";

        $scope.moduleId = 2;
        $scope.applicant = {
        	IsCustomer: true,
            SalesCustReferences: [],
            SalesOrders: []
        };
        $scope.applicantId = 0;

        $scope.isSubmitting = false;
        $scope.stepsBase = [{ Name: $scope.languages.APPLICANT_FORM.REQUEST_STEP, Order: 1, StepClass: 'btn-starter' },
                           { Name: $scope.languages.APPLICANT_FORM.APPROVED_STEP, Order: 2, StepClass: 'btn-primary' },
                           { Name: $scope.languages.APPLICANT_FORM.AGREEMENT_STEP, Order: 3, StepClass: 'btn-danger' },
                           { Name: $scope.languages.APPLICANT_FORM.PAYEMENT_STEP, Order: 4, StepClass: 'btn-success' },
                           { Name: $scope.languages.APPLICANT_FORM.DELIVERY_STEP, Order: 5, StepClass: 'btn-warning' },
                           { Name: $scope.languages.APPLICANT_FORM.COMPLETE_STEP, Order: 6, StepClass: 'btn-info' }];

        $scope.steps = [];
        $scope.currentStep = 1;
        $scope.currentStepOnForm = 1;
        $scope.isShowPineStep = false;
        $scope.doneStep = 0;
        $scope.keyPopup = '';
        $scope.isShowDeliveryTab = false;
        $scope.isShowCustomerAcceptTab = false;

        //check if app is belonged to the sold process
        $scope.isSoldProcess = false;

        //$scope.currentUser = appService.getCurrentUser();

        //status costant
        $scope.pendingStatus = appConfig.orderStatus.pendingStatus;
        $scope.preApprovalStatus = appConfig.orderStatus.preApprovalStatus;
        $scope.pendingCusAcceptStatus = appConfig.orderStatus.pendingCusAcceptStatus;
        $scope.inProgressStatus = appConfig.orderStatus.inProgressStatus;
        //$scope.contractSignedStatus = appConfig.orderStatus.contractSignedStatus;
        //$scope.paidFullStatus = appConfig.orderStatus.paidFullStatus;
        $scope.pendingDeliveryStatus = appConfig.orderStatus.pendingDeliveryStatus;
        $scope.deliveredNotSigned = appConfig.orderStatus.deliveredNotSigned;
        $scope.completedStatus = appConfig.orderStatus.completedStatus;
        $scope.cancelledStatus = appConfig.orderStatus.cancelledStatus;

        $scope.birthDateOptions = {
        	maxDate: initDate,
        	initDate: initDate
        };
        $scope.addressSinceOptions = {
        	minMode: 'month',
        	datepickerMode: 'month',
        	maxDate: new Date(),
        	initDate: new Date()
        };

        //----------scope methods----------------------    

        //open pop-up
        $scope.openSignPopup = function (key) {
        	isOpeningSignPadPop = true;

        	$scope.keyPopup = key;
        	$("#dialogDetail p.error").remove();
        	$('#dialogDetail input.error').removeClass("error");
        	$scope.$broadcast("openSignaturePopup", $scope.keyPopup);
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

        $scope.saveSignPopup = function () {
            $scope.$broadcast("saveSignaturePopup", $scope.keyPopup);
        }

        //hide pop-up
        $scope.hidePopup = function () {
            $('#dialogDetail').modal('hide');
        }

        //get applicant by Id
        $scope.getApplicationById = function (id) {
            salesCustomerService.getApplicantById(id).then(function (data) {
                if (data == null)
                    NotifyError('Some unexpected error occurred. Please reload the page and try again or contact admin.');
                else {
                    $scope.applicant = angular.fromJson(data);
                    if ($scope.applicant.SalesOrders != null && $scope.applicant.SalesCustReferences.length < 3) {
                        $scope.applicant.SalesCustReferences.push({ Id: 0, Name: '', Relationship: '', Phone: '' });
                    }

                    if (!$scope.applicant.SalesOrders[0].SalesOrderDeliveries) {
                    	$scope.applicant.SalesOrders[0].SalesOrderDeliveries = [{
                    		Id: 0,
                    		DeliveryType: appConfig.deliveryTypes.standardDeliveryType
                    	}];
                    }

                    //with "Sold" process
                    if ($scope.applicant.SalesOrders && $scope.applicant.SalesOrders.length > 0 &&
                        $scope.applicant.SalesOrders[0].IsSold === true) {
                        //mark as the "Sold" process
                        $scope.isSoldProcess = true;                       
                    }

                    $scope.setPageHeader('Status: ' + $scope.applicant.SalesOrders[0].StatusName, $scope.applicant.SalesOrders[0].StatusDescription);
                    $scope.$broadcast('ApplicantLoaded', $scope.applicant);
                }
            });
        }

        //check & specify the step on process
        $scope.initProcessStep = function() {
            if ($scope.applicant != null &&
                $scope.applicant.SalesOrders.length > 0 &&
                $scope.applicant.SalesOrders[0].Status != null) {
                //set the default step
                $scope.currentStep = 1;

                //specify the step
                var appStatus = $scope.applicant.SalesOrders[0].Status;
                switch (appStatus) {
                	case $scope.preApprovalStatus:
                        $scope.currentStep = 2;
                        break;                    
                    case $scope.pendingCusAcceptStatus:
                        $scope.currentStep = 3;
                        if (($scope.applicant.SalesOrders[0].LesseeSignature &&
                             $scope.applicant.SalesOrders[0].LesseeSignature != null) ||
                            ($scope.applicant.SalesOrders[0].SalesOrderDeliveries != null &&
                             $scope.applicant.SalesOrders[0].SalesOrderDeliveries.length > 0 && 
                             $scope.applicant.SalesOrders[0].SalesOrderDeliveries[0].Id > 0)) {
                            $scope.isShowDeliveryTab = true;
                        }

                        break;                   
                    case $scope.inProgressStatus:
                        $scope.currentStep = 4;
                        if($scope.applicant)
                        $scope.isShowDeliveryTab = true;
                        break;                   
                    case $scope.pendingDeliveryStatus:                    
                        $scope.currentStep = 5;
                        $scope.isShowDeliveryTab = true;
                        $scope.isShowCustomerAcceptTab = true;
                        break;
                    case $scope.deliveredNotSigned:
                        $scope.currentStep = 6;
                        $scope.isShowDeliveryTab = true;
                        $scope.isShowCustomerAcceptTab = true;
                        break;
                    case $scope.completedStatus:
                        $scope.currentStep = 6;
                        $scope.isShowDeliveryTab = true;
                        $scope.isShowCustomerAcceptTab = true;
                        break;
                }
                //init cursor step
                $scope.currentStepOnForm = $scope.currentStep;

                //load content by step
                loadContentByStep($scope.currentStep);
            }
        }

        $scope.checkDOB = function (model, isCheckCoApp) {
        	if ((model - initDate) > 0) {
        		if (!isCheckCoApp) {
        			$scope.applicant.DateOfBirth = initDate;
        		}
        		else {
        			$scope.applicant.CoDateOfBirth = initDate;
        		}
        	}
        }

        $scope.checkAddressSince = function () {
        	if (($scope.applicant.AtAddressSince - (new Date())) > 0) {
        		$scope.applicant.AtAddressSince = new Date();
        	}
        }

        $scope.cloneThisApp = function () {
            $scope.SetConfirmMsg($scope.languages.APPLICANT_FORM.COPY_APP_CONFIRM_MSG, copyAppConfirmKey, $scope.languages.APPLICANT_FORM.COPY_APP_CONFIRM_TITLE);
        };

        //---------event hanlder method-----------------------
       
        $scope.$on('ReloadStep', function () {
            $scope.initProcessStep();
            $scope.setPageHeader('Status: ' + $scope.applicant.SalesOrders[0].StatusName, $scope.applicant.SalesOrders[0].StatusDescription);
        });

        $scope.$on('ApplicantLoaded', function (e, app) {
            $scope.applicant = app;

            //Identify the current step 
            $scope.initProcessStep();      
        });

        $scope.$on(
           "requestContextChanged",
           function () {
               $('#dialogDetail').remove();
           }
        );

        $scope.$on("$includeContentLoaded", function (event) {
        	if (countIncludedView >= 6 && isOpeningSignPadPop) {
        		$scope.$broadcast("includedPopup", $scope.keyPopup);
        	}        	
        	countIncludedView += 1;        	
        });

        $scope.$on("yesEvent", function (e, key) {
            if (key === copyAppConfirmKey) {
                var applicantId = requestContext.getParamAsInt('appId', 0);
                $scope.savePromise = salesOrderService.cloneApp(applicantId).then(function (response) {
                    if (response) {
                        if (response.ReturnCode === 200) {
                            NotifySuccess(response.Result);
                            $timeout(function () {
                                $location.path('/my-applications/' + response.Id);
                                location.reload();
                            }, 2000);
                        }
                        else {
                            NotifyError(response.Result);
                        }
                    }
                });
            }
        });

        //------------Dialog Event ----------------------
        $scope.setPopUp = function(title , url) {
            $scope.FormTitle = title;
            $scope.detailFormUrl = url;
        };

        //---------internal method-----------------------
        //initialize page
        function init() {
            $scope.setWindowTitle($scope.languages.APPLICANT_FORM.DETAIL_TITLE);
            $scope.applicantId = requestContext.getParamAsInt("appId", 0);

            //init steps
            drawStep();

            //Show default tab
            $('#Ordertab a:first').tab('show');
            $('#Ordertab a').off("click");
            $('#Ordertab a').click(function (e) {
                e.preventDefault();
                if (!$(this).parent().hasClass("disabled")) {
                    $(this).tab('show');
                }
            });
        }

        //create the step layout
        function drawStep() {
            angular.copy($scope.stepsBase, $scope.steps);
            $scope.isShowPineStep = true;
            $scope.doneStep = lastStep + 1;
            //setTimeout(function () {
            //    configureCircleLines($("#step-container"));
            //}, 200);            
        }
		    
        //load the content by step on process
        function loadContentByStep(step) {
            //init the default tab
            var activeTab = $('#Ordertab a:first');

            switch (step) {
                case 1://Request                                  
                    break;

                case 2://Approval
                    activeTab = $('#Ordertab a[href="#ContractInfo"]');
                    break;

                case 3://Agreement

                    activeTab = $scope.applicant.SalesOrders[0].LesseeSignature &&
                                $scope.applicant.SalesOrders[0].LesseeSignature.length > 0 ? $('#Ordertab a[href="#DeliveryRequest"]') : //signed contract already
                                                                                             $('#Ordertab a[href="#ContractInfo"]'); //not sign yet contract
                    break;

                case 4://Payment            	
                    activeTab = $('#Ordertab a[href="#DeliveryRequest"]');
                    break;

                case 5://Delivery
                    activeTab = $scope.applicant.IsDeliveryPerson ? $('#Ordertab a[href="#CustomerAccept"]') : $('#Ordertab a[href="#DeliveryRequest"]');
                    break;

                case 6://Completed
                    activeTab = $('#Ordertab a[href="#CustomerAccept"]');
                    break;
            }

            activeTab.tab('show');
        }
		        
        init();
    }

})();