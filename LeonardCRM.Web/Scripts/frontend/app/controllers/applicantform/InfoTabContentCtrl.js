(function () {

	'use strict';

	angular.module("LeonardCRMFrontEnd").controller("InfoTabContentCtrl", ctrl);

	ctrl.$inject = ["$scope", 'salesCustomerService', "FileUploader", '_', 'feFilterService', 'salesOrderService', 'requestContext', 'appConfig'];

	function ctrl($scope, salesCustomerService, FileUploader, _, feFilterService, salesOrderService, requestContext, appConfig) {
		//----------private variables--------------------       
		var applicantId = 0;
		var tempApplicantId = 0;

		//----------scope variables--------------------                   
		$scope.salesCustReferences = [];
		$scope.states = [];
		$scope.maillingStates = [];
		$scope.residenceTypes = [];
		$scope.landTypes = [];
		$scope.onlyShowCustomer = false;

		//----------scope methods----------------------          

		$scope.Combobox_Changed = function (fieldValue, fieldName) {
			//var listTargetValueIds;
			//var result;
			//var data;
			//switch (fieldName) {
			//    case 'PhysicalCountry':
			//        result = _.filterWithProperty($scope.PickListDependencies, 'MasterValueId', fieldValue);
			//        listTargetValueIds = _.uniq(_.pluck(result, 'ChildValueId'));
			//        data = _.filterWithValues($scope.PickList, 'Id', listTargetValueIds);
			//        $scope.states = _.filterWithProperty(data, 'FieldName', 'PhysicalState');
			//        break;
			//    case 'MailingCountry':
			//        result = _.filterWithProperty($scope.PickListDependencies, 'MasterValueId', fieldValue);
			//        listTargetValueIds = _.uniq(_.pluck(result, 'ChildValueId'));
			//        data = _.filterWithValues($scope.PickList, 'Id', listTargetValueIds);
			//        $scope.maillingStates = _.filterWithProperty(data, 'FieldName', 'MailingState');
			//        break;
			//}
		}

		$scope.save = function () {
			var form = $('#applicantForm');
			form.submit();
			event.preventDefault();
		}

		$scope.cancelApp = function (appId) {
			tempApplicantId = appId;
			$scope.SetConfirmMsg($scope.languages.APPLICANT_FORM.CANCEL_APPICANT_CONFIRM, 'cancelApp', $scope.languages.APPLICANT_FORM.CANCEL_APPICANT_CONFIRM_HEADER);
		}

		$scope.isEnableLandLordField = function () {
			return ($scope.applicant.ResidenceType != null && $scope.applicant.ResidenceType === appConfig.residenceTypes.Rent) ||
				   ($scope.applicant.LandType != null && $scope.applicant.LandType === appConfig.landTypes.Rent);
		}

		$scope.updateLandLordInfo = function () {
			if (!$scope.isEnableLandLordField()) { //disabled
				$scope.applicant.LandlordName = null;
				$scope.applicant.LandlordPhone = null;
			}
		}

		$scope.changedMaillingAddress = function () {
			if ($scope.applicant.IsSameMailingAddress == true) {
				$scope.applicant.PhysicalStreet = $scope.applicant.MailingStreet;
				$scope.applicant.PhysicalCity = $scope.applicant.MailingCity;
				$scope.applicant.PhysicalZip = $scope.applicant.MailingZip;
				$scope.applicant.PhysicalState = $scope.applicant.MailingState;
			}				
		}
		//---------event hanlder method-----------------------
		$scope.$on('filterPickListEvent', function () {
			loadDataForDropdownList();
		});

		$scope.$on('yesEvent', function (event, args) {
			if (args == 'cancelApp') {
				$scope.cancelPromise = salesOrderService.cancelApplication(tempApplicantId).then(function (data) {
					if (data.ReturnCode == 200) {
						NotifySuccess(data.Result.Message);
						$scope.applicant.SalesOrders[0].Status = data.Result.Status;
						$scope.applicant.SalesOrders[0].StatusDescription = data.Result.StatusDescription;
						$scope.applicant.SalesOrders[0].StatusName = data.Result.StatusName;
						$scope.applicant.Editable = false;
						$scope.$emit('ReloadStep');
					} else {
						NotifyError(data.Result);
					}
				});
			}
		});

		$scope.$on('ApplicantLoaded', function (e, app) {
			if (app.Editable) {
				initInputPlugin();
			}
		});
		//---------internal method-----------------------
		function init() {
			applicantId = requestContext.getParamAsInt('appId', 0);

			loadDataForDropdownList();
			$scope.getApplicationById($scope.applicantId);
		}

		function saveApplicant() {
			$scope.applicant.FieldData = [];
			$scope.applicant.CustomFields = [];

			$scope.savePromise = salesCustomerService.SaveCustomer($scope.applicant, $scope.modeParam).then(function (data) {
				if (data.ReturnCode == 200) {
					NotifySuccess(data.Result.Message, 10000);
					$scope.applicant.SalesOrders[0].Status = data.Result.AppStatus;
					$scope.applicant.SalesOrders[0].StatusDescription = data.Result.StatusDescription;
					$scope.applicant.SalesOrders[0].StatusName = data.Result.StatusName;
					$scope.applicant.Editable = data.Result.AppStatus == $scope.pendingStatus;
					$scope.$emit('ReloadStep');
				} else {
					NotifyError(data.Result, 10000);
				}
			});
		}

		function loadDataForDropdownList() {
			$scope.PickList = feFilterService.getPickList(appConfig.customerModule);
			$scope.residenceTypes = _.filterWithProperty($scope.PickList, 'FieldName', 'ResidenceType');
			$scope.landTypes = _.filterWithProperty($scope.PickList, 'FieldName', 'LandType');
			$scope.maillingStates = $scope.states = _.filterWithProperty($scope.PickList, 'FieldName', 'PhysicalState');

			$scope.cusRefPickList = feFilterService.getPickList(appConfig.saleCusRefModule);
			$scope.relationships = _.filterWithProperty($scope.cusRefPickList, 'FieldName', 'Relationship');
		}

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
			return angular.isDefined($scope.applicant.SalesCustReferences[2]) && $scope.applicant.SalesCustReferences[2] != null ? (!$scope.applicant.SalesCustReferences[2].Name && !$scope.applicant.SalesCustReferences[2].Relationship && !$scope.applicant.SalesCustReferences[2].Phone) : true;
		}

		function isAdditionalReferenceFull() {
			return angular.isDefined($scope.applicant.SalesCustReferences[2]) && $scope.applicant.SalesCustReferences[2] != null ? (!!$scope.applicant.SalesCustReferences[2].Name && !!$scope.applicant.SalesCustReferences[2].Relationship && !!$scope.applicant.SalesCustReferences[2].Phone) : true;
		}

		function initInputPlugin() {
			//init client validator
			$.validate({
			    form: '#applicantForm',
			    validateOnBlur: true, 
			    errorMessagePosition: 'top', 
			    scrollToTopOnError: true,
			    showHelpOnFocus: false,
			    onSuccess: function (status) {
			        $scope.formStatus = status;
			        if ($scope.formStatus && isFurtherInfoValid()) {
			            saveApplicant();
			        }
			        return false;
			    },
			    onError: function () {
			        return false;
			    },
			    onValidate: function () {
			        var msg = '';
			        var nameValue = $("#applicantForm input[name='customer-name']").val();
			        if (nameValue != '' && nameValue.trim().split(' ').length < 2) {
			            msg += $scope.languages.APPLICANT_FORM.NAME_AT_LEAST_ERROR_MSG + "<br/>";
			        }

			        var coNameValue = $("#applicantForm input[name='co-customer-name']").val();
			        if (coNameValue != '' && coNameValue.trim().split(' ').length < 2) {
			            msg += (msg.length > 0 ? "* " : "") + $scope.languages.APPLICANT_FORM.CO_NAME_AT_LEAST_ERROR_MSG + "<br/>";
			        }

			    	if ($scope.applicant.CheckingAccount == null) {
			    	    msg += (msg.length > 0 ? "* " : "") + $scope.languages.APPLICANT_FORM.CHECKING_ACCOUNT_REQUIRE_ERROR_MSG + "<br/>";
			    	}

			    	if ($scope.applicant.SavingAccount == null) {
			    		msg += (msg.length > 0 ? "* " : "") + $scope.languages.APPLICANT_FORM.SAVING_ACCOUNT_REQUIRE_ERROR_MSG + "<br/>";
			    	}

			    	if ($scope.applicant.EnrollAutoPay == null) {
			    		msg += (msg.length > 0 ? "* " : "") + $scope.languages.APPLICANT_FORM.ENROLL_AUTO_PAY_REQUIRE_ERROR_MSG + "<br/>";
			    	}

			    	if (msg != '') {
			    		return {
			    			element: $("#validate-form input[name='check-acount']"),
			    			message: msg
			    		}
			    	}

			        return "";
			    }
			});

			//init the input masks
			$("[data-mask]").inputmask();
		}

		init();
	}

})();