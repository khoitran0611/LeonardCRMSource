(function (ng) {

    'use strict';

    angular.module("LeonardCRM").controller("EditSalesCustomerCtrl", ctrl);

    ctrl.$inject = ['$scope', '$http', '$location', '$timeout', 'FileUploader', 'salesCustomerService', 'filterService', 'noteService', 'viewService', 'appService', 'toolbarService', 'requestContext', 'windowService', 'entityFieldService', '_'];
        
    function ctrl($scope, $http, $location, $timeout, FileUploader, salesCustomerService, filterService, noteService, viewService, appService, toolbarService, requestContext, windowService, entityFieldService, _) {

        var anonymousUser = '/Content/images/user-anonymous.png';

        // --- Define Controller Methods. ------------------- //
        var getSalesCustomerByIdSuccessCallback = function (data, status, headers, config) {
            $scope.customer = angular.fromJson(data);
            if ($scope.customerId == 0) {
                $scope.customerShadow = angular.copy($scope.customer);
                $scope.setTitle($scope.languages.SALES_CUSTOMER.ADD_CUSTOMER_TITLE);
                $scope.currentUserId = angular.copy($scope.customer.UserIds[0]);
            } else {
                $scope.setTitle($scope.languages.SALES_CUSTOMER.EDIT_CUSTOMER_TITLE + ' ' + $scope.customer.Name);

                angular.forEach($scope.customer.RelateViews, function (item, index) {
                    var name = item.Name + appService.getModuleName(item.ModuleId).toUpperCase();
                    item.Name = $scope.languages.SALES_CUSTOMER[name];
                    item.Url = '/Home/RelatedView/' + item.ViewId + '/' + $scope.CurrentParam.Id + '/?t=' + Math.random();
                });
            }

            var pickList = filterService.getPickList($scope.CurrentParam.ModuleId);
            var referenceList = filterService.getReferenceList($scope.CurrentParam.ModuleId);
            $scope.pickListForm = {
                'Source': _.where(pickList, { 'FieldName': 'Source' }),
                'Title': _.where(pickList, { 'FieldName': 'Title' }),
                'ResponsibleUsers': _.where(referenceList, { 'FieldName': 'ResponsibleUsers' }),
                'Status': _.where(pickList, { 'FieldName': 'Status' }),
                'Industry': _.where(pickList, { 'FieldName': 'Industry' })
            };
            var properties = Object.keys($scope.customer);
            var fields = filterService.getFields($scope.CurrentParam.ModuleId);
            angular.forEach(properties, function (prop, index) {
                var field = _.findWithProperty(fields, 'ColumnName', prop);
                if (field != null) {
                    $scope.customer[prop + 'Control'] = {
                        'Visible': field.Visible,
                        'Locked': field.Locked
                    };
                } else {
                    $scope.customer[prop + 'Control'] = {
                        'Visible': false,
                        'Locked': true
                    };
                }
            });
            entityFieldService.setCustomData($scope.customer.CustomFields);
            $timeout(function () {
                $scope.$broadcast('parentModulePassEvent');
            }, 500);

            if ($scope.customer.Photo != null) {
                $scope.photo = $scope.showimage($scope.customer.Photo);
            } else {
                $scope.photo = anonymousUser;
            }
        };

        var addSalesCustomerSuccessCallback = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);
                if ($scope.customerId == 0) {
                    $scope.customerId = angular.copy(data.Id);
                    $scope.$emit('CloseAndOpenWindow', $scope.customerId);
                } else {
                    $scope.$emit('closeWindow');
                }
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var getViewSuccessCallback = function (data, status, headers, config) {
            $scope.pageInfo = angular.fromJson(data);
            $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
        };

        var getCustomer = function () {
            $scope.pageInfo.Id = $scope.customerId;

            $scope.pageInfo.ModuleId = $scope.CurrentParam.ModuleId;
            $scope.pageInfo.ViewId = $scope.CurrentParam.ViewId;
            salesCustomerService.GetSalesCustomerById($scope.pageInfo)
                .then(getSalesCustomerByIdSuccessCallback);
        };

        var initForm = function () {
            $('#CustomerTab a:first').tab('show');
            $('#CustomerTab a').click(function (e) {
                if ($(this).parent('li').hasClass('disabled')) {
                    return false;
                };
                e.preventDefault();
                $(this).tab('show');
            });

            $.formUtils.addValidator({
                name: 'urlValid',
                validatorFunction: function (value, $el, config, language, $form) {
                    var patt = new RegExp("^(http://|www+\.|https://)");
                    return patt.test(value);
                }
            });

            $.validate({
                form: '#customerForm',
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    $scope.formStatus = status;
                    if ($scope.formStatus && $scope.isUploaded) {
                        $scope.addSalesCustomer();
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

            appService.hasPermission($scope.CurrentParam.ModuleId).then(function (data) {
                $scope.hasPermission = data;
            });

            //$scope.customerId = $scope.CurrentParam.Id;
            getCustomer();
        };

        // --- Define Scope Methods. ------------------------ //

        $scope.addSalesCustomer = function () {
            for (var i = 0; i < $scope.FieldData.length; i++) {
                $scope.FieldData[i].CustFieldId = $scope.FieldData[i].FieldId;
                $scope.FieldData[i].MaterRecordId = $scope.customerId;
                if ($scope.FieldData[i].FieldDataId > 0) {
                    $scope.FieldData[i].Id = $scope.FieldData[i].FieldDataId;
                }
            }
            $scope.customer.FieldData = $scope.FieldData;
            $scope.customer.CustomFields = $scope.FieldData;

            $scope.customer.ModuleId = $scope.CurrentParam.ModuleId;
            if ($scope.customer.Id > 0) {
                salesCustomerService.SaveCustomer($scope.customer)
                    .then(addSalesCustomerSuccessCallback);
            } else {
                $scope.$broadcast('note_request');
            }
        };

        $scope.cancelClick = function () {

        };

        $scope.DeletePhoto = function (imageName) {
            $scope.isUploaded = false;
            $http.post($scope.UploadHandlerUrl + '?f=' + imageName)
                .success(function (data, status, headers, config) {
                    $scope.filesUploaded = [];
                    $scope.photo = $scope.customer.Photo = '';
                })
                .error(function (data, status, headers, config) {
                    alert(data);
                });
        };

        $scope.deleteLink = function (index) {
            $scope.customer.SalesCustLinks.splice(index, 1);
        };

        $scope.addLink = function () {
            if ($scope.customerlink.length > 0) {
                var cuslink = {
                    Id: 0,
                    CusId: $scope.customerId,
                    Link: $scope.customerlink
                };
                $scope.customer.SalesCustLinks.push(cuslink);
                $scope.customerlink = '';
            }
        };

        $scope.GetViewById = function (args) {
            $scope.pageInfo = args;
            if ($scope.pageInfo.ViewId > 0 && $scope.pageInfo.Id > 0) {
                viewService.GetView($scope.pageInfo)
                    .then(getViewSuccessCallback);
            }
        };

        $scope.CollapsedEvent = function (item) {
            item.Collapsed = !item.Collapsed;
        };

        // --- Define Controller Variables. ----------------- //

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext("customer");

        // --- Define Scope Variables. ---------------------- //
        $scope.customerId = $scope.CurrentParam.Id;
        $scope.customer = {};
        $scope.customerShadow = {};
        $scope.photo = '';
        $scope.formStatus = false;
        $scope.filesUploaded = [];
        $scope.customerlink = '';
        $scope.OrderGridUrl = '';
        $scope.AppointmentGridUrl = '';
        $scope.pageInfo = {};
        $scope.isUploaded = true;
        $scope.customFieldArgs = {};
        $scope.FieldData = [];
        $scope.UserIds = [];
        $scope.currentUserId = 0;
        $scope.pickListForm = {
            'Source': [],
            'Title': [],
            'ResponsibleUsers': []
        };
        $scope.pattern = '^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|www\\.){1}([0-9A-Za-z-\\.@:%_\+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?';
        //Upload File
        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            url: $scope.UploadHandlerUrl + "?folder=customers",
            autoUpload: true
        });

        uploader.filters.push({
            name: 'filterName',
            fn: function (item) {
                    var extension = item.name.toLowerCase().split('.').pop();
                    if ($.inArray(extension, $scope.allowImageExt) > -1) {
                        return true;
                    }
                    //error msg
                    return false;
                }
        });

        // --- Bind To Scope Events. ------------------------ //

        $scope.$on('GridPageIndexChanged', function (event, args) {
            $scope.pageInfo = args;
            $scope.GetViewById(args);
            event.preventDefault();
        });

        $scope.$on('sortEvent', function (event, args) {
            $scope.pageInfo = args;
            $scope.GetViewById(args);
            event.preventDefault();
        });

        $scope.$on('cancelEvent', function (event) {
            if ($scope.previousUrl == '') {
                var id = appService.getDefaultView('customer');
                $location.path('/customer/view/' + id);
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        $scope.$on('saveDataEvent', function (event) {
            $scope.$broadcast('parentSaveEvent');
            $scope.isUploaded = true;
            var form = $('#customerForm');
            form.submit();
            event.preventDefault();
        });

        $scope.$on('sendCutomFieldEvent', function (event, data) {
            $scope.FieldData = data;
        });

        $scope.$on('Grid_OnNeedDataSource', function (event, args) {
            $scope.pageInfo = args;
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

        $scope.$on('refreshDataEvent', function (event, args) {
            $scope.$broadcast('refeshDataByKey', args);
        });

        $scope.$on('ServerFilter', function (event, args) {
            viewService.serverFilterData(args.conditions, args.ViewId, args.ModuleId, args.Id, $scope.pageInfo.PageSize, $scope.pageInfo.PageIndex, $scope.pageInfo.GroupColumn, $scope.pageInfo.SortExpression)
                .then(function (data) {
                    $scope.pageInfo = angular.fromJson(data);
                    if ($scope.pageInfo != null)
                        $scope.$broadcast('Grid_DataBinding', $scope.pageInfo);
                });
        });

        $scope.$on('notes_saved', function (event, notes) {
            $scope.customer.Notes = notes;
            salesCustomerService.SaveCustomer($scope.customer)
                .then(addSalesCustomerSuccessCallback);
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

                // Get the relevant route IDs.
                $scope.customerId = requestContext.getParamAsInt("customerId", 0);

                if (requestContext.haveParamsChanged(["customerId"]) && requestContext.getParamAsInt("customerId", 0) > 0) {
                    getCustomer();
                }
                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {

                    return;
                }
            }
        );

        uploader.onSuccessItem = function (item, response, status, headers) {
            $scope.filesUploaded = response;
            var filename = $scope.filesUploaded[0].name;
            $scope.photo = $scope.showimage(filename);
            $scope.customer.Photo = filename;
            $scope.filesUploaded = [];
            $scope.uploader.clearQueue();
        };
        // --- Initialize. ---------------------------------- //

        initForm();
    }

})(angular);