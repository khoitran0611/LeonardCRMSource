/// <reference path="../../../../appviews/Order/detail.html" />
/// <reference path="../../../../appviews/Order/signaturePop.html" />
/// <reference path="../../../../appviews/Order/signaturePop.html" />
(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("EditSalesOrdersCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "$timeout", "salesInvoiceService", "noteService", "registryService", "viewService", "salesOrderService", "toolbarService", "appService", "$routeParams", "requestContext", "FileUploader", "windowService", "entityFieldService", "filterService", "_", "dialogService", "appConfig", "orderAttachmentService"];

    function ctrl($scope, $http, $location, $timeout, salesInvoiceService, noteService, registryService, viewService, salesOrderService, toolbarService, appService, $routeParams, requestContext, FileUploader, windowService, entityFieldService, filterService, _, dialogService, appConfig, orderAttachmentService) {        
        appService.hasPermission($scope.CurrentParam.ModuleId).then(function (data) {
            $scope.hasPermission = data;
        });

        appService.getCurrentUserRole().then(function (data) {
            $scope.currentUserRoleId = data;            
        });
        $scope.interestRate = (36 / 100 / 12); // 36 % per year
        $scope.currencyFormat = registryService.siteSettings.CURRENCY;
        $scope.orders = [];
        $scope.customers = [];
        $scope.users = [];
        $scope.selectedUser = [];
        $scope.selectedStatus = [];
        $scope.listname = [];
        $scope.userSelect2Option = {};
        $scope.select2 = '';
        $scope.select2Options = {};
        $scope.filesUploaded = [];
        $scope.attachment = '';
        $scope.totalUploadedFiles = 0;
        $scope.order = new Object();
        $scope.order.Filenames = new Array();
        $scope.file = {};
        $scope.orderid = $scope.CurrentParam.Id;
        $scope.parentId = $scope.CurrentParam.ParentId;
        $scope.InvoiceGridUrl = '';
        $scope.FieldData = [];
        $scope.customFieldArgs = {};
        $scope.pagedInfo = {};
        $scope.tempusers = [];
        $scope.tempcustomers = [];
        $scope.formStatus = false;
        $scope.SectionControls = {
            'InvoiceSection': { 'Collapsed': true }
        };
        $scope.pickListForm = {
            'Status': [],
            'RelatedCustomers': [],
            'ResponsibleUsers': [],
            'CapitalizationPeriods': [],
            'DeliveryStatus': []
        };

        $scope.applicant = {
            Editable: false,
            IsBelongAdmin: true
        };

        //status costant
        $scope.pendingStatus = appConfig.orderStatus.pendingStatus;
        $scope.preApprovalStatus = appConfig.orderStatus.preApprovalStatus;
        $scope.pendingCusAcceptStatus = appConfig.orderStatus.pendingCusAcceptStatus;
        $scope.inProgressStatus = appConfig.orderStatus.inProgressStatus;
        //$scope.contractSignedStatus = appConfig.orderStatus.contractSignedStatus;
        //$scope.paidFullStatus = appConfig.orderStatus.paidFullStatus;
        $scope.pendingDeliveryStatus = appConfig.orderStatus.pendingDeliveryStatus;
        //$scope.approvalStatus = appConfig.orderStatus.approvalStatus;
        $scope.completedStatus = appConfig.orderStatus.completedStatus;
        $scope.cancelledStatus = appConfig.orderStatus.cancelledStatus;

        $scope.residenceTypes = [];
        $scope.landTypes = [];
        $scope.maillingStates = [];
        $scope.states = [];
        $scope.relationships = [];
        $scope.onlyShowCustomer = true;
        $scope.orginalStatus = 0;
        $scope.contractPDFPath = '';
        $scope.deliveryPDFPath = '';
        $scope.acceptancePDFPath = '';
        //check if app is belonged to the sold process
        $scope.isSoldProcess = false;

        var isLoadedOrder = false;
        var managerSignatureApi = null;
        var numIncludedView = 0;
        var orderBase = null;
        var cancelAppKey = "cancelApp";
        $scope.myInit = function () {
            $('#Ordertab a:first').tab('show');
            $('#Ordertab a').click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            });

            $timeout(function () {
                $('.call-time-field').timepicker({
                    'showDuration': true,
                    'timeFormat': 'H:i',
                    'maxTime': '23:30'
                });
            });            

            var param = $scope.orderid.toString() + ',' + $scope.CurrentParam.ModuleId.toString();
            salesOrderService.LoadData(param)
            .then(function (data, status, headers, config) {
                var obj = angular.fromJson(data);
                $scope.order = obj;

                //parse the store number to string to map with picklist
                if ($scope.order.StoreNumber != null && $scope.order.StoreNumber > 0) {
                    $scope.order.StoreNumber = $scope.order.StoreNumber.toString();
                }

                var usernameTemp = new Array();

                var userId = appService.getCurrentUser();
                if ($scope.orderid == 0) {
                    //set the current userid when this user create a new order
                    usernameTemp.push(userId);
                    //$scope.order.ResponsibleUsers = usernameTemp;

                    //set default customer when user go from customer page
                    var custtemp = new Array();
                    if ($scope.parentId != 0) {
                        custtemp.push($scope.parentId);
                        $scope.order.SalesCustomers = custtemp;
                    }
                    $scope.setTitle($scope.languages.ORDERS.ADD_ORDER_TITLE);
                } else {
                    //when user update an exsiting order
                    $scope.setTitle($scope.languages.ORDERS.EDIT_ORDER_TITLE + ' ' + $scope.order.Id);

                    if ($scope.order.SalesOrderCustomers != null) {
                        $scope.order.SalesCustomers = $scope.order.RelatedCustomers.split(',');
                    }

                    $scope.order.OrginalStatus = $scope.orginalStatus = angular.copy($scope.order.Status);

                    loadAppInfoTab();

                    //mark as the "Sold" process or not
                    $scope.isSoldProcess = $scope.order.IsSold === true;
                }
                var pickList = filterService.getPickList($scope.CurrentParam.ModuleId);
                var referenceList = filterService.getReferenceList($scope.CurrentParam.ModuleId);
                $scope.pickListForm = {
                    'Status': _.where(pickList, { 'FieldName': 'Status' }),
                    'RelatedCustomers': _.where(referenceList, { 'FieldName': 'RelatedCustomers' }),
                    'ResponsibleUsers': $scope.order.UsersPicklist,
                    'CapitalizationPeriods': _.where(pickList, { 'FieldName': 'CapitalizationPeriod' }),
                    'DeliveryStatus': [],
                    'Stores': []
                };
                loadUserPickList();

                // Get delivery picklist
                angular.forEach($scope.pickListForm.Status, function (item, index) {
                    if (item.Id === $scope.paidFullStatus) {
                        $scope.pickListForm.DeliveryStatus.push(item);
                    }
                });

                if ($scope.order.Id == 0) {
                    $scope.order.Status = $scope.pickListForm.Status[0].Id;
                } else {
                    $scope.InvoiceGridUrl = '/Home/SubView/' + $scope.CurrentParam.ViewId + '/' + $scope.CurrentParam.ModuleId + '/' + $scope.orderid.toString();
                }
                $scope.filesUploaded = data.SalesDocuments.concat();
                var properties = Object.keys($scope.order);
                var fields = filterService.getFields($scope.CurrentParam.ModuleId);
                angular.forEach(properties, function (prop, index) {
                    var field = _.findWithProperty(fields, 'ColumnName', prop);
                    if (field != null) {
                        var noTrackingField = ["CreatedDate", "CreatedBy", "ModifiedDate", "ModifiedBy", "IsActive", "SerialNumber", "ResponsibleUsers",
                                             "RentToOwn", "SaleDate", "Color", "POSTicketNumber", "Status", "GPCustomerID", "GPOrderNumber", "AcceptDate", "PromoCode",
                                             "IsOldCustomer", "PartNumber", "StoreNumber", "IsNewPart", "RampPartNumber", "DisapprovedReason", "RampSalePrice", "IsApproveOrder", "DisapprovedReason"];

                        $scope.order[prop + 'Control'] = {
                            'Visible': field.Visible,
                            'Locked': noTrackingField.indexOf(prop) > -1 ? field.Locked : (field.Locked || (!$scope.order.IsAdminRoleUsers && $scope.order.Status >= $scope.pendingDeliveryStatus)),
                            'Mandatory': field.Mandatory
                        };
                    } else {
                        $scope.order[prop + 'Control'] = {
                            'Visible': false,
                            'Locked': true,
                            'Mandatory': false
                        };
                    }
                });

                entityFieldService.setCustomData($scope.order.CustomFields);
                $timeout(function () {
                    $scope.$broadcast('parentModulePassEvent');
                }, 500);

                //check if the status is locked
                $scope.order.StatusControl.Locked = $scope.order.Status >= $scope.paidFullStatus;

                isLoadedOrder = true;


                if ($scope.order.Status == $scope.preApprovalStatus) {
                    orderBase = angular.copy($scope.order);
                }

                $scope.order.IsCancelDisapproved = $scope.order.Status == $scope.cancelledStatus;

                $scope.setSaveButtonVisible($scope.order.IsAdminRoleUsers ||
                                            !(($scope.isSoldProcess !== true && ($scope.order.IsFinalize == true || $scope.order.IsCancelDisapproved)) ||
                                              ($scope.isSoldProcess === true && $scope.order.Status >= $scope.completedStatus)));
            });
        };

        //------------------------Event-----------------------------

        //----------------START upload file--------------------------
        $scope.flag = false;
        //configuration for uploading file
        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            url: $scope.UploadHandlerUrl + "?folder=documents",
            autoUpload: true
        });


        $scope.uploadAll = function () {
            $scope.uploader.uploadAll();
        };

        $scope.changeCallDate = function () {
            var callDate = $scope.order.SalesOrderCompletes[0].CallDate;
            if (callDate == null || callDate == '') {
                $scope.order.SalesOrderCompletes[0].CallTime = null;
            }
        }

        $scope.changeSpeakCustomer = function () {
            if ($scope.order.SalesOrderCompletes[0].SpokeWithCustomer == true) {
                $scope.order.SalesOrderCompletes[0].LeftVoiceMail = false;
            }
            else {
                $scope.order.SalesOrderCompletes[0].LeftVoiceMail = true;
            }
        };

        $scope.changeLeftVoice = function () {
            if ($scope.order.SalesOrderCompletes[0].LeftVoiceMail == true) {
                $scope.order.SalesOrderCompletes[0].SpokeWithCustomer = false;
                $scope.order.SalesOrderCompletes[0].FollowUpComment = null;
            }
            else {
                $scope.order.SalesOrderCompletes[0].SpokeWithCustomer = true;
            }
        }

        $scope.openSignPop = function () {
            enableOrDisableForm(false);
            $scope.FormTitle = "Create Signature";
            $scope.detailFormUrl = '/appviews/Order/signaturePop.html';

            $("#dialogDetail p.error").remove();
            $('#dialogDetail input.error').removeClass("error");

            var modal = $('#dialogDetail').appendTo("body").modal({
                show: true,
                keyboard: false,
                backdrop: 'static'
            });
            dialogService.hideAllButtons(false);

            $('#dialogDetail').off('shown.bs.modal');
            $('#dialogDetail').on('shown.bs.modal', function () {
                managerSignatureApi = $('#manager-sigPad').signaturePad({ validateFields: false, lineTop: 85 });
                managerSignatureApi.clearCanvas();
                $("#print-name-customer").val('');
            });
        }

        $scope.hidePopup = function () {
            enableOrDisableForm(true);
            $('#dialogDetail').modal('hide');
        }

        $scope.finalizeOrder = function () {
            //var isValid = managerSignatureApi.validateForm();

            //if (isValid) {
            //	enableOrDisableForm(true);
            //	$scope.order.SalesOrderCompletes[0].ManagerSignatureUrl = managerSignatureApi.getSignatureImage($scope.order.SalesOrderCompletes[0].ManagerSignIP, $("#print-name-customer").val()).replace("data:image/png;base64,", "");

            //	var tempArray = new Array();
            //	for (var i = 0; i < $scope.filesUploaded.length; i++) {
            //		tempArray[i] = $scope.filesUploaded[i].name;
            //	}
            //	$scope.order.Filenames = tempArray;

            //	salesOrderService.finalizeOrer($scope.order)
            //    .then(function (data) {
            //    	if (data.ReturnCode == 200) {
            //    		NotifySuccess(data.Result, 5000);
            //    		$scope.hidePopup();
            //    		setTimeout(function () {
            //    			$('#dialogDetail').remove();
            //    			$scope.$emit('CloseAndOpenWindow', $scope.orderid);
            //    		}, 500);
            //    	} else {

            //    		NotifyError(data.Result, 10000);
            //    	}
            //    	$scope.order.SalesOrderCompletes[0].ManagerSignatureUrl = '';
            //    });
            //}

            salesOrderService.finalizeOrer($scope.order)
                .then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result, 5000);
                        $scope.hidePopup();
                        setTimeout(function () {
                            $('#dialogDetail').remove();
                            $scope.$emit('CloseAndOpenWindow', $scope.orderid);
                        }, 500);
                    } else {

                        NotifyError(data.Result, 10000);
                    }
                    $scope.order.SalesOrderCompletes[0].ManagerSignatureUrl = '';
                });
        }

        $scope.changeRampNumber = function () {
            var isClearRampPart = $scope.order.RampPartNumber == null || $scope.order.RampPartNumber == '';
            $scope.order.RampPaidAtSign = isClearRampPart ? null : $scope.order.RampPaidAtSign;
            $scope.order.RampTax = isClearRampPart ? null : $scope.order.RampTax;
            $scope.order.RampReduction = isClearRampPart ? null : $scope.order.RampReduction;
        }

        $scope.changedMaillingAddress = function () {
            if ($scope.applicant.IsSameMailingAddress == true) {
                $scope.applicant.PhysicalStreet = $scope.applicant.MailingStreet;
                $scope.applicant.PhysicalCity = $scope.applicant.MailingCity;
                $scope.applicant.PhysicalZip = $scope.applicant.MailingZip;
                $scope.applicant.PhysicalState = $scope.applicant.MailingState;
            }
        }

        $scope.cancelApp = function () {
            $scope.SetConfirmMsg($scope.languages.APPLICANT_FORM.CANCEL_APPICANT_CONFIRM, cancelAppKey, $scope.languages.APPLICANT_FORM.CANCEL_APPICANT_CONFIRM_HEADER);
        }

        // ADDING FILTERS
        uploader.filters.push({
            name: 'filterName',
            fn: function (item) {
                return true;
            }
        });

        uploader.onAfterAddingAll = function (item) {
            $scope.totalUploadedFiles = 1;
        };
        var arrayOffFileName = new Array();

        uploader.onSuccessItem = function (item, response, status, headers) {
            var successCb = function () {
                for (var i = 0; i < response.length; i++) {
                    $scope.filesUploaded.push(response[i]);
                    arrayOffFileName.push(response[i].name);
                }

                $scope.totalUploadedFiles = 0;
                var filename = $scope.filesUploaded[0].name;
                $scope.attachment = $scope.UploadFolderUrl + filename;
            };

            var arrayPosted = [];
            for (var i = 0; i < response.length; i++) {
                arrayPosted.push({ 'FileName': response[0].name });
            }

            orderAttachmentService.SaveAttachment($scope.order.Id, arrayPosted, true)
                             .then(function (data, status, headers, config) {
                                 if (data.ReturnCode == 200) {
                                     successCb();
                                 }
                                 else {
                                     NotifyError(data.Result);
                                     //clear the selected file
                                     $scope.uploader.queue = [];
                                 }
                             });
        };

        $scope.getFileName = function (filePath) {
            if (filePath != null && filePath.length > 0 && filePath.indexOf('/') > -1) {
                var lastQuoute = filePath.lastIndexOf('/');
                filePath = filePath.substr(lastQuoute + 1);
            }
            return filePath;
        }

        //----------------END upload file----------------------------

        //--------------START DELETE ATTACHMENT----------------------
        $scope.DeleteAttachment = function (fileName) {
            for (var i = 0; i < $scope.filesUploaded.length; i++) {
                if ($scope.filesUploaded[i].name.trim() == fileName.trim()) {
                    $scope.filesUploaded.splice(i, 1);
                }
            }
        };

        //--------------END DELETE ATTACHMENT------------------------


        //--------------START ADD NEW ORDER--------------------------

        $scope.addorder = function () {
            //if not budget is not a number then display error message
            //if (isNaN($scope.order.Budget) == true) {
            //    NotifyError($scope.languages.ORDERS.INVALID_BUDGET, 5000);
            //    return;
            //}

            //hanlde uploaded file names
            var tempArray = new Array();
            for (var i = 0; i < $scope.filesUploaded.length; i++) {
                tempArray[i] = $scope.filesUploaded[i].name;
            }
            $scope.order.Filenames = tempArray;

            for (var i = 0; i < $scope.FieldData.length; i++) {
                $scope.FieldData[i].CustFieldId = $scope.FieldData[i].FieldId;
                $scope.FieldData[i].MaterRecordId = $scope.orderid;
                if ($scope.FieldData[i].FieldDataId > 0) {
                    $scope.FieldData[i].Id = $scope.FieldData[i].FieldDataId;
                }
            }
            $scope.order.FieldData = $scope.FieldData;
            $scope.order.CustomFields = $scope.FieldData;

            //change RelatedCustomers and ResponsibleUsers back to string type currently they are array type
            //var users = "";
            var customers = "";
            //if ($scope.order.ResponsibleUsers != null && $scope.order.ResponsibleUsers.length >0) {
            //    for (var j = 0; j < $scope.order.ResponsibleUsers.length; j++) {
            //        users += $scope.order.ResponsibleUsers[j] + ",";
            //    }
            //}

            if ($scope.order.SalesCustomers != null && $scope.order.SalesCustomers.length > 0) {
                for (var j = 0; j < $scope.order.SalesCustomers.length; j++) {
                    customers += $scope.order.SalesCustomers[j] + ",";
                }
            }
            //$scope.order.ResponsibleUsers = users;
            $scope.order.RelatedCustomers = customers;

            if ($scope.order.Id > 0) {
                var order = angular.copy($scope.order);
                if (order.Status <= $scope.completedStatus) //allow to edit the customer info when the status is pending or preapprove					
                {
                    order.SalesCustomer = $scope.applicant;
                }

                salesOrderService.AddNewOrder(order, $scope.CurrentParam.ModuleId)
                    .then(function (data, status, headers, config) {
                        if (data.ReturnCode == 200) {
                            NotifySuccess(data.Result, 5000);
                            if ($scope.orderid == 0) {
                                $scope.orderid = angular.copy(data.Id);
                                $scope.$emit('CloseAndOpenWindow', $scope.orderid);
                            } else {
                                $scope.$emit('closeWindow');
                            }
                        } else {

                            NotifyError(data.Result, 10000);
                        }
                    });
            } else {
                $scope.$broadcast('note_request');
            }
        };

        //--------------END ADD NEW ORDER----------------------------
        var openViewFormPop = function (title, filePath) {
            $scope.filePath = filePath;
            $scope.FormTitle = title;
            $scope.detailFormUrl = '/appviews/Order/viewForm.html';
            $('#dialogDetail').appendTo("body").modal({
                show: true,
                keyboard: false,
                backdrop: 'static'
            });
            dialogService.hideAllButtons(true);
            dialogService.hideSaveButton(false);
        }

        var enableOrDisableForm = function (isEnable) {
            if (!isEnable) {
                $('[data-validation]').each(function (index, ele) {
                    var jEle = $(ele)
                    jEle.attr('validation', jEle.attr('data-validation'));
                    jEle.attr('data-validation', '');
                });
            }
            else {
                $('[data-validation]').each(function (index, ele) {
                    var jEle = $(ele)
                    jEle.attr('data-validation', jEle.attr('validation'));
                });
            }
        }

        //-----------------START CONFIGURATION FOR JQUERY FORM VALIDATIOIN PLUGIN------------------


        //------------------END CONFIGURATION FOR JQUERY FORM VALIDATION PLUGIN--------------------

        //-------------------------START GETVIEWBYID----------------------------
        var getViewSuccessCallback = function (data, status, headers, config) {
            $scope.pagedInfo = angular.fromJson(data);
            $scope.$broadcast('Grid_DataBinding', $scope.pagedInfo);
        };

        var getViewErrorCallback = function (data, status, headers, config) {
        };

        var loadAppInfoTab = function () {
            $scope.applicant = angular.copy($scope.order.SalesCustomer);
            $scope.applicant.IsBelongAdmin = true;
            $scope.order.SalesCustomer = null;
            $scope.applicant.Editable = $scope.order.Status < $scope.completedStatus && $scope.order.Status != $scope.cancelledStatus;
            if ($scope.applicant.SalesCustReferences != null &&
				$scope.applicant.SalesCustReferences.length < 3) {
                $scope.applicant.SalesCustReferences.push({ Id: 0, Name: '', Relationship: 0, Phone: '' });
            }
            appService.getPickListByModules([appConfig.customerModule, appConfig.saleCusRefModule], false).then(function (data) {
                var picklist = angular.fromJson(data).PickList;
                $scope.residenceTypes = _.filterWithProperty(picklist, 'FieldName', 'ResidenceType');
                $scope.landTypes = _.filterWithProperty(picklist, 'FieldName', 'LandType');
                $scope.maillingStates = $scope.states = _.filterWithProperty(picklist, 'FieldName', 'PhysicalState');

                $scope.relationships = _.filterWithProperty(picklist, 'FieldName', 'Relationship');
            });
        }

        var initValidateForm = function () {
            $.validate({
                form: '#orderform',
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                submitHandler: function (form) { // for demo
                    return false;
                },
                onSuccess: function (status) {
                    $scope.formStatus = status;
                    if ($scope.formStatus && isFurtherInfoValid()) {
                        $scope.addorder();
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
        }

        function isFurtherInfoValid() {
            var error = "";

            if (!$scope.applicant.HomePhone && !$scope.applicant.CellPhone) {
                error += $scope.languages.APPLICANT_FORM.AT_LEAST_PHONE_ERROR_MSG + "</br>";
            }

            if (!$scope.isSoldProcess && !isAdditionalReferenceEmpty() && !isAdditionalReferenceFull()) {
                error += $scope.languages.APPLICANT_FORM.REF_RELATION_PHONE_3_REQUIRE_ERROR_MSG + "</br>";
            }

            if (!!error) {
                NotifyError(error);
                return false;
            }

            return true;
        }

        function isAdditionalReferenceEmpty() {
            return !$scope.applicant.SalesCustReferences[2].Name && !$scope.applicant.SalesCustReferences[2].Relationship && !$scope.applicant.SalesCustReferences[2].Phone;
        }

        function isAdditionalReferenceFull() {
            return !!$scope.applicant.SalesCustReferences[2].Name && !!$scope.applicant.SalesCustReferences[2].Relationship && !!$scope.applicant.SalesCustReferences[2].Phone;
        }

        function saveData() {
            $scope.$broadcast('parentSaveEvent');
            var form = $('#orderform');
            form.submit();
        }

        function loadUserPickList() {
            var userPickList = filterService.getPickList(appConfig.usersModule);
            if (userPickList == null || userPickList.length == 0) {
                filterService.preLoadFilterColumnsAndPickLists(appConfig.usersModule);
            }
            else {
                $scope.pickListForm.Stores = _.filterWithProperty(userPickList, 'FieldName', 'StoreId');
            }
        }

        function onChangeSalesPrice(newVal, oldVal) {
            var reduceAmt = ($scope.order.ReductionPayment != null && $scope.order.ReductionPayment > 0 ? $scope.order.ReductionPayment : 0);
            if (newVal) {
                newVal -= reduceAmt;

                var months = parseInt(_.find($scope.pickListForm.CapitalizationPeriods, function (item) {
                    return item.Id == $scope.order.CapitalizationPeriod;
                }).AdditionalInfo); // should be taken from capitalization period

                $scope.order.MonthlyPayment1 = Math.round(salesOrderService.PMT($scope.interestRate, months, -newVal));

                if (!$scope.order.MonthlyPayment2)
                    $scope.order.MonthlyPayment2 = 0;
                $scope.order.TotalMonthly = Math.round($scope.order.MonthlyPayment1 + parseFloat($scope.order.MonthlyPayment2));
            } else {
                $scope.order.MonthlyPayment1 = $scope.order.MonthlyPayment2 = $scope.order.TotalMonthly = 0;
            }

            if (oldVal != null && oldVal > 0) {
                oldVal -= reduceAmt;
            }

            if (isLoadedOrder && newVal != oldVal && angular.isDefined(oldVal) && angular.isDefined(newVal)) {
                $scope.order.PaidAtSigning = $scope.order.MonthlyPayment1 * $scope.order.NumberOfPayment;
                $scope.onChangeTaxPercent();
            }
        }

        function onChangeRampSalesPrice(newVal, oldVal) {
            var reduceAmt = ($scope.order.RampReduction != null && $scope.order.RampReduction > 0 ? $scope.order.RampReduction : 0);

            if (newVal) {
                newVal -= reduceAmt;

                var months = parseInt(_.find($scope.pickListForm.CapitalizationPeriods, function (item) {
                    return item.Id == $scope.order.CapitalizationPeriod;
                }).AdditionalInfo); // should be taken from capitalization period

                $scope.order.MonthlyPayment2 = Math.round(salesOrderService.PMT($scope.interestRate, months, -newVal));

            } else {
                $scope.order.MonthlyPayment2 = 0;
            }

            if (!$scope.order.MonthlyPayment1)
                $scope.order.MonthlyPayment1 = 0;
            $scope.order.TotalMonthly = Math.round($scope.order.MonthlyPayment1 + parseFloat($scope.order.MonthlyPayment2));

            if (oldVal != null && oldVal > 0) {
                oldVal -= reduceAmt;
            }

            if (isLoadedOrder && newVal != oldVal && angular.isDefined(oldVal) && angular.isDefined(newVal)) {
                $scope.order.RampPaidAtSign = $scope.order.MonthlyPayment2 * $scope.order.NumberOfPayment;
                $scope.onChangeTaxPercent();
            }
        }


        $scope.GetViewById = function (args) {
            $scope.pagedInfo = args;
            if ($scope.pagedInfo.ViewId > 0) {
                viewService.GetView($scope.pagedInfo)
                    .then(getViewSuccessCallback);
            }
        };
        //---------------------END GETVIEWBYID---------------------------------

        $scope.viewContract = function () {
            if (null) {
                console.log("test");
            }
            if ($scope.contractPDFPath == null || $scope.contractPDFPath == '' || $scope.order.IsFinalize != true) {
                $scope.savePromise = salesOrderService.getPDFByForm(0, $scope.order.Id, $scope.order.IsFinalize == true).then(function (data) {
                    if (data.ReturnCode == 200) {
                        $scope.contractPDFPath = data.Result;
                        openViewFormPop($scope.languages.ORDERS.VIEW_CONTRACT_BTN, $scope.contractPDFPath);
                    }
                    else {
                        NotifyError(data.Result, 5000);
                    }
                });
            }
            else {
                openViewFormPop($scope.languages.ORDERS.VIEW_CONTRACT_BTN, $scope.contractPDFPath);
            }
        }

        $scope.viewDeliveryForm = function () {
            $scope.savePromise1 = salesOrderService.getPDFByForm(1, $scope.order.Id, $scope.order.Status >= $scope.pendingDeliveryStatus).then(function (data) {
                if (data.ReturnCode == 200) {
                    $scope.deliveryPDFPath = data.Result;
                    openViewFormPop($scope.languages.ORDERS.VIEW_DELIVERY_FORM_BTN, $scope.deliveryPDFPath);
                }
                else {
                    NotifyError(data.Result, 5000);
                }
            });
        }

        $scope.viewAcceptanceForm = function () {
            if ($scope.acceptancePDFPath == null || $scope.acceptancePDFPath == '') {
                $scope.savePromise2 = salesOrderService.getPDFByForm(2, $scope.order.Id, true, $scope.order.IsFinalize).then(function (data) {
                    if (data.ReturnCode == 200) {
                        $scope.acceptancePDFPath = data.Result;
                        openViewFormPop($scope.languages.ORDERS.VIEW_ACCEPTANCE_FORM_BTN, $scope.acceptancePDFPath);
                    }
                    else {
                        NotifyError(data.Result, 5000);
                    }
                });
            }
            else {
                openViewFormPop($scope.languages.ORDERS.VIEW_ACCEPTANCE_FORM_BTN, $scope.acceptancePDFPath);
            }
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

        $scope.approveOrDisapprove = function () {
            $scope.order.DisapprovedReason = null;
            $scope.order.IsCancelDisapproved = null;

            if ($scope.order.IsApproveOrder) {
                $scope.order.Status = $scope.pendingDeliveryStatus;
            }
            else {
                $scope.order.Status = $scope.inProgressStatus;
            }
        }

        $scope.onChangeStatus = function () {
            if ($scope.orginalStatus >= $scope.inProgressStatus) {
                //sync with the approval controls
                $scope.order.IsApproveOrder = $scope.order.Status >= $scope.approvalStatus && $scope.order.Status != $scope.cancelledStatus;
                $scope.order.IsCancelDisapproved = $scope.order.Status == $scope.cancelledStatus;
            }

            if ($scope.order.Status <= $scope.preApprovalStatus) {//back before the inprogress
                //clear the approval controls
                $scope.order.IsApproveOrder = null;
                $scope.order.DisapprovedReason = null;
                $scope.order.IsCancelDisapproved = null;
            }
        }

        $scope.onCancelDisapproveOrder = function () {
            $scope.order.Status = $scope.order.IsCancelDisapproved ? $scope.cancelledStatus : $scope.inProgressStatus;
        }

        $scope.onChangeTaxPercent = function () {
            $scope.order.Tax = Math.round((($scope.order.SalesTaxPercent ? $scope.order.SalesTaxPercent : 0) * 0.01) * ($scope.order.MonthlyPayment1 ? $scope.order.MonthlyPayment1 : 0) * 100) / 100;
            $scope.order.SaleTax = Math.round((($scope.order.SalesTaxPercent ? $scope.order.SalesTaxPercent : 0) * 0.01) * ($scope.order.SalesPrice ? $scope.order.SalesPrice : 0) * 100) / 100;

            $scope.order.RampTax = Math.round((($scope.order.SalesTaxPercent ? $scope.order.SalesTaxPercent : 0) * 0.01) * ($scope.order.MonthlyPayment2 ? $scope.order.MonthlyPayment2 : 0) * 100) / 100;
        }

        $scope.onChangeReductionPayment = function () {
            onChangeSalesPrice($scope.order.SalesPrice, 0);
        }

        $scope.onChangeRampReductionPayment = function () {
            onChangeRampSalesPrice($scope.order.RampSalePrice, 0);
        }

        //------------------START HANDLE EVENT---------------------------

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pagedInfo = args;
            $scope.GetViewById(args);
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

        $scope.$on('rowClickEvent', function (event, args) {
            var param = {
                Url: '/appviews/' + args.ModuleName + '/detail.html',
                Id: args.RowId,
                ParentId: args.ParentId,
                ViewId: args.ViewId,
                ModuleId: args.ModuleId,
                Key: args.Key
            };
            windowService.openWindow(param);
        });

        $scope.$on('PageIndexChanged', function (event, args) {
            $scope.pagedInfo = args;
            $scope.GetViewById(args);
        });

        $scope.$on('saveDataEvent', function (event, args) {
            saveData();
        });

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.$broadcast('refeshDataByKey', args);
        });

        $scope.$on('cancelEvent', function (event) {
            if ($scope.previousUrl == '') {
                var id = appService.getDefaultView('order');
                $location.path('/order/view/' + id);
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pagedInfo = args;
            $scope.GetViewById(args);
            event.preventDefault();
        });

        $scope.$on('sendCutomFieldEvent', function (event, data) {
            $scope.FieldData = data;
        });

        $scope.$watch('order.SalesPrice', function (newVal, oldVal) {
            onChangeSalesPrice(newVal, oldVal);
        });

        $scope.$watch('order.RampSalePrice', function (newVal, oldVal) {
            onChangeRampSalesPrice(newVal, oldVal);
        });

        $scope.$watch('order.CapitalizationPeriod', function (newVal, oldVal) {
            if (newVal) {
                //var months = parseInt(_.find($scope.pickListForm.CapitalizationPeriods, function (item) {
                //	return item.Id == $scope.order.CapitalizationPeriod;
                //}).AdditionalInfo); // should be taken from capitalization period

                //$scope.order.MonthlyPayment1 = Math.round(salesOrderService.PMT($scope.interestRate, months, -$scope.order.SalesPrice));
                //$scope.order.MonthlyPayment2 = Math.round(salesOrderService.PMT($scope.interestRate, months, -$scope.order.RampSalePrice));

                onChangeSalesPrice($scope.order.SalesPrice, 0);
                onChangeRampSalesPrice($scope.order.RampSalePrice, 0);
                //$scope.order.TotalMonthly = Math.round($scope.order.MonthlyPayment1 + parseFloat($scope.order.MonthlyPayment2));
            }
        });

        $scope.$on('ServerFilter', function (event, args) {
            viewService.serverFilterData(args.conditions, $scope.pagedInfo.ViewId, $scope.pagedInfo.ModuleId, args.Id, $scope.pagedInfo.PageSize, $scope.pagedInfo.PageIndex, $scope.pagedInfo.GroupColumn, $scope.pagedInfo.SortExpression)
                .then(function (data) {
                    $scope.pagedInfo = angular.fromJson(data);
                    if ($scope.pagedInfo != null)
                        $scope.$broadcast('Grid_DataBinding', $scope.pagedInfo);
                });
        });

        $scope.$on('notes_saved', function (event, notes) {
            $scope.order.Notes = notes;
            salesOrderService.AddNewOrder($scope.order, $scope.CurrentParam.ModuleId)
                    .then(function (data, status, headers, config) {
                        if (data.ReturnCode == 200) {
                            NotifySuccess(data.Result, 5000);
                            if ($scope.orderid == 0) {
                                $scope.orderid = angular.copy(data.Id);
                                $scope.$emit('CloseAndOpenWindow', $scope.orderid);
                            } else {
                                $scope.$emit('closeWindow');
                            }
                        } else {

                            NotifyError(data.Result, 5000);
                        }
                    });
        });

        $scope.$on('yesEvent', function (event, args) {
            if (args == cancelAppKey) {
                $scope.cancelPromise = salesOrderService.cancelApplication($scope.orderid).then(function (data) {
                    if (data.ReturnCode == 200) {
                        NotifySuccess(data.Result.Message);
                        $scope.$emit('CloseAndOpenWindow', $scope.orderid);
                    } else {
                        NotifyError(data.Result);
                    }
                });
            }
        });

        $scope.$on('noEvent', function (event, args) {

        });

        $scope.$on('filterPickListEvent', function (event, moduleId) {
            if (moduleId == appConfig.usersModule) {
                loadUserPickList();
            }
        });

        var renderContext = requestContext.getRenderContext("order");
        $scope.$on("requestContextChanged", function () {
            // Get the relevant route IDs.                   

            // Make sure this change is relevant to this controller.
            if (!renderContext.isChangeRelevant()) {

                return;
            }
        });

        $scope.$watch("order.POSTicketNumber", function (newVal, oldVal) {
            if (isLoadedOrder == true && angular.isDefined(oldVal) && newVal != oldVal) {
                if (newVal != "" && $scope.order.Status === $scope.contractSignedStatus) {
                    $scope.order.Status = $scope.paidFullStatus;
                }
                else if (newVal == "" && $scope.orginalStatus > 0 && $scope.orginalStatus == $scope.contractSignedStatus) {
                    $scope.order.Status = $scope.contractSignedStatus;
                }
            }
        });

        $scope.$on("$includeContentLoaded", function (event) {
            numIncludedView += 1;
            if (numIncludedView == 5) {
                $("[data-mask]").inputmask();
                initValidateForm();
            }
        });

        //-----------------END HANDLE EVENT-----------------------------

        //$scope.setTitle($scope.languages.ORDERS.DETAIL_TITLE);
    }
})(angular);