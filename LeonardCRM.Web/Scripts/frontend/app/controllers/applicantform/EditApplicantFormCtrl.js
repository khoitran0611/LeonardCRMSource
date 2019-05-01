(function () {

	'use strict';

	angular.module("LeonardCRMFrontEnd").controller("EditApplicantFormCtrl", ctrl);

	ctrl.$inject = ["$scope", "$location", 'salesCustomerService', "FileUploader", '_', 'feFilterService', 'appConfig', '$sce', 'orderAttachmentService', 'userService', '$timeout'];

	function ctrl($scope, $location, salesCustomerService, FileUploader, _, feFilterService, appConfig, $sce, orderAttachmentService, userService, $timeout) {
		var countInclude = 0;
		var changeEmailTimer = null;

		$scope.moduleId = 2;
		$scope.applicant = {};
		$scope.salesCustReferences = [];
		$scope.states = [];
		$scope.maillingStates = [];
		$scope.residenceTypes = [];
		$scope.landTypes = [];
		$scope.driverLicenseAttachments = [];
		$scope.stores = [];
		$scope.folderPath = "documents/";
		$scope.currentStep = 1;
		$scope.numOfStep = 6;
		$scope.numOfApplicant = 1;
		$scope.numOfReferences = 2;
		$scope.isShowCam = false;
		$scope.isLockContiue = false;
		$scope.isExistEmail = false;

		var initDate = new Date();
		initDate.setYear(initDate.getFullYear() - 10);
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

		var uploader = $scope.uploader = new FileUploader({
			scope: $scope,
			url: $scope.UploadHandlerUrl + "?folder=documents"
		});

		uploader.filters.push({
			name: 'filterName',
			fn: function (item) {				
			    var extension = item.name.split('.').pop();

			    if ($scope.modeParam != null && $scope.modeParam != '' && $scope.allowImageExt && $scope.allowImageExt.indexOf('pdf') == -1) {
			        $scope.allowImageExt.push('pdf');
			    }

				if ($.inArray(extension, $scope.allowImageExt) > -1 && item.size <= (parseInt($scope.maxUploadFileSize) * 1024 *1024)) {
					return true;
				}

				//error msg				
				return false;
			}
		});

		$scope.infoSectionsBase = [{ Name: $scope.languages.APPLICANT_FORM.APPLICATION_INFO, Order: 1, StepClass: '', MinWidth: 179 },
                                   { Name: $scope.languages.APPLICANT_FORM.APPLICANT_SECTION_TITLE, Order: 2, StepClass: '', MinWidth: 125 },
                                   { Name: $scope.languages.APPLICANT_FORM.ADDRESS_SECTION_TITLE, Order: 3, StepClass: '', MinWidth: 125 },
                                   { Name: $scope.languages.APPLICANT_FORM.REFERENCES_SECTION_TITLE, Order: 4, StepClass: '', MinWidth: 135 },
                                   { Name: $scope.languages.APPLICANT_FORM.EMPLOYMENT_SECTION_TITLE, Order: 5, StepClass: '', MinWidth: 215 },
                                   { Name: $scope.languages.APPLICANT_FORM.PAYMENT_SECTION_TITLE, Order: 6, StepClass: '', MinWidth: 200 }];

		$scope.infoSections = [];


		$scope.uploadAll = function () {
			$scope.uploader.uploadAll();
		};	

		$scope.Combobox_Changed = function (fieldValue, fieldName) {
			//var listTargetValueIds;
			//var result;
			//var data;
			//switch (fieldName) {
			//    case 'PhysicalCountry':
			//        result = _.filterWithProperty($scope.PickListDependencies, 'MasterValueId', fieldValue);
			//        if ($scope.PickListDependencies.length > 0 && result != null) {
			//            listTargetValueIds = _.uniq(_.pluck(result, 'ChildValueId'));
			//            data = _.filterWithValues($scope.PickList, 'Id', listTargetValueIds);
			//            $scope.states = _.filterWithProperty(data, 'FieldName', 'PhysicalState');
			//        }
			//        break;
			//    case 'MailingCountry':
			//        result = _.filterWithProperty($scope.PickListDependencies, 'MasterValueId', fieldValue);
			//        if ($scope.PickListDependencies.length > 0 && result != null) {
			//            listTargetValueIds = _.uniq(_.pluck(result, 'ChildValueId'));
			//            data = _.filterWithValues($scope.PickList, 'Id', listTargetValueIds);
			//            $scope.maillingStates = _.filterWithProperty(data, 'FieldName', 'MailingState');
			//        }
			//        break;
			//}
		}

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

		$scope.isEnableLandLordField = function () {
			return ($scope.applicant.ResidenceType != null && $scope.applicant.ResidenceType === appConfig.residenceTypes.Rent) || ($scope.applicant.LandType != null && $scope.applicant.LandType === appConfig.landTypes.Rent);
		}

		$scope.updateLandLordInfo = function () {
			if (!$scope.isEnableLandLordField()) { //disabled
				$scope.applicant.LandlordName = null;
				$scope.applicant.LandlordPhone = null;
			}
		}

		$scope.openCamera = function () {
			Webcam.set({
				width: 600,
				height: 400,
				dest_width: 1024,
				dest_height: 768,
				image_format: 'png'
			});

			Webcam.set('constraints', {
				width: 600,
				height: 400,
				dest_width: 1024,
				dest_height: 768,
			});

			Webcam.attach('#camera-section');
			$scope.isShowCam = true;
		}

		$scope.takeSnapshot = function () {
			Webcam.snap(function (data_uri) {
				orderAttachmentService.uploadBase64PNG(data_uri.replace("data:image/png;base64,", "")).then(function (respone) {
					if (respone.ReturnCode == 200) {
						if ($scope.applicant.AttachmentFiles == null) {
							$scope.applicant.AttachmentFiles = [];
						}

						$scope.applicant.AttachmentFiles.push(respone.Result);
						//$scope.closeCamera();
					}
					else {
						NotifyError(respone.Result);
					}
				});
			});
		}

		$scope.closeCamera = function () {
			Webcam.reset();
			$("#camera-section").removeAttr("style");
			$scope.isShowCam = false;
		}

		$scope.checkIfExistCustomerEmail = function (email) {		   
		    if ($scope.modeParam != null && $scope.modeParam != '') {//on assistant mode
		        if (changeEmailTimer != null) {
		            $timeout.cancel(changeEmailTimer);
		        }

		        changeEmailTimer = $timeout(function () {
		            if (email && email.length > 0) {
		                if (email.trim().toLowerCase() !== $scope.noUserEmail.trim().toLowerCase()) {
		                    userService.checkIfExistEmail(email).then(function (response) {
		                        $scope.isLockContiue = (response == true);
		                        $scope.isExistEmail = (response == true);
		                    });
		                }
		                else {
		                    $scope.isLockContiue = false;
		                    $scope.isExistEmail = false;
		                }
		            }
		            else {
		                $scope.isLockContiue = true;
		                $scope.isExistEmail = false;
		            }
		        }, 1000);
		    }
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
				PartNumber: $scope.applicant.PartNumber
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
					//window.location.href = '/#/my-applications';
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
					$scope.salesCustReferences.push({ Name: '', Relationship: null, Phone: '', IsActive : true });
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
					if ($scope.currentStep == 6) {
						var msg = '';
						if ($scope.applicant.CheckingAccount == null) {
							msg += $scope.languages.APPLICANT_FORM.CHECKING_ACCOUNT_REQUIRE_ERROR_MSG + "<br/>";
						}

						if ($scope.applicant.SavingAccount == null) {
							msg += (msg.length > 0 ? "* " : "") +  $scope.languages.APPLICANT_FORM.SAVING_ACCOUNT_REQUIRE_ERROR_MSG + "<br/>";
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
					}

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
				        if (nameValue != '' &&  nameValue.trim().split(' ').length < 2) {
				            msg += $scope.languages.APPLICANT_FORM.NAME_AT_LEAST_ERROR_MSG + "<br/>";				           
				        }

				        var coNameValue = $("#section-2 input[name='co-customer-name']").val();
				        if (coNameValue != '' && coNameValue.trim().split(' ').length < 2) {
				            msg += (msg.length > 0 ? "* " : "") + $scope.languages.APPLICANT_FORM.CO_NAME_AT_LEAST_ERROR_MSG + "<br/>";
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

					if ($scope.currentStep == 3) {
						if ($scope.applicant.ResidenceType == null) {
							msg += "Residence Type is required" + "<br/>";
						}

						if ($scope.applicant.LandType == null) {
							msg += (msg.length > 0 ? "* " : "") + "Land Type is required" + "<br/>";
						}

						if (msg != '') {
							return {
								element: $("#validate-form input[name='restype']"),
								message: msg
							}
						}
					}					

					return '';
				}
			});

			$("[data-mask]").inputmask();
			$scope.uploader.queue = [];

			initDataForSalesCustReferences();
			loadDataForDropdownList();
			$scope.setWindowTitle($scope.languages.APPLICANT_FORM.TITLE);
		}

		uploader.onCompleteAll = function (event, xhr, item, response) {
			$scope.uploader.queue = [];
		}

		uploader.onCompleteItem = function (item, response, status, headers) {
			if ($scope.applicant.AttachmentFiles == null) {
				$scope.applicant.AttachmentFiles = [];
			}
			$scope.applicant.AttachmentFiles.push(response[0].name);
		}

		$scope.$on('filterPickListEvent', function () {
			loadDataForDropdownList();
		});

		$scope.$on("$includeContentLoaded", function (event) {
			countInclude += 1;
			if (countInclude == $scope.numOfStep) {
				init();
			}
		});

		$scope.$on('$destroy', function () {
			$scope.closeCamera();
		});
	}
})();