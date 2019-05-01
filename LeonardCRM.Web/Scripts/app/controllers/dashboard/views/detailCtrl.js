(function () {
    'use strict';

    angular.module("LeonardCRM").controller("viewDetailCtrl", ctrl);

    ctrl.$inject = ["$scope", "$http", "$location", "$routeParams", "requestContext", "viewService", "entityFieldService", "appService", "toolbarService", "_", "$timeout", "filterService"];

    function ctrl($scope, $http, $location, $routeParams, requestContext, viewService, entityFieldService, appService, toolbarService, _, $timeout, filterService) {
        $scope.newView = {};
        $scope.currentView = {};
        $scope.viewColumns = [];
        $scope.fieldsSortable = [];
        $scope.fieldsGroup = [];
        $scope.viewlist = [];
        $scope.roles = [];
        $scope.sortDirection = '';
        $scope.sortColumnId = 0;
        $scope.orderByColumn = {};
        $scope.viewId = requestContext.getParamAsInt("viewId", 0);
        $scope.moduleId = appService.getModuleId("views");

        $scope.sortableOptions1 = {
            placeholder: 'field',
            connectWith: '.fields-container',
            stop: function (event, ui) {
                
            }
        };
        $scope.sortableOptions2 = {
            placeholder: 'field',
            connectWith: '.fields-container',
            receive: function (event, ui) {
                //check length
            }
        };

        $scope.select2Options = {
            allowClear: true,
            dropdownAutoWidth: true
            //minimumResultsForSearch: 10
        };

        var orderCol = {
            ColumnName: '',
            FieldId: 0,
            OrderDirection: 'Asc'
        };
        var groupCol = {
            ColumnName: '',
            FieldId: 0
        };

        var getEntityFieldByModuleIdSuccessCallback = function (data, status, headers, config) {
        	$scope.viewColumns = angular.fromJson(data);        	
            $scope.currentView.Eli_ViewColumns = [];
            angular.forEach($scope.viewColumns, function (item, index) {
                $scope.currentView.Eli_ViewColumns.push({
                    'ColumnName': item.ColumnName,
                    'ViewId': $scope.currentView.Id,
                    'FieldId': item.FieldId,
                    'SortOrder': index + 1,
                    'LabelDisplay': item.LabelDisplay,
                    'Visible': false,
                    'Width': '100px',
                    'AllowGroup': item.AllowGroup || false
                });
            });

            $scope.fieldsSortable = angular.copy($scope.viewColumns);

            $scope.fieldsGroup = _.where($scope.currentView.Eli_ViewColumns, { 'AllowGroup': true });
            $scope.fieldsGroup.splice(0, 0, { FieldId: 0, LabelDisplay: '' });

            var obj = {
                Fields: $scope.viewColumns,
                Conditions: $scope.currentView.Eli_ViewConditions
            };
            $scope.$broadcast('filterColumnsEvent', obj);
        };

        var saveViewSuccessCallBack = function (data, status, headers, config) {
            if (data.ReturnCode == 200) {
                NotifySuccess(data.Result, 5000);

                if ($scope.previousUrl == '') {
                    $location.path('/dashboard/views');
                } else {
                    $location.path($scope.previousUrl);
                }
            } else {
                NotifyError(data.Result, 5000);
            }
        };

        var getOrderByColumnById = function (id) {
            var col = _.findWithProperty($scope.viewColumns, 'FieldId', id);
            var name = '';
            if (col != null)
                name = col.ColumnName;
            return name;
        };

        var getViewByModuleIdSuccessCallback = function (data, status, headers, config) {
            $scope.viewlist = angular.fromJson(data);
        };
        
        $scope.module_SelectChanged = function () {
            entityFieldService.GetEntityFieldByModuleId($scope.currentView.ModuleId)
                .then(getEntityFieldByModuleIdSuccessCallback);

            viewService.GetViewByModuleId($scope.currentView.ModuleId)
                .then(getViewByModuleIdSuccessCallback);
        };

        $scope.defaultViewChange = function () {
            if ($scope.currentView.DefaultView) {
                $scope.currentView.LoadChildView = false;
                $scope.currentView.ParentId = '';
                $scope.currentView.Parent = [];
                $('#ddlParentIds').select2('val', '');
            }
        };

        $scope.loadChildViewChange = function () {
            if ($scope.currentView.LoadChildView) {
                $scope.currentView.DefaultView = false;
            }
        };

        //------Define scope's methods
        function initViewPage() {
            $.validate({
                validateOnBlur: true, // disable validation when input looses focus
                errorMessagePosition: 'top', // Instead of 'element' which is default
                scrollToTopOnError: false, // Set this property to true if you have a long form
                showHelpOnFocus: false,
                onSuccess: function (status) {
                    var cols = _.where($scope.currentView.Eli_ViewColumns, { 'Visible': true });
                    if (cols.length > 0) {
                        if (status) {
                            // SortOrder
                            var i = 1;
                            angular.forEach($scope.currentView.Eli_ViewColumns, function(item, index) {
                                if (item.Visible == true) {
                                    item.SortOrder = i;
                                    i++;
                                }
                            });
                            $scope.currentView.Eli_ViewColumns = _.sortBy($scope.currentView.Eli_ViewColumns, 'SortOrder');
                            //Set ParentId
                            var str = '';
                            if ($scope.currentView.Parent != null && $scope.currentView.Parent.length > 0) {
                                angular.forEach($scope.currentView.Parent, function (value, key) {
                                    str += '{' + value + '}';
                                });
                            }
                            $scope.currentView.ParentId = str;

                            //Set Order By
                            if ($scope.currentView.Eli_ViewGroupBy.length > 0 && $scope.currentView.Eli_ViewOrderBy[0].FieldId < 1) {
                                $scope.currentView.Eli_ViewOrderBy = [];
                            }
                            //Set Order By
                            if ($scope.currentView.Eli_ViewGroupBy.length > 0 && $scope.currentView.Eli_ViewGroupBy[0].FieldId < 1) {
                                $scope.currentView.Eli_ViewGroupBy = [];
                            }
                            $scope.currentView.CurrentModule = $scope.CurrentModule;
                            $scope.currentView.IsPublic = true;
                            $scope.currentView.IsActive = $scope.currentView.Id > 0 ? $scope.currentView.IsActive : true;;

                            viewService.SaveView($scope.currentView)
                                .then(saveViewSuccessCallBack);
                        }
                    } else {
                        NotifyError($scope.languages.VIEW.MUST_SELECT_COLUMN);
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

            $scope.NoSelectedItems = true;

            viewService.GetViewObjById($scope.viewId)
                .then(function (data) {
                    $scope.currentView = angular.fromJson(data);
                    $scope.newView = angular.copy($scope.newView);
                    $scope.viewColumns = angular.copy($scope.currentView.NameValues.viewColumns);
                    if ($scope.viewId > 0 && $scope.currentView != null) {

                        $scope.viewlist = angular.copy($scope.currentView.NameValues.viewlist);

                        $scope.fieldsSortable = angular.copy($scope.viewColumns);
                        $scope.fieldsSortable.splice(0, 0, {'FieldId':-1,'LabelDisplay':''});
                        $scope.fieldsGroup = _.where($scope.currentView.Eli_ViewColumns, { 'AllowGroup': true });
                        $scope.fieldsGroup.splice(0, 0, { FieldId: 0, LabelDisplay: '' });

                        var obj = {
                            Fields: $scope.viewColumns,
                            Conditions: $scope.currentView.Eli_ViewConditions
                        };
                        $timeout(function () {
                            $scope.$broadcast('filterColumnsEvent', obj);
                        }, 1500);
                    }

                    if ($scope.currentView != null && $scope.currentView.Eli_ViewOrderBy.length == 0) {
                        $scope.currentView.Eli_ViewOrderBy.push(orderCol);
                    }
                    if ($scope.currentView != null && $scope.currentView.Eli_ViewGroupBy.length == 0) {
                        $scope.currentView.Eli_ViewGroupBy.push(groupCol);
                    }

                    var referenceList = filterService.getReferenceList($scope.moduleId);
                    $scope.roles = _.where(referenceList, { "FieldName": "UserRole" });
                });
            toolbarService.NeedSaveCommand(true);
            $scope.setWindowTitle($scope.languages.VIEW.EDIT_VIEW);
        };

        //Toolbar events
        $scope.$on('cancelEvent', function (event) {
            if ($scope.previousUrl == '') {
                $location.path('/dashboard/views');
            } else {
                $location.path($scope.previousUrl);
            }
            event.preventDefault();
        });

        $scope.$on('saveEvent', function (event) {
            if ($scope.currentView != null) {
                $scope.$broadcast('needConditionsEvent');
            }
        });

        // Get the render context local to this controller (and relevant params).
        var renderContext = requestContext.getRenderContext('dashboard.views');

        // The subview indicates which view is going to be rendered on the page.
        $scope.subview = renderContext.getNextSection();


        $scope.orderByChange = function (fieldId) {
            $scope.currentView.Eli_ViewOrderBy[0].ColumnName = getOrderByColumnById(fieldId);
        };
        $scope.groupByChange = function (fieldId) {
            $scope.currentView.Eli_ViewGroupBy[0].ColumnName = getOrderByColumnById(fieldId);
        };

        $scope.$on('receivedConditionsEvent', function (event, args) {
            $scope.currentView.Eli_ViewConditions = args;
            var form = $('#userForm');
            form.submit();
            event.preventDefault();
        });

        // I handle changes to the request context.
        $scope.$on(
            "requestContextChanged",
            function () {

                $scope.viewId = requestContext.getParamAsInt("viewId", 0);
                if (requestContext.haveParamsChanged(["viewId"]) && requestContext.getParamAsInt("viewId", 0) > 0) {

                }

                // Make sure this change is relevant to this controller.
                if (!renderContext.isChangeRelevant()) {
                    return;
                }

                // Update the view that is being rendered.
                $scope.subview = renderContext.getNextSection();

            }
        );

        //---Initalize------
        initViewPage();


    }
}());